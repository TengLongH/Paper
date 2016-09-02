using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using AutoMapper;
using PaperRecognize.Models;
using PaperRecognize.DTOs;
using PaperRecognize.DTOs.PaperDTO;
using System.Data.SqlClient;

namespace PaperRecognize.Repository
{
    public class RecognizeRepository
    {
        private int PageSize = 10;
        private DBModel context = new DBModel();
        public IEnumerable<GetAuthorPersonDTO> UpdateAuthorPerson(UpdateAuthorPersonDTO update)
        {
            if (null == update) return null;
            if (null == update.AuthorId) return null;
            //更新PaperOwnerConfirm数据
            try
            {
                Author_Person ap = context.Author_Person.FirstOrDefault(c => c.Id == update.Id);
                if (null == ap) return null;

                if (ap.PersonNo != update.PersonNo )
                {
                    UpdatePersonNo( ap, update );
                }
                else if ( ap.Name != update.Name)
                {
                    UpdateName(ap, update);
                }
                else if (ap.status != (int)update.status)
                {
                    UpdateStatus( ap, update);
                }
                else
                {
                    throw new Exception("useless request");
                }
            }
            catch (Exception e)
            {
                String str = e.Message;
            }

            context.SaveChanges();

            return GetAuthorPersons((int)update.AuthorId);
        }
        private void UpdateStatus(Author_Person ap, UpdateAuthorPersonDTO update)
        {
            if (update.status != AuthorPersonStatus.RIGHT)
                throw new Exception("Author_Person status value is illegal");
           
            //将所有人的状态设置为错误
            int i = 0;
            ICollection<Author_Person> aps = ap.Author.Author_Person;
            while (i < aps.Count)
            {
                aps.ElementAt(i).status = (int)AuthorPersonStatus.WRONG;
                i++;
            }
            //将他的状态设置为正确
            ap.status = (int)AuthorPersonStatus.RIGHT;

            //如果论文的所有作者都通过验证，将论文的状态修改为通过
            List<Author> authors = context.Author
                .Where(a => a.PaperId == ap.Author.PaperId)
                .ToList();
            if (authors.All(a=>a.Author_Person.Any( apa=>apa.status==(int)AuthorPersonStatus.RIGHT)))
            {
                ap.Author.Paper.status = (int)PaperStatus.DEAL;
            }   
        }
        private void UpdateName(Author_Person ap, UpdateAuthorPersonDTO update)
        {
            //从数据库取出指定名字的所有人
            List<Person> persons = context.Person.Where(p => p.NameCN == update.Name).ToList();
            if (null == persons || persons.Count <= 0) return;

            //删除同一人的重复信息
            int j = 0, k = 0;
            while (j < persons.Count)
            {
                k = j + 1;
                while (k < persons.Count)
                {
                    if (persons.ElementAt(k).PersonNo == persons.ElementAt(j).PersonNo)
                    {
                        persons.RemoveAt(k);
                    }
                    else
                    {
                        k++;
                    }
                }
                j++;
            }

            //删除掉列表中英文名不匹配的项
            Author author = context.Author.FirstOrDefault( a=>a.Id == update.AuthorId );
            int i = 0;
            while (i < persons.Count)
            {
                if (!ValideEnglishName(persons[i], author))
                {
                    persons.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
           //将新找到的人添加进数据库
            foreach (Person p in persons)
            {
                Author_Person nap = Mapper.Map<Author_Person>(ap);
                ChangeAuthorPersonValue(nap, p.PersonNo, p.NameCN, AuthorPersonStatus.CONFIRM );
                context.Author_Person.Add(nap);
            }
            //将原来的记录从数据库删除
            context.Author_Person.Remove( ap );
        }

        private void UpdatePersonNo(Author_Person ap, UpdateAuthorPersonDTO update)
        {
            Person p = context.Person.FirstOrDefault( person=>person.PersonNo == update.PersonNo);
          
            if (null == p)
            {
                throw new Exception("PersonNo is not correct");
            }
            if( null == ap.Author )
            {
                 throw new Exception("AuthorId is not correct");
            }
            if (!ValideEnglishName( p, ap.Author ))
            {
                throw new Exception("expert English name is not match");
            }
            ChangeAuthorPersonValue( ap, p.PersonNo, p.NameCN, AuthorPersonStatus.CONFIRM );
        }
        private void ChangeAuthorPersonValue(Author_Person ap, string PresonNo, string  Name, AuthorPersonStatus status)
        {
            ap.PersonNo = PresonNo;
            ap.Name = Name;
            ap.status = (int)status;
        }
        private bool ValideEnglishName(Person p, Author author)
        {
            if (null == p.NameEN) return true;
            if (null == author.NameEN ) return true;
            if (p.NameEN.Equals(author.NameEN, StringComparison.OrdinalIgnoreCase)) 
                return true;
            if (null == p.NameENAbbr ) return true;
            StringBuilder abbreviation = new StringBuilder();
            for (int i = 0; i < author.NameEN.Length; i++)
            {
                int c = author.NameEN.ElementAt(i);
                if (c >= 'A' && c <= 'Z')
                {
                    abbreviation.Append((char)c);
                }
            }
            String abb = abbreviation.ToString();
            return p.NameENAbbr.StartsWith(abb, StringComparison.OrdinalIgnoreCase);
        }
        public IEnumerable<GetOnePaperDTO> GetPapers( int page )
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT * FROM Paper w1,( ");
            sqlBuilder.Append("SELECT TOP "+ PageSize +" Id FROM ( ");
            sqlBuilder.Append("SELECT TOP "+ page * PageSize+" Id FROM Paper WHERE status = "+ (int)PaperStatus.ANALISIS +" ORDER BY Id DESC) ");
            sqlBuilder.Append("w ORDER BY w.Id ASC) ");
            sqlBuilder.Append("w2 WHERE w1.Id = w2.Id ORDER BY w1.Id DESC ");
        
            List<Paper> papers = null;
            papers = context.Paper.SqlQuery(sqlBuilder.ToString())
                 .ToList();

            List<GetOnePaperDTO> dtos = new List<GetOnePaperDTO>();
            foreach (var paper in papers)
            {
                dtos.Add( Mapper.Map<GetOnePaperDTO>( paper ) );
            }
            return dtos;

        }
        public GetOnePaperDTO GetOnePaper(int paperId)
        {
            Paper paper = context.Paper.FirstOrDefault(p => p.Id == paperId);
            if (null == paper) return null;
            GetOnePaperDTO dto = Mapper.Map<GetOnePaperDTO>(paper);
            dto.Authors.AddRange(GetAuthors(paperId));
            return dto;
        }
        public IEnumerable<GetOneAuthorDTO> GetAuthors(int paperId)
        {
            List<GetOneAuthorDTO> authorDTOs = context.Author
                .Where(a => a.PaperId == paperId)
                .Select(Mapper.Map<GetOneAuthorDTO>)
                .ToList();
            if( null == authorDTOs ) return new List<GetOneAuthorDTO>();
            foreach (GetOneAuthorDTO dto in authorDTOs)
            {
                dto.AuthorPersons.AddRange( GetAuthorPersons(dto.Id));
            }
            return authorDTOs;
        }
        public IEnumerable<GetAuthorPersonDTO> GetAuthorPersons(int AuthorId)
        {
            List<GetAuthorPersonDTO> list = context.Author_Person
                .Where(ap => ap.AuthorId == AuthorId)
                .Select( Mapper.Map<GetAuthorPersonDTO>)
                .ToList();
            return list;
        }

    }
}