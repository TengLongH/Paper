namespace PaperRecognize.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Person")]
    public partial class Person
    {
        public Person()
        {
            Person_Department = new HashSet<Person_Department>();
        }

        public int Id { get; set; }

        [StringLength(64)]
        public string PersonNo { get; set; }

        [StringLength(128)]
        public string NameCN { get; set; }

        [StringLength(128)]
        public string NameEN { get; set; }

        [StringLength(128)]
        public string NameENAbbr { get; set; }

        [StringLength(6)]
        public string Sex { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Birthday { get; set; }

        [StringLength(256)]
        public string IDCard { get; set; }

        [StringLength(64)]
        public string Mobile { get; set; }

        [StringLength(512)]
        public string Email { get; set; }

        [StringLength(128)]
        public string Tutor { get; set; }

        public int? PersonType { get; set; }

        public virtual ICollection<Person_Department> Person_Department { get; set; }
    }
}
