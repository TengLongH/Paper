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

namespace PaperRecognize.Repository
{
    public class AdminRepository:UserRepository
    {

        public IEnumerable<GetConfirmDTO> GetConfirmPapers()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select p.Id as Id, p.PaperName as PaperName,ap.Id as AuthorPersonId 
                        from Paper p, Author a,( select * from Author_Person where status = 0 ) ap
                        where ap.AuthorId = a.Id and a.PaperId = p.Id;");

            var result = context.Database
               .SqlQuery<GetConfirmDTO>(sql.ToString());
            if (result != null)
            {
                return result.ToList<GetConfirmDTO>();
            }
            else
            {
                return new List<GetConfirmDTO>();
            }
        }
        public IEnumerable<GetConfirmDTO> GetCancelPapers()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select p.Id as Id, p.PaperName as PaperName,ap.Id as AuthorPersonId 
                        from Paper p, Author a,( select * from Author_Person where status = {0}) ap
                        where ap.AuthorId = a.Id and a.PaperId = p.Id;");

            var result = context.Database
               .SqlQuery<GetConfirmDTO>(sql.ToString(), (int)AuthorPersonStatus.RIGHT);
            if (result != null)
            {
                return result.ToList<GetConfirmDTO>();
            }
            else
            {
                return new List<GetConfirmDTO>();
            }
        }
        public List<GetOnePaperDTO> GetClaimPaper()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from Paper where Id in( 
                        select PaperId from Author where Id in(
                        select AuthorId from Author_Person where [status]={0} or [status]={1}));");
            var list = context.Database.SqlQuery<GetOnePaperDTO>(sql.ToString(),
                AuthorPersonStatus.CLAIM, AuthorPersonStatus.REJECT );
            return list.ToList();
        }

        public string AssignedPaper( int authorPersonId, int personNo )
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select * from Author_Person where AuthorId in( 
                        select AuthorId from Author_Person where Id = '54643')");
            var authorPersons = context.Database.SqlQuery<Author_Person>(sql.ToString(), authorPersonId ).ToList();
            

            return "success";
        }

        internal IEnumerable<GetOnePaperDTO> SearchPaper(SearchPaperDTO search)
        {

            var papers = PaperRecognize.Business.LookupPaper
                .Lookup.LookupPaperByName(search.PaperName);
            return papers;
        }

        public List<AuthorDTO> GetPaperAuthors(int paperId)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select Id, Ordinal, NameEN, Department
                        from Author where PaperId = {0};");
            List<AuthorDTO> authors = context.Database
                .SqlQuery<AuthorDTO>( sql.ToString(), paperId )
                .ToList();
            
            foreach (AuthorDTO a in authors)
            {
                var aps = context.Author_Person.Where(ap => ap.AuthorId == a.Id).ToList();
                var pvos = changeToPersonVO( aps );
                a.Authors.AddRange(pvos);
            }
            return authors;
        }

        private List<AuthorPersonVO> changeToPersonVO( List<Author_Person> aps )
        {
            List<AuthorPersonVO> pvos = new List<AuthorPersonVO>();
            foreach (Author_Person ap in aps)
            {
                AuthorPersonVO vo = new AuthorPersonVO();
                vo.AuthorPersonId = ap.Id;
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

        public void CancelAuthorPerson(CancelDTO dto)
        {
            Author_Person item = context.Author_Person.FirstOrDefault(ap => ap.Id == dto.AuthorPersonId);
            item.status = (int)AuthorPersonStatus.WRONG;
            Author_Person claim = new Author_Person();
            claim.status = (int)AuthorPersonStatus.NEEDCLAIM;
            claim.AuthorId = item.AuthorId;
            claim.Name = "not found";
            context.Author_Person.Add(claim);
            context.SaveChanges();
        }
    }
}