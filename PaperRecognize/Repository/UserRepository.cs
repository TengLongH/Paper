using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperRecognize.Models;
using PaperRecognize.DTOs.UserDTO;
using PaperRecognize.DTOs;
using System.Text.RegularExpressions;
using PaperRecognize.DTOs.PersonDTO;
using AutoMapper;
using PaperRecognize.DTOs.PaperDTO;
using PaperRecognize.DTOs.Author;
using PaperRecognize.DTOs.AndroidDTO;

namespace PaperRecognize.Repository
{
    /// <summary>
    /// 用户操作的具体实现类
    /// </summary>
    public class UserRepository : RecognizeRepository
    {
        /// <summary>
        /// 分页，每页显示的条目数
        /// </summary>
        public static readonly int pageSize = 8;
        /// <summary>
        /// 构造函数
        /// </summary>
        public UserRepository()
        {

        }
        /// <summary>
        /// 获取系统推荐给该用户的论文信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>系统推荐的论文信息列表</returns>
        
        public List<GetAuthorDTO> GetSystemPushAuthor(string username)
        {
            string sql = @"select p.Id as PaperId, p.PaperName as PaperName, a.Id as AuthorId, a.NameEN as AuthorName
                           from Paper p, (select * from Author where id in ( 
                           select AuthorId from Candidate where PersonNo={0} and status = {1} and Operator={2}) ) a
                           where p.Id = a.PaperId";
            var authors = context.Database
               .SqlQuery<GetAuthorDTO>(sql.ToString(), username, (int)CandidateStatus.CHECK, "system")
               .ToList();

            string systemPushCount = @"select Id from Candidate where AuthorId={0} and and Operator={1}";
            foreach (var a in authors)
            {
                a.SystemCandidateCount = context.Database
                    .SqlQuery<int>(systemPushCount, a.AuthorId, "system")
                    .Count();
            }
            return authors;
        }
        /// <summary>
        /// 获取用户从认领平台认领的论文
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户认领的论文列表</returns>
        internal List<GetAuthorDTO> GetMyClaimAuthor(string username)
        {
            string sql = @"select p.Id as PaperId, p.PaperName as PaperName, a.Id as AuthorId, a.NameEN as AuthorName
                           from Paper p, (select * from Author where id in ( 
                           select AuthorId from Candidate where PersonNo={0} and status = {1} and Operator={2}) ) a
                           where p.Id = a.PaperId";

            var authors = context.Database
                .SqlQuery<GetAuthorDTO>(sql.ToString(), username, (int)CandidateStatus.CHECK, "user")
                .ToList();
            return authors;

        }

        /// <summary>
        /// 获取分配给该用户的论文
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>分配给该用户的论文列表</returns>
        internal IEnumerable<GetOnePaperDTO> GetMyPaper(string username)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append(@"select * from Paper where Id in(
                         select PaperId from Author where Id in (
                         select AuthorId from Candidate where PersonNo ={0} and status = {1}));");
            List<GetOnePaperDTO> papers = context.Database
                .SqlQuery<GetOnePaperDTO>(sql.ToString(), username, (int)CandidateStatus.RIGHT)
                .ToList();

            return papers;
        }
        /// <summary>
        /// 获取一篇论文的详细信息，如论文的作者列表，所属实验室，关键字，摘要等等
        /// </summary>
        /// <param name="paperId">论文的ID</param>
        /// <returns></returns>
        public PaperDetailDTO GetPaperDetail(int paperId)
        {
            PaperDetailDTO paperDetail = context.Paper.Select(Mapper.Map<PaperDetailDTO>)
                .FirstOrDefault(p => p.Id == paperId);
            return paperDetail;
        }
        /// <summary>
        /// 用户确认系统推荐的论文信息。如果系统只找到一个候选人，用户可以接受或者拒绝
        /// 如果系统找到多个候选人，用户只能拒绝
        /// </summary>
        /// <param name="dto">有三个属性：
        /// 1.Username，用户名。
        /// 2.CandidateId,候选人信息的ID。
        /// 3.Belong, 用户接受为true，用户拒绝为false
        /// </param>
        public void ConfirmSystemPushAuthor(ConfirmDTO dto)
        {
            var candidate = context.Candidate.FirstOrDefault(c => c.Id == dto.CandidateId);
            if (null == candidate) return;
            //只能确认推荐给他的论文
            if (!dto.Username.Equals(candidate.PersonNo)) return;
            //该条候选人信息必须是由系统分发的
            if (candidate.Operator.Equals("system")) return;
            //该条候选人信息的状态必须是待审核
            if (candidate.status != (int)CandidateStatus.CHECK) return;

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (dto.Belongs)
                    {
                        //如果只有一条系统推荐信息
                        if (candidate.Author.Candidate.Count == 1)
                        {
                            candidate.status = (int)CandidateStatus.RIGHT;
                            context.SaveChanges();
                            UpdatePaperStatus(candidate.Author.Paper);
                        }
                    }
                    else
                    {
                        candidate.status = (int)CandidateStatus.WRONG;
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
        /// 如果将某一个候选人的状态设置为right，就需要查看一下论文的状态是否需要更改
        /// </summary>
        /// <param name="paper">论文对象，看该论文对象的状态是否需要更改</param>
        public void UpdatePaperStatus(Paper paper)
        {
            foreach (Author a in paper.Author)
            {
                //Author的作者中必须有一人为right
                if (!a.Candidate.Any(c => c.status == (int)CandidateStatus.RIGHT))
                {
                    return;
                }
            }
            paper.status = (int)PaperStatus.DEAL;
            context.SaveChanges();
        }

        
        /// <summary>
        /// 从认领平台认领作者
        /// </summary>
        /// <param name="claim">它有两个属性：
        ///     1.Username，认领的用户名
        ///     2.AuthorId,被认领的论文作者的ID
        /// </param>
        public void ClaimAuthor(ClaimDTO claim )
        {
            Author author = context.Author.FirstOrDefault(a => a.Id == claim.AuthorId);
            if (null == author) return;
            //该作者必须是没有候选人的
            if (!isNoneCandidatesAuthor(author.Id)) return;
            Person person = context.Person.First(p => p.PersonNo == claim.Username );

            //禁止重复添加
            var count = context.Candidate
                .Where(c => c.AuthorId == claim.AuthorId && c.PersonNo == claim.Username)
                .ToList().Count;
            if (count > 0) return;

            //生成一条新的候选人认领信息
            Candidate candidate = CreateNewCandidate(author, person);
            context.Candidate.Add(candidate);
            context.SaveChanges();
        }

        /// <summary>
        /// Person是Author的候选人，使用Person和Author对象生成一个新的候选人对象
        /// </summary>
        /// <param name="author">作者对象</param>
        /// <param name="person">人员对象</param>
        /// <returns>生成的候选人对象</returns>
        public Candidate CreateNewCandidate(Author author, Person person)
        {
            Candidate candidate = new Candidate();
            candidate.AuthorId = author.Id;
            candidate.Name = person.NameCN;
            candidate.Operator = "user";
            candidate.PersonNo = person.PersonNo;
            candidate.status = (int)CandidateStatus.CHECK;
            return candidate;
        }

        /// <summary>
        /// 判断一个作者有没有候选人
        /// </summary>
        /// <param name="authorId">作者的ID</param>
        /// <returns>如果作者没有候选人返回true，反之为false</returns>
        public bool isNoneCandidatesAuthor(int authorId)
        {
            return NoneCandidatesAuthors().Contains(authorId);
        }

        /// <summary>
        /// 获取没有候选人的作者
        /// </summary>
        /// <param name="page">数据分页显示，输入的页码</param>
        /// <returns>没有候选人的作者列表</returns>
        public List<GetAuthorDTO> GetNoneCandidateAuthor( int page )
        {
            List<int> authorIds = NoneCandidatesAuthors();
            string ids = string.Join(",", authorIds.ToArray());

            string sql = @"select p.Id as PaperId, p.PaperName as PaperName, a.Id as AuthorId, a.NameEN as AuthorName
                           from Paper p, (select * from Author where id in (" + ids + @") ) a
                           where p.Id = a.PaperId";
            return context.Database.SqlQuery<GetAuthorDTO>(sql)
                .Skip( (page -1) * pageSize )
                .Take(pageSize)
                .ToList();
        }

        /// <summary>
        /// 获取所有没有候选人作者的ID列表
        /// </summary>
        /// <returns>无候选人的作者ID列表</returns>
        public List<int> NoneCandidatesAuthors()
        {
            //选出没有check候选人和right候选人的作者
            string sql = @"select Id from Author where Id not in(
				   select distinct AuthorId from Candidate 
				   where Candidate.status = {0} or Candidate.status = {1})";
            List<int> authors = context.Database
                .SqlQuery<int>(sql, (int)CandidateStatus.CHECK, (int)CandidateStatus.RIGHT)
                .ToList();
            return authors;
        }
    }
}
