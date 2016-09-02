using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs
{
    public enum PaperStatus
    {
        ANALISIS,
        CONFIRM,
        DEAL
    }
    public enum AuthorPersonStatus
    {
        //0:未确认
        CONFIRM,
        //1：确认正确
        RIGHT,
        //2：确认错误
        WRONG,
        //3.待认领
        NEEDCLAIM,
        //4：已认领
        CLAIM,
        //5.已驳回
        REJECT
    }
    public enum UserRole
    {
        COMMON ,
        DEPTADMIN ,
        SCHOOLADMIN,
    }

    public enum DepartmentType
    {
        SCHOOL = 1,
        COLLEGE = 10,
        DEPARTMENT = 20,
        LAB= 21,
        INSTITUTE = 22

    }
}