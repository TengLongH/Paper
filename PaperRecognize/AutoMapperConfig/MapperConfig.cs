using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AutoMapper;

using PaperRecognize.DTOs;
using PaperRecognize.Models;
using PaperRecognize.DTOs.UserDTO;
using PaperRecognize.DTOs.DepartmentDTO;
using PaperRecognize.DTOs.PersonDTO;
using PaperRecognize.DTOs.PaperDTO;
using PaperRecognize.DTOs.AndroidDTO;
namespace PaperRecognize.AutoMapperConfig
{
    public class MapperConfig
    {
        public void config()
        {
            Mapper.Initialize( mapper => {
                mapper.CreateMap<Paper, GetOnePaperDTO>();

                mapper.CreateMap<Author, GetOneAuthorDTO>();
                mapper.CreateMap<Author_Person, GetAuthorPersonDTO>();
                mapper.CreateMap<Author_Person, Author_Person>();

                mapper.CreateMap<Person, GetPersonDTO>()
                    .ForMember(entity => entity.DepartmentId, opt=>opt.Ignore());
                mapper.CreateMap<AddUserDTO, User>();
                mapper.CreateMap<Department, GetDepartmentDTO>();
                mapper.CreateMap<Person, GetPersonDTO>();
                mapper.CreateMap<Paper, PaperDetailDTO>();
                
            });
           
        }
    }
}