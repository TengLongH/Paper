using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaperRecognize.Models;
using PaperRecognize.DTOs.DepartmentDTO;
using AutoMapper;

namespace PaperRecognize.Repository
{
    public class DepartmentRepository
    {
        private DBModel context = new DBModel();

        public IEnumerable<GetDepartmentDTO> GetDepartments() 
        {
            return context.Department.Select(Mapper.Map<GetDepartmentDTO>).ToList();
        }

        public IEnumerable<GetDepartmentDTO> GetColleges()
        {
            return context.Department
                .Where( d =>d.Type >= 10 && d.Type < 20 )
                .Select(Mapper.Map<GetDepartmentDTO>).ToList();
        }

        public string AddDepartment( AddDepartmentDTO dto )
        {
            Department depart = Mapper.Map<Department>(dto);
            try
            {
                context.Department.Add(depart);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                return e.Message;
            }
            return "success";
        }

        public string UpdateDepartment( UpdateDepartmentDTO dto ) 
        {
            Department depart = context.Department.FirstOrDefault(d => d.Id == dto.Id);
            if (null == depart)
            {
                return "can't find the department";
            }
            if (depart.Id != dto.Id) 
            {
                if (null != dto.ParentId && 
                    null == context.Department.FirstOrDefault(d => d.Id == dto.ParentId))
                {
                    return "parent id is error";
                }
            }
            
            depart.Name = dto.Name;
            depart.ParentId = dto.ParentId;
            depart.Type = dto.Type;
            depart.Code = dto.Code;
            depart.ZipCode = dto.ZipCode;
            context.SaveChanges();
            return "success";
        }

        public string AddDepartAlias( AddDepartAliasDTO dto )
        {
            DepartmentAlias alias = Mapper.Map<DepartmentAlias>(dto);
            try
            {
                context.DepartmentAlias.Add(alias);
                context.SaveChanges();
            }
            catch( Exception e )
            {
                return e.Message;
            }
            return "success";
        }
        public string DeleteDepartAlias( int departId, string alias )
        {
            DepartmentAlias da = context.DepartmentAlias.FirstOrDefault(
                entity =>entity.DepartmentId == departId &&
                entity.Alias == alias );
            if (null == da)
            {
                return "can't find the alias";
            }
            context.DepartmentAlias.Remove(da);
            context.SaveChanges();
            return "success";
        }

        

    }
}