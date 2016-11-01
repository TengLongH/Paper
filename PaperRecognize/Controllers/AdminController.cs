using PaperRecognize.DTOs;
using PaperRecognize.DTOs.PaperDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using PaperRecognize.Repository;
using PaperRecognize.DTOs.AndroidDTO;
using System.Text;
using System.IO;
using PaperRecognize.Import;
using PaperRecognize.Business;

namespace PaperRecognize.Controllers
{
    /// <summary>
    /// 管理员操作类
    /// </summary>
    public class AdminController : ApiController
    {
        private AdminRepository repository = new AdminRepository() ;

        /// <summary>
        /// 获取需要管理员处理的论文
        /// </summary>
        /// <param name="page">显示哪一页的条目</param>
        /// <returns>需要处理的论文列表</returns>
        [Route("api/admin/papers/get/{page}")]
        public HttpResponseMessage GetPapers( int page )
        {
            var papers = repository.GetPapers(page);
            return Utils.Util.toJson( papers );
        }
        /// <summary>
        /// 获取论文所有作者的候选人
        /// </summary>
        /// <param name="paperId">论文ID</param>
        /// <returns>返回一个作者列表，每一个作者对象里都有一个候选人列表</returns>
        [Route("api/admin/paperauthors/get/{paperId}")]
        public HttpResponseMessage GetCandidate( int paperId )
        {
            var candidates = repository.GetCandidate(paperId);
            return Utils.Util.toJson(candidates);
        }
        /// <summary>
        /// 用论文名，作者名，论文所属院系等搜索论文
        /// </summary>
        /// <param name="search">对象里有搜索论文的关键字</param>
        /// <returns>搜索到的所有论文列表</returns>
        [Route("api/admin/search")]
        public HttpResponseMessage PostSearchPaper( SearchPaperDTO search )
        {
            IEnumerable<GetOnePaperDTO> list = repository.SearchPaper( search );
            return Utils.Util.toJson(list);
        }

       
        /// <summary>
        /// 将上传的论文信息导入到数据库
        /// </summary>
        /// <returns>导入成功返回success,否则返回一条错误信息</returns>
        [Route("api/recognize/importpapers")]
        public HttpResponseMessage GetImportPapers()
        { 
            return Utils.Util.toJson( repository.ImportPapers() );
        }

      
        /// <summary>
        /// 搜索作者的候选人
        /// </summary>
        /// <returns>成功执行返回success，否则返回一条错误信息</returns>
        [Route("api/recognize/lookup")]
        public HttpResponseMessage GetLookupAuhor()
        {
            string response = DateTime.Now.ToString();
            LookupAuthor lookup = new LookupAuthor();
            lookup.LookupPapers();
            return Utils.Util.toJson("success");
        }
    }
}
