
using PaperRecognize.Business;
using PaperRecognize.DTOs;
using PaperRecognize.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PaperRecognize.Import
{
    public enum PaperExcel
    {
        PressType = 0,
        AuthorsShort = 1,
        AuthorName = 2,
        FirstDept = 3,
        OUCOrder = 4,
        College = 5,
        Lab = 6,
        Authors = 7,

        PaperName = 8,
        JournalName = 9,
        Series = 10,
        Language = 11,
        PaperType = 12,
        AuthorKeyWord = 13,
        KeyWords = 14,
        Abstract = 15,
        Address = 16,
        ConnAuthor = 17,
        ConnAuthorName = 18,
        ConnAuthorDept = 19,

        EmailAddess = 20,
        CitedReference = 21,
        CitedReferenceCount = 22,
        TotalTimesCited = 23,
        Press = 24,
        City = 25,
        PressAddress = 26,
        ISSN = 27,
        DI = 28,
        StandardJournalShort = 29,
        ISIJournalShort = 30,
        PublishDate = 31,
        PublishYear = 32,
        Volume = 33,
        Issue = 34,
        PartNumber = 35,
        Supplement = 36,
        SpecialIssue = 37,

        StartPage = 38,
        EndPage = 39,
        ArticleNumber = 40,
        PageCount = 41,
        SubjectCategory = 42,
        Citations = 43,
        ISIDeliveryNo = 44,
        ISIArticleIdentifier = 45,
    }

    public class ImportPaper
    {
        //private static readonly log4net.ILog Log = PaperRecognize.Log.LogHelper.GetLogger();
        private DateTime? ParseDate(string sDate, string sYear)
        {
            DateTime? PublishDate = null;
            string[] expectedFormats = { "yyyy/M/d H:mm:ss", "yyyy", "yyyy/M/d" };
            string[] months = { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            Regex re = new Regex(@"^([A-Z][-]?)+$"); //匹配字母字符串

            if (sDate != "" && re.IsMatch(sDate) && sYear != "")    //发表日期为月份英文简写
            {
                PublishDate = DateTime.ParseExact(sYear, expectedFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                for (int m = 0; m < 12; m++)
                {
                    if (sDate.Contains(months[m]))
                    {
                        PublishDate = PublishDate.Value.AddMonths(m);
                        break;
                    }
                }
            }
            if (PublishDate == null && sDate != "" && sYear != "")  //发表日期为yyyy/m/d这种形式
            {
                DateTime dt = DateTime.ParseExact(sDate, expectedFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                PublishDate = new DateTime(int.Parse(sYear), dt.Month, dt.Day);
            }
            if (PublishDate == null && sYear != "") //只有发表年份
            {
                PublishDate = DateTime.ParseExact(sYear, expectedFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
            }

            return PublishDate;
        }

        private Paper getPaper(DataRow dr, ExcelHelper excel)
        {
            //创建新的Paper对象
            var paper = new Paper
            {
                PressType = dr[(int)PaperExcel.PressType].ToString().Trim(),
                AuthorsShort = dr[(int)PaperExcel.AuthorsShort].ToString().Trim(),
                //AuthorsFull = dr[(int)PaperExcel.AuthorName].ToString(),
                //FirstAuthorSignUnit = dr[(int)PaperExcel.FirstDept].ToString(),
                AuthorsFull = dr[(int)PaperExcel.Authors].ToString().Trim(),
                PaperName = dr[(int)PaperExcel.PaperName].ToString().Trim(),
                JournalName = dr[(int)PaperExcel.JournalName].ToString().Trim(),
                Series = dr[(int)PaperExcel.Series].ToString().Trim(),
                Language = dr[(int)PaperExcel.Language].ToString().Trim(),
                PaperType = dr[(int)PaperExcel.PaperType].ToString().Trim(),
                AuthorKeyWord = dr[(int)PaperExcel.AuthorKeyWord].ToString().Trim(),
                KeyWords = dr[(int)PaperExcel.KeyWords].ToString().Trim(),
                Abstract = dr[(int)PaperExcel.Abstract].ToString().Trim(),
                AuthorsAddress = dr[(int)PaperExcel.Address].ToString().Trim(),
                CorrespondenceEN = dr[(int)PaperExcel.ConnAuthor].ToString().Trim(),
                EmailAddress = dr[(int)PaperExcel.EmailAddess].ToString().Trim(),
                Reference = dr[(int)PaperExcel.CitedReference].ToString().Trim(),

                Press = dr[(int)PaperExcel.Press].ToString().Trim(),
                City = dr[(int)PaperExcel.City].ToString().Trim(),
                PressAddress = dr[(int)PaperExcel.PressAddress].ToString().Trim(),
                ISSN = dr[(int)PaperExcel.ISSN].ToString().Trim(),
                DI = dr[(int)PaperExcel.DI].ToString().Trim(),
                StandardJournalAbbr = dr[(int)PaperExcel.StandardJournalShort].ToString().Trim(),
                ISIJournalAbbr = dr[(int)PaperExcel.ISIJournalShort].ToString().Trim(),

                Volume = dr[(int)PaperExcel.Volume].ToString().Trim(),
                Issue = dr[(int)PaperExcel.Issue].ToString().Trim(),   //期
                PartNumber = dr[(int)PaperExcel.PartNumber].ToString().Trim(),

                SpecialIssue = dr[(int)PaperExcel.SpecialIssue].ToString().Trim(),

                ArticleNumber = dr[(int)PaperExcel.ArticleNumber].ToString().Trim(),
                SubjectCategory = dr[(int)PaperExcel.SubjectCategory].ToString().Trim(),
                IncludeType = dr[(int)PaperExcel.Citations].ToString().Trim(),
                ISIDeliveryNo = dr[(int)PaperExcel.ISIDeliveryNo].ToString().Trim(),
                ISIArticleIdentifier = dr[(int)PaperExcel.ISIArticleIdentifier].ToString().Trim(),
                status = (int)PaperStatus.PRETREATMENT,
            };
            //参考文献数
            string rCount = dr[(int)PaperExcel.CitedReferenceCount].ToString().Trim();
            if (!string.IsNullOrEmpty(rCount))
            {
                paper.ReferenceCount = int.Parse(rCount);
            }
            //被引频次
            string citedCount = dr[(int)PaperExcel.TotalTimesCited].ToString().Trim();
            if (!string.IsNullOrEmpty(citedCount))
            {
                paper.CitedCount = int.Parse(citedCount);
            }
            //发表日期、发表年
            string sDate = dr[(int)PaperExcel.PublishDate].ToString().Trim();
            string sYear = dr[(int)PaperExcel.PublishYear].ToString().Trim();
            try
            {
                paper.PublishDate = ParseDate(sDate, sYear);
                if (!string.IsNullOrEmpty(sYear))
                {
                    paper.Year = int.Parse(sYear);
                }
            }
            catch (Exception e)
            {
                throw new Exception("parse publish date error");
            }
            //卷
            /* string volume = dr[(int)PaperExcel.Volume].ToString().Trim();
             if (!string.IsNullOrEmpty(volume))
             {
                 paper.Volume = int.Parse(volume);
             }
             */
            string supplement = dr[(int)PaperExcel.Supplement].ToString().Trim();  //是否增刊
            if (!string.IsNullOrEmpty(supplement))
            {
                //paper.Supplement = bool.Parse(supplement);
                paper.Supplement = true;
            }
            //起始页码、结束页码、页数
            Regex re = new Regex(@"^\d+$"); //匹配数字字符串
            string start = dr[(int)PaperExcel.StartPage].ToString().Trim();
            if (!string.IsNullOrWhiteSpace(start) && re.IsMatch(start))
            {
                paper.StartPage = int.Parse(start);
            }
            string end = dr[(int)PaperExcel.EndPage].ToString().Trim();
            if (!string.IsNullOrWhiteSpace(end) && re.IsMatch(end))
            {
                paper.EndPage = int.Parse(end);
            }
            string pcount = dr[(int)PaperExcel.PageCount].ToString().Trim();
            if (!string.IsNullOrWhiteSpace(pcount) && re.IsMatch(pcount))
            {
                paper.PageCount = int.Parse(pcount);
            }

            return paper;
        }
        /// <summary>
        /// 将Excel文件里的论文信息导入到数据库里
        /// </summary>
        /// <param name="file">excel文件名</param>
        /// <param name="sheetName">包含论文信息的表名</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>成功导入的论文信息数目</returns>
        public int ExcelToSQLServer(string file, string sheetName, bool isFirstRowColumn)
        {
            ExcelHelper excel = new ExcelHelper();
            DataTable data = excel.ExcelToDataTable(file, sheetName, isFirstRowColumn); //读取excel中paper信息

            int result = 0;
            List<Paper> pList = new List<Paper>();
            DataRow dr = null;

            for (int i = 0; i < data.Rows.Count; i++)
            {

                try
                {
                    dr = data.Rows[i];
                    var paper = getPaper(dr, excel);

                    pList.Add(paper);
                }
                catch (Exception ex)
                {
                    //Log.Error(ex.Message + ex.StackTrace);
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }
            }

            using (var db = new DBModel())   //创建对象上下文
            {
             
                try
                {
                    db.Paper.AddRange(pList);
                    result = db.SaveChanges();  //保存到数据库
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }
    }
}