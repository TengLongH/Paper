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
    public class UserRepository:RecognizeRepository
    {

        public UserRepository()
        {

        }
        internal List<GetClaimDTO> GetMyClaimPaper(string username )
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select ap.Id as AuthorPersonId,a.NameEN as AuthorName,p.Id as Id,PaperName 
                        from Author a, Paper p,(select * from Author_Person where status = {0} and PersonNo={1} ) ap 
                        where ap.AuthorId = a.Id and a.PaperId = p.Id;");
            var papers = context.Database
                .SqlQuery<GetClaimDTO>(sql.ToString(), (int)AuthorPersonStatus.CLAIM, username)
                .ToList();
            return papers;

        }

       
        internal IEnumerable<GetOnePaperDTO> GetMyPaper(string username)
        {
            StringBuilder sql = new StringBuilder();
           
            sql.Append(@"select * from Paper where Id in(
                        select PaperId from Author where Id in (
                        select AuthorId from Author_Person where PersonNo ={0} and status = {1}));");
            List<GetOnePaperDTO> papers = context.Database
                .SqlQuery<GetOnePaperDTO>(sql.ToString(), username,(int)AuthorPersonStatus.RIGHT )
                .ToList();

            return papers;
        }
        public IEnumerable<GetClaimDTO> GetClaimAuthors()
        {
            StringBuilder sql = new StringBuilder();
         
            sql.Append(@"select ap.Id as AuthorPersonId,a.NameEN as AuthorName,p.Id as Id,PaperName 
                        from Author a, Paper p,(select * from Author_Person where status = {0} ) ap 
                        where ap.AuthorId = a.Id and a.PaperId = p.Id;");
            List<GetClaimDTO> papers = context.Database
                .SqlQuery<GetClaimDTO>(sql.ToString(), (int)AuthorPersonStatus.NEEDCLAIM)
                .ToList();
            return papers;
        }

        public IEnumerable<GetConfirmDTO> GetConfirmPapers(string username)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(@"select p.Id as Id, p.PaperName as PaperName,ap.Id as AuthorPersonId 
                        from Paper p, Author a,( select * from Author_Person where PersonNo ={0} and status = 0 ) ap
                        where ap.AuthorId = a.Id and a.PaperId = p.Id;");

             var result = context.Database
                .SqlQuery<GetConfirmDTO>(sql.ToString(), username);
             if (result != null)
             {
                 return result.ToList<GetConfirmDTO>();
             }
             else 
             {
                 return new List<GetConfirmDTO>();
             }
        }

        public PaperDetailDTO GetPaperDetail( int paperId ) 
        {
            PaperDetailDTO paperDetail = context.Paper.Select(Mapper.Map<PaperDetailDTO>)
                .FirstOrDefault(p => p.Id == paperId);
            return paperDetail;
        }

        public void ConfirmAuthorPerson(ConfirmDTO dto)
        {
            List<Author_Person> aps = context.Author_Person
                .Where( ap=> dto.AuthorPersonId == ap.Id )
                .ToList();
            AuthorPersonStatus statu = dto.Belongs ? AuthorPersonStatus.RIGHT:AuthorPersonStatus.WRONG;
            Author_Person item = context.Author_Person.FirstOrDefault( ap=>ap.Id == dto.AuthorPersonId);

            if (dto.Belongs)
            {
                item.status = (int)AuthorPersonStatus.RIGHT;
            }
            else
            {
                item.status = (int)AuthorPersonStatus.WRONG;
                Author_Person nap = new Author_Person();
                nap.AuthorId = item.AuthorId;
                nap.Name = "未找到";
                nap.status = (int)AuthorPersonStatus.NEEDCLAIM;
                context.Author_Person.Add(nap);
            }

            context.SaveChanges();
        }

        public void ClaimAuthorPerson(ClaimDTO dto)
        {
            Person person = context.Person.FirstOrDefault(p => p.PersonNo == dto.Username);
            Author_Person item = context.Author_Person.FirstOrDefault( ap=>ap.Id == dto.AuthorPersonId );
            //如果为true，将论文分配给他
            if (dto.Claim)
            {
                if (item.status != (int)AuthorPersonStatus.CLAIM)
                {
                    return;
                }
                var authorPersons = context.Author_Person
                    .Where( ap=>ap.AuthorId == item.AuthorId )
                    .ToList();
                for( int i = 0; i <authorPersons.Count; i++ )
                {
                    var ap = authorPersons[i];
                    if (ap.status == (int)AuthorPersonStatus.CONFIRM)
                    {
                        ap.status = (int)AuthorPersonStatus.WRONG;
                    }
                    else if (ap.status == (int)AuthorPersonStatus.RIGHT)
                    {
                        ap.status = (int)AuthorPersonStatus.WRONG;
                    }
                    else if (ap.status == (int)AuthorPersonStatus.CLAIM)
                    {
                        ap.status = (int)AuthorPersonStatus.REJECT;
                    }
                    else if (ap.status == (int)AuthorPersonStatus.NEEDCLAIM)
                    {
                        authorPersons.Remove(ap);
                        i--;
                    }
                }
                item.Name = person.NameCN;
                item.PersonNo = person.PersonNo;
                item.status = (int)AuthorPersonStatus.RIGHT;
            }
            else
            {
                item.status = (int)AuthorPersonStatus.REJECT;
                context.SaveChanges();
                int count = context.Author_Person
                        .Where(cap => cap.AuthorId == item.AuthorId && cap.status == (int)AuthorPersonStatus.CLAIM)
                        .ToList()
                        .Count();
               
                if (count <= 0)
                {
                    Author_Person nap = new Author_Person();
                    nap.AuthorId = item.AuthorId;
                    nap.Name = "未找到";
                    nap.status = (int)AuthorPersonStatus.NEEDCLAIM;
                    context.Author_Person.Add(nap);
                }
            }
            context.SaveChanges();
        }

        private void updatePaperStatus( int paperId )
        {
            Paper paper = context.Paper.FirstOrDefault(p => p.Id == paperId);
            if (null == paper) return;
            var authors = context.Author.Where(a => a.PaperId == paper.Id).ToList();
            foreach (var author in authors)
            {
                author.Author_Person.Any( );
            }
        }
        public string AddUser(AddUserDTO dto)
        {
            if (null == dto) return "request is usless";
            var person = context.Person.FirstOrDefault(p => p.PersonNo == dto.Name);
            if (null == person)
                return "this man is not our school teacher";
            var user = context.User.FirstOrDefault(u => u.Name == dto.Name && u.Role == dto.Role);
            if (null != user)
                return "user has exist";

            if (!AcceptPassword(dto.Password))
                return "password contains digital alphabet and underline the length is 6-20";
            if (!(dto.Role == (int)UserRole.DEPTADMIN || dto.Role == (int)UserRole.COMMON))
            {
                return "the role is error";
            }
            var depart = context.Department.FirstOrDefault(d => d.Id == dto.DepartmentId);
            if (depart == null)
                return "can't find the department";
            User newUser = Mapper.Map<User>(dto);
            context.User.Add(newUser);
            context.SaveChanges();
            return "success";
        }

        public string DeleteUser(GetUserDTO dto)
        {
            var user = context.User.First(u => u.Name == dto.Name && u.Role == dto.Role);
            if (null != user)
            {
                context.User.Remove(user);
                context.SaveChanges();
                return "success";
            }
            return "can't find the user";
        }

        public string UpdateUser(UpdateUserDTO dto)
        {
            var user = context.User.First(u => u.Name == dto.Name && u.Role == dto.Role);
            if (null == user)
                return "can't find the user";

            if (!AcceptPassword(dto.Password))
                return "password contains digital alphabet and underline the length is 6-20";

            user.Password = dto.Password;
            context.SaveChanges();
            return "success";
        }
        private bool AcceptPassword(string password)
        {
            if (null == password) return false;
            Regex reg = new Regex(@"\b(\w){6,20}\b");
            return reg.IsMatch(password);
        }

    }
}
