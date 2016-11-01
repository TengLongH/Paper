using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.PaperDTO
{
    public class GetOnePaperDTO
    {
        public GetOnePaperDTO()
        {
            //Authors = new List<GetOneAuthorDTO>();
        }
        public int Id { get; set; }
        public string AuthorsShort { get; set; }
        public string AuthorsFull { get; set; }
        public string ChineseName { get; set; }
        public string DepartmentName { get; set; }
        public string LabName { get; set; }
        public string PaperName { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? PublishDate { get; set; }
        //public virtual List<GetOneAuthorDTO> Authors { get; set; }
    }

}