using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaperRecognize.DTOs
{
    /// <summary>
    /// 论文状态枚举类
    /// </summary>
    public enum PaperStatus
    {
        /// <summary>
        /// 论文导入，需要预处理
        /// </summary>
        PRETREATMENT,
        /// <summary>
        /// 经过预处理，系统还没有搜索作者对应的人
        /// </summary>
        LOOKUP,
        /// <summary>
        /// 系统已经搜索过作者对应的人，系统的推荐信息还没有经过用户和管理员确认
        /// </summary>
        CONFIRM,
        /// <summary>
        /// 所有作者都找到了对应人
        /// </summary>
        DEAL
    }
    /// <summary>
    /// 候选人状态枚举类
    /// </summary>
    public enum CandidateStatus
    {
        /// <summary>
        /// 候选人是作者
        /// </summary>
        RIGHT,
        /// <summary>
        /// 候选人不是作者
        /// </summary>
        WRONG,
        /// <summary>
        /// 等待用户或者管理员确认
        /// </summary>
        CHECK
    }
    /// <summary>
    /// 用户角色枚举类
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        COMMON ,
        /// <summary>
        /// 管理员
        /// </summary>
        ADMIN 
    }
    /// <summary>
    /// 部门类型枚举类
    /// </summary>
    public enum DepartmentType
    {
        /// <summary>
        /// 学校
        /// </summary>
        SCHOOL = 1,
        /// <summary>
        /// 学院
        /// </summary>
        COLLEGE = 10,
        /// <summary>
        /// 学院下属的系
        /// </summary>
        DEPARTMENT = 20,
        /// <summary>
        /// 实验室
        /// </summary>
        LAB= 21,
        /// <summary>
        /// 研究所
        /// </summary>
        INSTITUTE = 22

    }
}