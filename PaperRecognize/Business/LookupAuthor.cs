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
        private static readonly log4net.ILog Log = LogHelper.GetLogger();

        private DBModel context = new DBModel();
        public void LookupPapers()
        {
            
            string sql = "select Id from Paper where status ={0}";
            string updateSql = "update Paper set status={0} where Id ={1}";
            List<int> paperIds = context.Database
                .SqlQuery<int>(sql, (int)PaperStatus.ANALISIS)
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

        public void LookupPaperAuthors( int paperId )
        {
            string sql = "select * from Author where PaperId={0}";
            List<Author> authors = context.Database
                .SqlQuery<Author>(sql,paperId)
                .ToList();
            
            foreach (Author a in authors)
            {
                LookupAuthorPerson(a);
            }
        }

        private void LookupAuthorPerson(Author author )
        {
            //如果是外单位人员
            if (null != author.IsOtherUnit && (bool)author.IsOtherUnit)
            {
                Author_Person ap = new Author_Person();
                ap.AuthorId = author.Id;
                ap.Name = "外单位";
                ap.status = (int)AuthorPersonStatus.RIGHT;
                context.Author_Person.Add(ap);
                return;
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
            if (null == personNos || personNos.Count <= 0)
            {
                Author_Person ap = new Author_Person();
                ap.AuthorId = author.Id;
                ap.Name = "未找到";
                //ap.status = (int)AuthorPersonStatus.CONFIRM;
                ap.status = (int)AuthorPersonStatus.NEEDCLAIM;
                context.Author_Person.Add(ap);
                return;
            }
            //将搜索到的候选人添加进数据库
            string personSql = "select NameCN from Person where PersonNo={0}";
            foreach (string no in personNos)
            {
                string[] name = context.Database.SqlQuery<string>(personSql, no).ToArray();
                if (null != name && name.Length >= 1)
                {
                    Author_Person ap = new Author_Person();
                    ap.AuthorId = author.Id;
                    ap.PersonNo = no;
                    ap.Name = name[0];
                    ap.status = (int)AuthorPersonStatus.CONFIRM;
                    context.Author_Person.Add(ap);
                }
                
            }
         
        }

        private List<string> GetPersonFromShool(Author author)
        {
            string sql = @"select distinct PersonNo from Person where lower(NameEN)=lower({0}) or lower(NameENAbbr)=lower({1})";
            List<string> personNos = context.Database.SqlQuery<string>(
                sql, author.NameEN, author.NameENAbbr).ToList();
            return personNos;
        }
        private List<string> GetPersonFromDepart(Author author, List<int> departIds)
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

        private List<int> GetChildDepartIds(List<int> departIds)
        {
            string sql = @"select Id from Department where ParentId in ("+ TransList(departIds)+")";
            List<int> ids = context.Database.SqlQuery<int>(
                sql, TransList(departIds))
                .ToList().Distinct().ToList();
            return ids;
        }

        private string TransList(List<int> list )
        {
            StringBuilder builder = new StringBuilder();
            foreach (int value in list)
            {
                builder.Append(value.ToString());
                builder.Append(',');
            }
            if (builder.Length >= 1)
            {
                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }
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