using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PaperRecognize.Business;
using PaperRecognize.Models;
using PaperRecognize.DTOs.AndroidDTO;
using PaperRecognize.DTOs.PaperDTO;
using PaperRecognize.DTOs;
using System.Text;
using AutoMapper;
using System.IO;
using PaperRecognize.Import;

namespace PaperRecognize.Repository
{
    /// <summary>
    /// 管理员操作的具体实现类
    /// </summary>
    public class AdminRepository:UserRepository
    {
        /// <summary>
        /// 将一篇论文分配给一个候选人
        /// </summary>
        /// <param name="candidateId">候选人的ID</param>
        public void Accept( int candidateId )
        {
            var candidate = context.Candidate.FirstOrDefault(c => c.Id == candidateId);
            if (null == candidate) return;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var c in candidate.Author.Candidate)
                    {
                        c.status = (int)CandidateStatus.WRONG;
                    }
                    candidate.status = (int)CandidateStatus.RIGHT;
                    context.SaveChanges();
                    UpdatePaperStatus(candidate.Author.Paper);
          
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
                
            }
                
        }
        /// <summary>
        /// 否决该作者的所有候选人
        /// </summary>
        /// <param name="authorId">作者的ID</param>
        public void RejectAll(int authorId)
        {
            Author author = context.Author.FirstOrDefault( a=>a.Id == authorId );
            if (author == null) return;
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var c in author.Candidate)
                    {
                        c.status = (int)CandidateStatus.WRONG;
                    }
                    if (author.Paper.status == (int)PaperStatus.DEAL)
                    {
                        author.Paper.status = (int)PaperStatus.CONFIRM;
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
                
            }
        }
        /// <summary>
        /// 使用论文名或者作者名所属学院等搜索论文
        /// </summary>
        /// <param name="search">用来搜索的关键字：
        ///     PaperName，论文名，可以只有一部分；
        ///     AuthorName，作者名；
        ///     Depart，论文所属学院。
        /// </param>
        /// <returns>搜索到的论文列表</returns>
        internal IEnumerable<GetOnePaperDTO> SearchPaper(SearchPaperDTO search)
        {

            var papers = PaperRecognize.Business.LookupPaper
                .Lookup.LookupPaperByName(search.PaperName);
            return papers;
        }

        /// <summary>
        /// 获取需要管理员处理的论文，作者候选人里有状态为check的论文
        /// </summary>
        /// <param name="page">分页显示，输入需要显示第几页的数据</param>
        /// <returns>需要处理的论文列表</returns>
        public List<GetOnePaperDTO> GetPapers( int page )
        {
            string sql = @"select * from Paper where Id in(
                         select PaperId from Author where Id in (
                         select AuthorId from Candidate where status = {1}));";

            return context.Database
                .SqlQuery<GetOnePaperDTO>(sql, (int)CandidateStatus.CHECK)
                .Skip( (page -1) * pageSize )
                .Take( pageSize )
                .ToList();
        }
        /// <summary>
        /// 获取一篇论文的所有作者及其候选人
        /// </summary>
        /// <param name="paperId">论文ID</param>
        /// <returns>论文作者列表，作者对象里有它的候选人列表</returns>
        public List<AuthorDTO> GetCandidate(int paperId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select Id, Ordinal, NameEN, Department
                        from Author where PaperId = {0};");
            List<AuthorDTO> authors = context.Database
                .SqlQuery<AuthorDTO>( sql.ToString(), paperId )
                .ToList();
            
            foreach (AuthorDTO a in authors)
            {
                var aps = context.Candidate.Where(ap => ap.AuthorId == a.Id).ToList();
                var pvos = changeToPersonVO( aps );
                a.Candidates.AddRange(pvos);
            }
            return authors;
        }

        private List<CandidateVO> changeToPersonVO( List<Candidate> aps )
        {
            List<CandidateVO> pvos = new List<CandidateVO>();
            foreach (Candidate ap in aps)
            {
                CandidateVO vo = new CandidateVO();
                vo.CandidateId = ap.Id;
                vo.Status = (int)ap.status;
                var person = context.Person.FirstOrDefault(p => p.PersonNo == ap.PersonNo);
                if (null != person)
                {
                    vo.PersonNo = person.PersonNo;
                    vo.NameEN = person.NameEN;
                    vo.Mobile = person.Mobile;
                    vo.Email = person.Email;
                }
                vo.NameCN = ap.Name;
                pvos.Add(vo);
            }
            return pvos;
        }

        /// <summary>
        /// 将论文数据从Excel导入到数据库
        /// </summary>
        /// <returns>导入成功返回success，否则返回一条错误信息</returns>
        public string ImportPapers()
        {
            string path = HttpRuntime.AppDomainAppPath + @"App_Data/Paper";
            string[] files = Directory.GetFiles(path);
            if (null == files) return "success";
            ImportPaper import = new ImportPaper();
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (string name in files)
                    {
                        if (name.EndsWith(".xls") || name.EndsWith(".xlsx"))
                        {
                            import.ExcelToSQLServer(name, "Sheet1", true);
                            File.Delete(name);
                        }
                    }

                    Pretreatment p = new Pretreatment();
                    p.pretreatPaper();

                    transaction.Commit();
                }
                catch (Exception e )
                {
                    transaction.Rollback();
                    return e.Message;
                }
                
            }
            return "success";
        }

    }
}