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

namespace PaperRecognize.Controllers
{
    public class AdminController : ApiController
    {
        private AdminRepository repository = new AdminRepository() ; 
        [Route("api/admin/claim/get")]
        public IEnumerable<GetOnePaperDTO> GetClaimPaper()
        {
            IEnumerable<GetOnePaperDTO> list = repository.GetClaimPaper();
            return list;
        }
        [Route("api/admin/paperauthors/get/{paperId}")]
        public IEnumerable<AuthorDTO> GetPaperAuthors( int paperId )
        {
            return repository.GetPaperAuthors( paperId );
        }
        [Route("api/admin/confirm/get")]
        public IEnumerable<GetConfirmDTO> GetConfrimPaper()
        {
            IEnumerable<GetConfirmDTO> list = repository.GetConfirmPapers();
            return list;
        }

        [Route("api/admin/cancel/get")]
        public IEnumerable<GetConfirmDTO> GetCancelPaper()
        {
            IEnumerable<GetConfirmDTO> list = repository.GetCancelPapers();
            return list;
        }
        [Route("api/admin/claim/post")]
        public HttpResponseMessage PostClaimPaper( ClaimDTO dto )
        {
            //repository.ClaimAuthorPerson(dto);
            return new HttpResponseMessage() { Content = new StringContent("success") };
        }

        [Route("api/admin/confirm/post")]
        public HttpResponseMessage PostConfirmPaper(ConfirmDTO dto)
        {
            repository.ConfirmAuthorPerson(dto);
            return new HttpResponseMessage() { Content = new StringContent("success") };
        }

        [Route("api/admin/authorperson")]
        public HttpResponseMessage PostUpdateAuthorPerson(UpdateAuthorPersonDTO update)
        {
            repository.UpdateAuthorPerson(update);
            return new HttpResponseMessage { Content = new StringContent("success") };
        }
        [Route("api/admin/search")]
        public IEnumerable<GetOnePaperDTO> PostSearchPaper( SearchPaperDTO search )
        {
            IEnumerable<GetOnePaperDTO> list = repository.SearchPaper( search );
            return list;
        }
    }
}
