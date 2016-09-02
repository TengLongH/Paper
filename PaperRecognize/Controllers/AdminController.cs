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

namespace PaperRecognize.Controllers
{
    public class AdminController : ApiController
    {
        private AdminRepository repository = new AdminRepository() ; 
        [Route("api/admin/claim")]
        public IEnumerable<GetOnePaperDTO> GetClaimPaper()
        {
            var session = HttpContext.Current.Session;
            //String username = session["username"].ToString();
            //int role = Int32.Parse(session["role"].ToString());
            string username = "1993027";
            int role = 1;
            if (role != (int)UserRole.DEPTADMIN) return null;
            IEnumerable<GetOnePaperDTO> list = repository.GetClaimPaper();
            return list;
        }
        [Route("api/admin/paperauthors/{paperId}")]
        public IEnumerable<AuthorDTO> GetPaperAuthors( int paperId )
        {
            return repository.GetPaperAuthors( paperId );
        }
    }
}
