﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs.AndroidDTO
{
    public class AuthorDTO
    {
        public AuthorDTO()
        {
            Authors = new List<PersonVO>();
        }
        public int Id { get; set; }
        public int Ordinal { get; set; }
        public string NameEN { get; set; }
        public string Department { get; set; }
        public List<PersonVO> Authors { get; set; }
    }

    public class PersonVO
    {
        public int PersonAuthorId { get; set; }
        public int Status { get; set; }
        public string PersonNo { get; set; }
        public string NameCN { get; set;}
        public string NameEN { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}