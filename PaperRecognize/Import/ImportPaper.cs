
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using PaperRecognize.Models;
using PaperRecognize.DTOs;

namespace PaperRecognize.Import
{
    public static class ImportPaper
    {
        private static DateTime? ParseDate(string sDate, string sYear)
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
        public static int ExcelToSQLServer(string file, string sheetName, bool isFirstRowColumn)
        {
            ExcelHelper excel = new ExcelHelper();
            DataTable data = excel.ExcelToDataTable(file, sheetName, isFirstRowColumn); //读取excel中paper信息

            int result = 0;
            using (var db = new DBModel())   //创建对象上下文
            {
                DataRow dr = null;

                for(int i = 0; i < data.Rows.Count; i++)
                {
                    dr = data.Rows[i];

                    //创建新的Paper对象
                    var paper = new Paper
                    {
                        PressType = dr[excel.PressType].ToString().Trim(),
                        AuthorsShort = dr[excel.AuthorsShort].ToString().Trim(),
                        //AuthorsFull = dr[excel.AuthorName].ToString(),
                        //FirstAuthorSignUnit = dr[excel.FirstDept].ToString(),
                        AuthorsFull = dr[excel.Authors].ToString().Trim(),
                        PaperName = dr[excel.PaperName].ToString().Trim(),
                        JournalName = dr[excel.JournalName].ToString().Trim(),
                        Series = dr[excel.Series].ToString().Trim(),
                        Language = dr[excel.Language].ToString().Trim(),
                        PaperType = dr[excel.PaperType].ToString().Trim(),
                        AuthorKeyWord = dr[excel.AuthorKeyWord].ToString().Trim(),
                        KeyWords = dr[excel.KeyWords].ToString().Trim(),
                        Abstract = dr[excel.Abstract].ToString().Trim(),
                        AuthorsAddress = dr[excel.Address].ToString().Trim(),
                        CorrespondenceEN = dr[excel.ConnAuthor].ToString().Trim(),
                        EmailAddress = dr[excel.EmailAddess].ToString().Trim(),
                        Reference = dr[excel.CitedReference].ToString().Trim(),

                        Press = dr[excel.Press].ToString().Trim(),
                        City = dr[excel.City].ToString().Trim(),
                        PressAddress = dr[excel.PressAdress].ToString().Trim(),
                        ISSN = dr[excel.ISSN].ToString().Trim(),
                        DI = dr[excel.DI].ToString().Trim(),
                        StandardJournalAbbr = dr[excel.StandardJournalShort].ToString().Trim(),
                        ISIJournalAbbr = dr[excel.ISIJournalShort].ToString().Trim(),

                        Issue = dr[excel.Issue].ToString().Trim(),   //期
                        PartNumber = dr[excel.PartNumber].ToString().Trim(),

                        SpecialIssue = dr[excel.SpecialIssue].ToString().Trim(),

                        ArticleNumber = dr[excel.ArticleNumber].ToString().Trim(),
                        SubjectCategory = dr[excel.SubjectCategory].ToString().Trim(),
                        IncludeType = dr[excel.Citations].ToString().Trim(),
                        ISIDeliveryNo = dr[excel.ISIDeliveryNo].ToString().Trim(),
                        ISIArticleIdentifier = dr[excel.ISIArticleIdentifier].ToString().Trim(),
                        status = (int)PaperStatus.ANALISIS,
                    };
                    //参考文献数
                    string rCount = dr[excel.CitedReferenceCount].ToString().Trim();
                    if (!string.IsNullOrEmpty(rCount))
                    {
                        paper.ReferenceCount = int.Parse(rCount);
                    }
                    //被引频次
                    string citedCount = dr[excel.TotalTimesCited].ToString().Trim();
                    if (!string.IsNullOrEmpty(citedCount))
                    {
                        paper.CitedCount = int.Parse(citedCount);
                    }
                    //发表日期、发表年
                    string sDate = dr[excel.PublishDate].ToString().Trim();
                    string sYear = dr[excel.PublishYear].ToString().Trim();
                    try
                    {
                        paper.PublishDate = ParseDate(sDate, sYear);
                        if (!string.IsNullOrEmpty(sYear))
                        {
                            paper.Year = int.Parse(sYear);
                        }
                    }
                    catch(Exception e)
                    {
                        throw;
                    }
                    //卷
                    string volume = dr[excel.Volume].ToString().Trim();
                    if (!string.IsNullOrEmpty(volume))
                    {
                        paper.Volume = int.Parse(volume);
                    }
                    
                    string supplement = dr[excel.Supplement].ToString().Trim();  //是否增刊
                    if (!string.IsNullOrEmpty(supplement))
                    {
                        paper.Supplement = bool.Parse(supplement);
                    }
                    //起始页码、结束页码、页数
                    Regex re = new Regex(@"^\d+$"); //匹配数字字符串
                    string start = dr[excel.StartPage].ToString().Trim();
                    if(!string.IsNullOrWhiteSpace(start) && re.IsMatch(start))
                    {
                        paper.StartPage = int.Parse(start);
                    }
                    string end = dr[excel.EndPage].ToString().Trim();
                    if(!string.IsNullOrWhiteSpace(end) && re.IsMatch(end))
                    {
                        paper.EndPage = int.Parse(end);
                    }
                    string pcount = dr[excel.PageCount].ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(pcount) && re.IsMatch(pcount))
                    {
                        paper.PageCount = int.Parse(pcount);
                    }

                    db.Paper.Add(paper);    //将Paper对象添加到对象上下文中
                }
                try
                { 
                    result = db.SaveChanges();  //保存到数据库
                }
                catch(DbEntityValidationException ex)
                {
                    foreach (var entityValidationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {
                            Console.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                        }
                    }
                    throw;
                }
            }

            return result;
        }
    }
}