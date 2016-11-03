using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaperRecognize.Repository;
using PaperRecognize.DTOs;
using PaperRecognize.DTOs.UserDTO;
using PaperRecognize.DTOs.PersonDTO;
using PaperRecognize.DTOs.PaperDTO;
using System.Web;
using PaperRecognize.DTOs.Author;
using PaperRecognize.DTOs.AndroidDTO;
using PaperRecognize.Utils;
namespace PaperRecognize.Controllers
{
    /// <summary>
    /// 用户操作类
    /// </summary>
    public class UserController : ApiController
    {

        private UserRepository repository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserController()
        {
            repository = new UserRepository();
        }
        /// <summary>
        /// 获取分配给自己的论文
        /// </summary>
        /// <returns>分配给该用户的论文列表</returns>
        [Route("api/user/mypaper/get")]
        public HttpResponseMessage GetMyPaper()
        {

            object username = HttpContext.Current.Session["username"];
            IEnumerable<GetOnePaperDTO> list = repository.GetMyPaper(username.ToString());
            return Util.toJson( list );
        }
        /// <summary>
        /// 获取系统推荐的论文信息
        /// </summary>
        /// <returns>接收到的论文推荐信息列表</returns>
        [Route("api/user/system/get")]
        public HttpResponseMessage GetSystemPushAuthors()
        {

            object username = HttpContext.Current.Session["username"];
            return Util.toJson(repository.GetSystemPushAuthor(username.ToString()));
        }
        /// <summary>
        /// 获取认领平台的论文列表
        /// </summary>
        /// <returns>所有放在认领平台的论文列表</returns>
        [Route("api/user/claim/get/")]
        public HttpResponseMessage GetClaimAuthors()
        {
            return Util.toJson(repository.GetNoneCandidateAuthor());
        }

        /// <summary>
        /// 获取一篇论文的详细内容
        /// </summary>
        /// <param name="paperId">论文的ID</param>
        /// <returns>论文的详细信息</returns>
        [Route("api/user/paper/detail/{paperId}")]
        public HttpResponseMessage GetPaperDetail(int paperId)
        {

            return Util.toJson(repository.GetPaperDetail(paperId));
        }
        /// <summary>
        /// 获取自己从认领平台认领下的论文
        /// </summary>
        /// <returns>该用户认领的论文列表</returns>
        [Route("api/user/myclaim/get")]
        public HttpResponseMessage GetMyClaimAuthor()
        {
            var name = HttpContext.Current.Session["username"];
            var list = repository.GetMyClaimAuthor(name.ToString());
            return Util.toJson(list);
        }

        /// <summary>
        /// 处理系统推荐的论文
        /// </summary>
        /// <param name="dto">它有3个属性：username用户名，CandidateId系统推荐信息的ID，Belong用户接受为true否则为false</param>
        /// <returns>处理成功返回success</returns>
        [Route("api/user/confirm/post")]
        public HttpResponseMessage PostConfirmPaper( ConfirmDTO dto)
        {

            dto.Username = HttpContext.Current.Session["username"].ToString();
            repository.ConfirmSystemPushAuthor( dto );
            return new HttpResponseMessage() { Content = new StringContent("success") };
        }
        
        /// <summary>
        /// 从认领平台认领论文
        /// </summary>
        /// <param name="dto">它有两个属性：username用户名，AuthorId用户要认领下的作者ID</param>
        /// <returns>认领成功返回success</returns>
        [Route("api/user/claim/post")]
        public HttpResponseMessage PostClaimAuthor(ClaimDTO dto)
        {
            dto.Username = HttpContext.Current.Session["username"].ToString();
            repository.ClaimAuthor( dto );
            return new HttpResponseMessage() { Content = new StringContent("success") };
        }
       
    }
}