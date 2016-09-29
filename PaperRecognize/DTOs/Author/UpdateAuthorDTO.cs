using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs
{
    public class UpdateAuthorPersonDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string PersonNo { get; set; }
        public string NameEN { get; set; }
        public string NameCN { get; set; }
        public AuthorPersonStatus status { get; set; }

    }
}