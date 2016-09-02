using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.AndroidDTO
{
    public class GetConfirmDTO
    {

        public int Id { get; set; }
        public string PaperName { get; set; }
        public int AuthorPersonId { get; set; }
    }
}