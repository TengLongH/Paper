using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PaperRecognize.Repository;
using PaperRecognize.DTOs.DepartmentDTO;

namespace PaperRecognize.Controllers
{
    public class DepartmentController : ApiController
    {
        private DepartmentRepository repository = new DepartmentRepository();

        [Route("api/departments")]
        public IEnumerable<GetDepartmentDTO> GetDepartments()
        {
            return repository.GetDepartments();
        }
        [Route("api/colleges")]
        public IEnumerable<GetDepartmentDTO> GetColleges()
        {
            var list = repository.GetColleges();
            return list;
        }
    }
}
