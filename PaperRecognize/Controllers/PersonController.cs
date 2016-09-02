using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaperRecognize.DTOs.PersonDTO;
using PaperRecognize.Repository;

namespace PaperRecognize.Controllers
{
    public class PersonController : ApiController
    {
        private PersonRepository repository;

        public PersonController()
        {
            repository = new PersonRepository();
        }
        [Route("api/person/experts/{departId}")]
        public IEnumerable<GetPersonDTO> GetDepartExperts(int departId)
        {
            return repository.GetDepartExpert(departId);
        }
    }
}
