namespace PaperRecognize.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Department")]
    public partial class Department
    {
        public Department()
        {
            DepartmentAlias = new HashSet<DepartmentAlias>();
            Department1 = new HashSet<Department>();
            Paper = new HashSet<Paper>();
            Person_Department = new HashSet<Person_Department>();
            User = new HashSet<User>();
        }

        public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(64)]
        public string Code { get; set; }

        [StringLength(64)]
        public string ZipCode { get; set; }

        public int? ParentId { get; set; }

        public int? Type { get; set; }

        public virtual ICollection<DepartmentAlias> DepartmentAlias { get; set; }

        public virtual ICollection<Department> Department1 { get; set; }

        public virtual Department Department2 { get; set; }

        public virtual ICollection<Paper> Paper { get; set; }

        public virtual ICollection<Person_Department> Person_Department { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
