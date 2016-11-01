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
        protected DBModel context = new DBModel();
        /*
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
                else if ( ap.Name != update.NameCN)
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
        public void UpdateStatus(Author_Person ap, UpdateAuthorPersonDTO update)
        {
            bool authorPass = false;
            CandidateStatus status = (CandidateStatus)ap.status;
            if (status == CandidateStatus.NEEDCLAIM)
            {
                //认领
                if (update.status == CandidateStatus.CLAIM)
                {
                    ap.status = (int)update.status;
                    ap.Name = update.NameEN;
                    ap.PersonNo = update.PersonNo;
                }
            }
            else if (status == CandidateStatus.CONFIRM)
            {
                //分配
                if (update.status == CandidateStatus.RIGHT)
                {
                    ap.status = (int)CandidateStatus.RIGHT;
                    authorPass = true;
                }
                //否决
                if (update.status == CandidateStatus.WRONG)
                {
                    ap.status = (int)CandidateStatus.WRONG;
                    Author_Person nap = new Author_Person();
                    nap.AuthorId = ap.AuthorId;
                    nap.Name = "未找到";
                    nap.status = (int)CandidateStatus.NEEDCLAIM;
                    context.Author_Person.Add(nap);
                }

            }
            else if (status == CandidateStatus.RIGHT)
            {
                //撤销
                if (update.status == CandidateStatus.WRONG)
                {
                    ap.status = (int)CandidateStatus.WRONG;
                    Author_Person nap = new Author_Person();
                    nap.AuthorId = ap.AuthorId;
                    nap.Name = "未找到";
                    nap.status = (int)CandidateStatus.NEEDCLAIM;
                    context.Author_Person.Add(nap);
                }
            }
            else if (status == CandidateStatus.WRONG)
            {
                //强制分配
                if (update.status == CandidateStatus.RIGHT)
                {
                    var list = ap.Author.Author_Person;
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list.ElementAt(i);
                        if (item.status == (int)CandidateStatus.RIGHT)
                        {
                            item.status = (int)CandidateStatus.WRONG;
                        }
                        else if (item.status == (int)CandidateStatus.NEEDCLAIM)
                        {
                            list.Remove(item);
                            i--;
                        }
                        else if (item.status == (int)CandidateStatus.CLAIM)
                        {
                            item.status = (int)CandidateStatus.REJECT;
                        }
                    }

                    ap.status = (int)CandidateStatus.RIGHT;
                    authorPass = true;
                }
            }
            else if (status == CandidateStatus.CLAIM)
            {
                //否认
                if (update.status == CandidateStatus.REJECT)
                {
                    ap.status = (int)CandidateStatus.REJECT;
                    var list = ap.Author.Author_Person;
                    if (list.All(item => { return item.status != (int)CandidateStatus.CLAIM; }))
                    {
                        Author_Person nap = new Author_Person();
                        nap.AuthorId = ap.AuthorId;
                        nap.Name = "未找到";
                        nap.status = (int)CandidateStatus.NEEDCLAIM;
                        context.Author_Person.Add(nap);
                    }
                }
                //认领分配
                else if (update.status == CandidateStatus.RIGHT)
                {
                    var list = ap.Author.Author_Person;
                    foreach (var item in list)
                    {
                        if (item.status == (int)CandidateStatus.CLAIM)
                        {
                            item.status = (int)CandidateStatus.REJECT;
                        }
                    }
                    ap.status = (int)CandidateStatus.RIGHT;
                    authorPass = true;
                }
            }
            else if (status == CandidateStatus.REJECT)
            {
                //强制认领分配
                if (update.status == CandidateStatus.RIGHT)
                {
                    var list = ap.Author.Author_Person;
                    for ( int i = 0; i < list.Count; i++ )
                    {
                        if (list.ElementAt(i).status == (int)CandidateStatus.CLAIM)
                        {
                            list.ElementAt(i).status = (int)CandidateStatus.REJECT;
                        }
                        else if (list.ElementAt(i).status == (int)CandidateStatus.NEEDCLAIM)
                        {
                            list.Remove(list.ElementAt(i));
                            i--;
                        }
                        
                    }
                    ap.status = (int)CandidateStatus.RIGHT;
                    authorPass = true;
                } 
            }

            context.SaveChanges();
            //如果论文的所有作者都通过验证，将论文的状态修改为通过
            if( authorPass)
            {
                List<Author> authors = context.Author
               .Where(a => a.PaperId == ap.Author.PaperId)
               .ToList();
                if (authors.All(a => a.Author_Person.Any(apa => apa.status == (int)CandidateStatus.RIGHT)))
                {
                    ap.Author.Paper.status = (int)PaperStatus.DEAL;
                }
            }
            
        }
        private void UpdateName(Author_Person ap, UpdateAuthorPersonDTO update)
        {
            //从数据库取出指定名字的所有人
            List<Person> persons = context.Person.Where(p => p.NameCN == update.NameCN).ToList();
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
                ChangeAuthorPersonValue(nap, p.PersonNo, p.NameCN, CandidateStatus.CLAIM );
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
            ChangeAuthorPersonValue( ap, p.PersonNo, p.NameCN, CandidateStatus.CONFIRM );
        }
        private void ChangeAuthorPersonValue(Author_Person ap, string PresonNo, string  Name, CandidateStatus status)
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
     
        public IEnumerable<GetAuthorPersonDTO> GetAuthorPersons(int AuthorId)
        {
            List<GetAuthorPersonDTO> list = context.Author_Person
                .Where(ap => ap.AuthorId == AuthorId)
                .Select( Mapper.Map<GetAuthorPersonDTO>)
                .ToList();
            return list;
        }
        */
    }
}