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

        public UserController() 
        {
            repository = new UserRepository();
        }
        [Route("api/user/mypaper")]
        public IEnumerable<GetOnePaperDTO> GetMyPaper() 
        {
            var session = HttpContext.Current.Session;
            //String username = session["username"].ToString();
            //int role = Int32.Parse(session["role"].ToString());
            string username = "2012304";
            int role = 0;
            if( role != (int)UserRole.COMMON )return null;
            IEnumerable<GetOnePaperDTO> list = repository.GetMyPaper(username);
            return list;
        }

        [Route("api/user/confirm")]
        public HttpResponseMessage GetConfirmPapers() {
            var session = HttpContext.Current.Session;
            //String username = session["username"].ToString();
            //int role = Int32.Parse(session["role"].ToString());
            //if (role != (int)UserRole.COMMON) return null;
            return Util.toJson(repository.GetConfirmPapers("2012304"));
        }
        [Route("api/user/claim")]
        public HttpResponseMessage GetClaimPapers()
        {
            return Util.toJson(repository.GetClaimAuthors());
        }
        [Route("api/user/paper/{paperId}")]
        public HttpResponseMessage GetPaperDetail( int paperId )
        {
            return Util.toJson(repository.GetPaperDetail(paperId));
        }
        [Route("api/user/confirm/request")]
        public HttpResponseMessage PostConfirmPaper( ConfirmDTO dto ) {

            repository.confirmAuthorPerson( dto );
            return Util.toJson("success");
        }
        [Route("api/user/claim/request")]
        public HttpResponseMessage PostClaimPaper(ClaimDTO dto)
        {

            repository.claimAuthorPerson( dto );
            return Util.toJson("success");
        }
        [Route("api/user/myclaim")]
        public HttpResponseMessage GetMyClaimPaper(ClaimDTO dto)
        {

            var list = repository.GetMyClaimPaper("2012304");
            return Util.toJson(list);
        }

        [Route("api/managers")]
        public IEnumerable<GetPersonDTO> GetManagers()
        {
            IEnumerable<GetPersonDTO> managers;
            managers = repository.GetManager();
            return managers;
        }

        [Route("api/user/{name}/{role}")]
        public string DeleteUser( string name, int role ) 
        {
            return "";
            //return repository.DeleteUser( dto );
        }

        [Route("api/user")]
        public string PostUser( AddUserDTO dto )
        {
            return repository.AddUser(dto);
        }

        [Route("api/user")]
        public string PutUser(UpdateUserDTO dto)
        {
            return repository.UpdateUser( dto );
        }
    }
}
