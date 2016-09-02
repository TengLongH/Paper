
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using PaperRecognize.Models;
using PaperRecognize.ParseName;
using PaperRecognize.DTOs;

namespace PaperRecognize.Business
{
    class Pretreatment
    {
       // private static readonly log4net.ILog Log = PaperRecognize.Log.LogHelper.GetLogger();
        private static string OUC = "中国海洋大学";
        private static List<Department> depList = null;
        private static List<DepartmentAlias> aliasList = null;
        private static List<DepartmentAlias> oucAlias = null;
        private List<Paper> pList = null;

        public Pretreatment()
        {
            using (var db = new DBModel())
            {
                depList = db.Department.ToList();
                aliasList = db.DepartmentAlias.ToList();
                pList = db.Paper.Where(p => p.status == (int)PaperStatus.ANALISIS).ToList();
            }

            int OUCId = depList.Where(e => e.Name == OUC).First().Id;
            oucAlias = aliasList.Where(e => e.DepartmentId == OUCId).ToList();

            aliasList = aliasList.Except(oucAlias).ToList();
        }

        /// <summary>
        /// 对论文作者等信息进行预处理
        /// </summary>
        public void pretreatPaper()
        {
            List<Author> aList = new List<Author>();

            foreach (var paper in pList)
            {
                aList.AddRange(getAuthors(paper));
            }

            using (var db = new DBModel())
            {
                db.Author.AddRange(aList);

                db.SaveChanges();
            }
        }
        /// <summary>
        /// 获得每篇论文的作者相关信息，包括英文名字、院系、是否通信作者等信息
        /// </summary>
        /// <param name="paper">一篇论文</param>
        /// <returns>作者列表</returns>
        public List<Author> getAuthors(Paper paper)
        {
            string[] authorAbbr = paper.AuthorsShort.Split(new char[] { ';' }); //作者名字简拼数组
            string[] authorFull = paper.AuthorsFull.Replace(",", "").Replace("-", "").Replace(".", "").Replace(" ", "").Split(';'); //作者名字全拼数组
            string[] authorAry = extractAuthorName(paper.AuthorsFull, paper.AuthorsAddress);    //从作者地址中提取出的作者数组
            string[] addrLine = paper.AuthorsAddress.Split(']');    //作者地址中的每条地址信息
            string connAuthor = ""; //通行作者英文名字
            int userOrdinal = 0;    //作者在作者地址列表中所署名的本校位次
            List<Author> authorList = new List<Author>();
            Author author = null;
            List<string> userDept = null;   //每个作者的单位List
            List<Name> names = new List<Name>();
            Name name = null;
            //获得通信作者的名字
            if (!string.IsNullOrWhiteSpace(paper.CorrespondenceEN))
            {
                connAuthor = paper.CorrespondenceEN.Split(new char[] { '(' })[0].Replace(" ", "").ToLower();
            }

            try
            {
                //解析作者名字，检测是否是简拼、倒序
                ParseName.ParseName parse = new ParseName.ParseName();
                parse.parseName(paper.AuthorsFull, names);

            }
            catch (Exception e)
            {
               // Log.Warn(e.Message);
            }

            //对每个作者进行预处理，生成model
            for (int i = 0; i < authorFull.Count(); i++)
            {
                name = names[i];
                userOrdinal = getSchoolOrdinal(authorFull[i], authorAry, addrLine);
                userDept = getDepartment(authorFull[i], authorAry, addrLine);

                if (paper.PaperDepartmentId == null && userDept.Count > 0)
                {
                    paper.PaperDepartmentId = getPaperDepartmentId(userDept);
                }

                author = new Author
                {
                    PaperId = paper.Id,
                    Ordinal = i + 1,
                    Department = string.Join(";", userDept),
                    IsCorrespondingAuthor = authorAbbr[i].Replace(" ", "").ToLower() == connAuthor,
                    PublishDate = paper.PublishDate,
                    IsOtherUnit = userOrdinal > 0 ? false : true,
                    SignOrdinal = userOrdinal,
                };

                if (name.IsSecondAbbr)
                {
                    author.NameENAbbr = name.FirstName + ", " + name.SecondName;
                }
                else
                {
                    author.NameEN = name.FirstName + name.SecondName;
                }

                authorList.Add(author);
            }

            return authorList;
        }

        /// <summary>
        /// 从【作者地址】中提取作者
        /// </summary>
        /// <param name="address">一条作者地址</param>
        /// <returns>提取出的作者list</returns>
        private string[] extractAuthorName(string authors, string address)
        {
            //匹配成对圆括号中的内容
            string pattern = @"(?<=\()((?<gp>\()|(?<-gp>\))|[^()]+)*(?(gp)(?!))";

            address = address.Replace("[", "(").Replace("]", ")");
            string[] ary = Regex.Matches(address, pattern).Cast<Match>().Select(t => t.Value.Replace(",", "").Replace("-", "").Replace(".", "").Replace(" ", "")).ToArray();

            if (ary.Count() == 0)   //作者地址没写作者
            {
                ary = new string[1] { authors.Replace(",", "").Replace("-", "").Replace(".", "").Replace(" ", "") };
            }

            return ary;
        }

        /// <summary>
        /// 判断给出的单位地址是否是本单位
        /// </summary>
        /// <param name="addrLine">一条地址</param>
        /// <returns>true or false</returns>
        private bool isOurSchool(string addrLine)
        {
            foreach (var name in oucAlias)
            {
                if (addrLine.Contains(name.Alias.Replace(" ", "")))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取该条地址包含的院系、实验室的别名
        /// </summary>
        /// <param name="addrLine">一条地址</param>
        /// <returns>院系别名的列表</returns>
        private List<string> getContainsAlias(string addrLine)
        {
            List<string> list = new List<string>();
            HashSet<int> deptId = new HashSet<int>();
            foreach (var alias in aliasList)
            {
                if (addrLine.Contains(alias.Alias.Replace(" ", "")))
                {
                    if (!deptId.Contains(alias.DepartmentId))
                    {
                        list.Add(alias.Alias);
                        deptId.Add(alias.DepartmentId);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// 获取某位作者在作者地址列表中所署名的本校位次
        /// </summary>
        /// <param name="name">一个作者英文名字</param>
        /// <param name="authors">从作者地址中提取出的作者list</param>
        /// <param name="address">地址list</param>
        /// <returns>0：外单位 >0：本校署名位次</returns>
        private int getSchoolOrdinal(string name, string[] authors, string[] address)
        {
            int index = 0;

            //去提取出的作者数组中进行匹配
            for (int i = 0; i < authors.Count(); i++)
            {
                string[] nameSpell = authors[i].Split(';');
                if (nameSpell.Where(s => s == name).Count() > 0)  //该行匹配成功
                {
                    index++;
                    //找到该作者的学校或单位
                    string addr = address[i + 1 >= address.Count() ? i : i + 1].Replace(" ", "").ToLower();
                    if (isOurSchool(addr))
                    {
                        return index;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取某个作者在地址列表中署名的院系、实验室的英文名字
        /// </summary>
        /// <param name="name">作者英文名字</param>
        /// <param name="authors">从作者地址中提取出的作者list</param>
        /// <param name="address">地址list</param>
        /// <returns>院系list</returns>
        private List<string> getDepartment(string name, string[] authors, string[] address)
        {
            List<string> dept = new List<string>();
            List<string> list = null;

            //去提取出的作者数组中进行匹配
            for (int i = 0; i < authors.Count(); i++)
            {
                string[] nameSpell = authors[i].Split(';');
                if (nameSpell.Where(s => s == name).Count() > 0)  //该行匹配成功
                {
                    //找到该作者的学校或单位
                    string addr = address[i + 1 >= address.Count() ? i : i + 1].Replace(" ", "").ToLower();
                    if (isOurSchool(addr))
                    {
                        list = getContainsAlias(addr);
                        if (list.Count() > 0)
                        {
                            dept.AddRange(list);
                        }
                    }
                }
            }

            //Lambda表达式去重，保留重复元素的第一个
            dept = dept.Where((d, i) => dept.FindIndex(p => p == d) == i).ToList();

            return dept;
        }

        /// <summary>
        /// 获得论文所属学院
        /// </summary>
        /// <param name="depts">院系英文名字list</param>
        /// <returns>学院ID</returns>
        public int getPaperDepartmentId(List<string> depts)
        {
            DepartmentAlias dAlias = null;
            Department department = null;

            foreach (string dept in depts)
            {
                dAlias = aliasList.Where(e => e.Alias == dept).First();
                department = depList.Where(e => e.Id == dAlias.DepartmentId).First();

                while (department.Type != (int)DepartmentType.COLLEGE && department.ParentId != null)
                {
                    department = depList.Where(e => e.Id == department.ParentId).First();
                }

                if (department.Type == (int)DepartmentType.COLLEGE)
                {
                    return department.Id;
                }
            }

            return department.Id;
        }
    }
}
