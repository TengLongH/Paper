using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PaperRecognize.Models;
using PaperRecognize.DTOs;
using System.Text;

using PaperRecognize.Log;
using System.Data;
using System.Data.Entity;
namespace PaperRecognize.Business
{
    public class LookupAuthor
    {
        /// <summary>
        /// 获取日志对象的实例
        /// </summary>
        private static readonly log4net.ILog Log = LogHelper.GetLogger();

        /// <summary>
        /// 数据库对象
        /// </summary>
        private DBModel context = new DBModel();
        /// <summary>
        /// 查找所有论文的作者对应的人员
        /// </summary>
        public void LookupPapers()
        {
            
            string sql = "select Id from Paper where status ={0}";
            string updateSql = "update Paper set status={0} where Id ={1}";
            List<int> paperIds = context.Database
                .SqlQuery<int>(sql, (int)PaperStatus.PRETREATMENT)
                .ToList();
            int i = 0;
            while (i < paperIds.Count)
            {
                using (DbContextTransaction tran = context.Database.BeginTransaction())
                {
                    try
                    {
                        LookupPaperAuthors(paperIds[i]);
                        context.Database.ExecuteSqlCommand(updateSql, (int)PaperStatus.CONFIRM, paperIds[i]);
                        context.SaveChanges();
                        tran.Commit();
                        i++;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();
                        Log.Error("Id= " + paperIds[i] + "lookup error" + e.Message);
                    }
                }
            }
        }
        /// <summary>
        /// 查找一篇论文的作者对应的人员
        /// </summary>
        /// <param name="paperId">查找的论文ID</param>
        public void LookupPaperAuthors( int paperId )
        {
            string sql = "select * from Author where PaperId={0}";
            string updateAuthorStatus = "update Author set HasCandidate={0} where Id={1}";
            List<Author> authors = context.Database
                .SqlQuery<Author>(sql,paperId)
                .ToList();
           
            for (int i = 0; i < authors.Count; i++ )
            {
                var find = LookupCandidate(authors.ElementAt(i));
                context.Database.ExecuteSqlCommand(updateAuthorStatus, find ? 1 : 0, authors.ElementAt(i).Id);
            }
        }
        /// <summary>
        /// 查找一个作者的候选人
        /// </summary>
        /// <param name="author">需要查找候选人的作者对象</param>
        /// <returns>是否找到了候选人，找到为true，没找到为false</returns>
        public bool LookupCandidate(Author author )
        {
            //如果是外单位人员
            if (null != author.IsOtherUnit && (bool)author.IsOtherUnit)
            {
                Candidate ap = new Candidate();
                ap.AuthorId = author.Id;
                ap.Name = "外单位";
                ap.status = (int)CandidateStatus.RIGHT;
                ap.Operator = "system";
                context.Candidate.Add(ap);
                return true;
            }

            string rootDepartSql = @"select DepartmentId from DepartmentAlias where Alias in (" + TransList(author.Department)+" )";
            List<int> departId = context.Database.SqlQuery<int>(rootDepartSql ).ToList().Distinct().ToList();
            
            List<string> personNos;
            //如果找不到该部门从全校范围搜索，结束
            if (null == departId || departId.Count <= 0)
            {
                personNos = GetPersonFromShool(author);
            }
            //找到该部门从该部门及下属部门里搜索
            else
            {
                personNos = GetPersonFromDepart(author, departId);
            }
            //如果未找到此人
            if (personNos == null)
            {
                return false;
            }
            
            //将搜索到的候选人添加进数据库
            string personSql = "select NameCN from Person where PersonNo={0}";

            //如果只找到一个人或者多个人，将状态设置为待审核，操作者为system, Author的hasCandidate=true
            foreach (string no in personNos)
            {
                string[] name = context.Database.SqlQuery<string>(personSql, no).ToArray();
                if (null != name && name.Length >= 1)
                {
                    Candidate ap = new Candidate();
                    ap.AuthorId = author.Id;
                    ap.PersonNo = no;
                    ap.Name = name[0];
                    ap.Operator = "system";
                    ap.status = (int)CandidateStatus.CHECK;
                    context.Candidate.Add(ap);
                }
            }
            return true;
        }
        /// <summary>
        /// 从学校范围内查找作者候选人
        /// </summary>
        /// <param name="author">需要查找候选人的作者对象</param>
        /// <returns>找到的候选人的学号集合</returns>
        public List<string> GetPersonFromShool(Author author)
        {
            string sql = @"select distinct PersonNo from Person where lower(NameEN)=lower({0}) or lower(NameENAbbr)=lower({1})";
            List<string> personNos = context.Database.SqlQuery<string>(
                sql, author.NameEN, author.NameENAbbr).ToList();
            return personNos;
        }
        /// <summary>
        /// 在院系范围内查找作者候选人
        /// </summary>
        /// <param name="author">需要查找候选人的作者对象</param>
        /// <param name="departIds">院系的ID列表</param>
        /// <returns>找到的候选人学号列表</returns>
        public List<string> GetPersonFromDepart(Author author, List<int> departIds)
        {

            if (departIds.Count <= 0) return null;
            string departPersonIdSql = "select distinct PersonId from Person_Department where DepartmentId in ("+TransList(departIds)+")";
            StringBuilder personNoSql = new StringBuilder();
            personNoSql.Append("select distinct PersonNo from Person where ( lower(NameEN)=lower({0}) or lower(NameENAbbr)=lower({1}) ) and Id in( ");
            personNoSql.Append(departPersonIdSql);
            personNoSql.Append(")");

            List<string> personNos = context.Database.SqlQuery<string>(
                personNoSql.ToString(), author.NameEN, author.NameENAbbr)
                .ToList();
            if (personNos.Count > 0)
            {
                return personNos;
            }
            else
            {
                return GetPersonFromDepart(author, GetChildDepartIds(departIds));
            }
        }

        /// <summary>
        /// 获取下一级部门的ID
        /// </summary>
        /// <param name="departIds">父级部门的ID列表</param>
        /// <returns>子级部门的ID列表</returns>
        public List<int> GetChildDepartIds(List<int> departIds)
        {
            string sql = @"select Id from Department where ParentId in ("+ TransList(departIds)+")";
            List<int> ids = context.Database.SqlQuery<int>(
                sql, TransList(departIds))
                .ToList().Distinct().ToList();
            return ids;
        }
        /// <summary>
        /// 将int型数组转换成字符串
        /// </summary>
        /// <param name="list">需要转换的into数组</param>
        /// <returns>生成的字符串</returns>
        private string TransList(List<int> list )
        {
            string[] strArray = list.Select(i => i.ToString()).ToArray();
            return String.Join( ",", strArray );
        }

        /// <summary>
        /// 将 departmentName1;departmentName2;...departmentName3这种格式的字符串转化成
        /// ‘departmentName1’，‘departmentName2’，...‘departmentName3’这种格式
        /// </summary>
        /// <param name="strDeparts">输入的字符串</param>
        /// <returns>转化的结果</returns>
        private string TransList(string strDeparts)
        {
            if (null == strDeparts) return "";
            List<string> departs = strDeparts.Split(new char[] { ';' }).ToList();
            StringBuilder builder = new StringBuilder();
            foreach (string value in departs)
            {
                builder.Append("'");
                builder.Append(value.ToString());
                builder.Append("'");
                builder.Append(',');
            }
            if (builder.Length >= 1)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            string str = builder.ToString();
            return str;
        }
    }
}