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
    public class UserController : ApiController
    {
        public static int Claim_Page_Size = 5;
        private UserRepository repository;
        private Utils.Authority authority = Utils.Authority.GetInstance();
        public UserController() 
        {
            repository = new UserRepository();
        }
        [Route("api/user/mypaper/get")]
        public IEnumerable<GetOnePaperDTO> GetMyPaper() 
        {
           
            object username = HttpContext.Current.Session["username"];
            IEnumerable<GetOnePaperDTO> list = repository.GetMyPaper(username.ToString());
            return list;
        }

        [Route("api/user/confirm/get")]
        public HttpResponseMessage GetConfirmPapers()
        {
           

            return Util.toJson(repository.GetConfirmPapers("2012304"));
        }
        [Route("api/user/claim/get")]
        public HttpResponseMessage GetClaimPapers()
        {
           
            return Util.toJson(repository.GetClaimAuthors());
        }
        [Route("api/user/paper/detail/{paperId}")]
        public HttpResponseMessage GetPaperDetail( int paperId )
        {
           
            return Util.toJson(repository.GetPaperDetail(paperId));
        }
        [Route("api/user/myclaim/get")]
        public HttpResponseMessage GetMyClaimPaper()
        {
            var name = HttpContext.Current.Session["username"];
            var list = repository.GetMyClaimPaper(name.ToString());
            return Util.toJson(list);
        }


        [Route("api/user/confirm/post")]
        public HttpResponseMessage PostConfirmPaper( ConfirmDTO dto )
        {

            dto.Username = HttpContext.Current.Session["username"].ToString();
            repository.ConfirmAuthorPerson( dto );
            return new HttpResponseMessage() { Content= new StringContent("success")};
        }
        [Route("api/user/claim/post")]
        public HttpResponseMessage PostClaimPaper(ClaimDTO dto)
        {
            dto.Username = HttpContext.Current.Session["username"].ToString();
            repository.ClaimAuthorPerson( dto );
            return new HttpResponseMessage() { Content = new StringContent("success") };
        }
       
    }
}
