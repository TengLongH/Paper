using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PaperRecognize.Models;
using PaperRecognize.DTOs.AndroidDTO;
using PaperRecognize.DTOs.PaperDTO;
using PaperRecognize.DTOs;
using System.Text;

namespace PaperRecognize.Repository
{
    public class AdminRepository
    {
        private DBModel context = new DBModel();

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

        public List<PersonVO> changeToPersonVO( List<Author_Person> aps )
        {
            List<PersonVO> pvos = new List<PersonVO>();
            foreach (Author_Person ap in aps)
            {
                PersonVO vo = new PersonVO();
                vo.PersonAuthorId = ap.Id;
                vo.Status = (int)ap.status;
                if (vo.Status != (int)AuthorPersonStatus.NEEDCLAIM)
                {
                    var person = context.Person.FirstOrDefault(p => p.PersonNo == ap.PersonNo);
                    if (null != person)
                    {
                        vo.PersonNo = person.PersonNo;
                        vo.NameCN = person.NameCN;
                        vo.NameEN = person.NameEN;
                        vo.Mobile = person.Mobile;
                        vo.Email = person.Email;
                    }
                }
                else
                {
                    vo.NameCN = "未找到";
                }
                pvos.Add(vo);
            }
            return pvos;
        }
    }
}