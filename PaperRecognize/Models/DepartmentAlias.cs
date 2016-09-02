namespace PaperRecognize.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DepartmentAlias
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int DepartmentId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(256)]
        public string Alias { get; set; }

        public virtual Department Department { get; set; }
    }
}
