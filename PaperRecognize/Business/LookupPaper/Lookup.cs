using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaperRecognize.Models;
using System.Data.SqlClient;
using PaperRecognize.DTOs.PaperDTO;

namespace PaperRecognize.Business.LookupPaper
{
    /// <summary>
    /// 使用一些关键字如论文名，作者名，院系等搜索论文的类
    /// </summary>
    public class Lookup
    {
        private static string[] paperNames;
        private static string[] authorNames;
        private static string[] departs;
        private static SearchHelper helper;
        private static DBModel context;
        /// <summary>
        /// 静态构造函数，初始化各个参数
        /// </summary>
        static Lookup(){

            helper = new SearchHelper();

            context = new DBModel();

            string paperSql = "select PaperName from Paper";
            var nameClasses = context.Database
                .SqlQuery<PaperNameClass>( paperSql )
                .ToArray();
            paperNames = new string[nameClasses.Length];
            for (int i = 0; i < nameClasses.Length; i++ )
            {
                paperNames[i] = nameClasses[i].PaperName;
            }
        }
        /// <summary>
        /// 用论文名搜索论文
        /// </summary>
        /// <param name="name">论文名</param>
        /// <returns>搜索到的论文列表</returns>
        public static List<GetOnePaperDTO> LookupPaperByName(string name)
        {
            string[] result = helper.Search( name, paperNames, 10, (float)0.5 );
            var papers = context.Paper
                .Where(p => result.Contains(p.PaperName))
                .Select( AutoMapper.Mapper.Map<GetOnePaperDTO>)
                .ToList();
            return papers;
        }

    }

    class PaperNameClass
    {
        public string PaperName { get; set; }
    }
}