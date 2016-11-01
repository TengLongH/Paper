namespace PaperRecognize.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Candidate")]
    public partial class Candidate
    {
        public int Id { get; set; }

        public int? AuthorId { get; set; }

        [StringLength(64)]
        public string PersonNo { get; set; }

        [StringLength(127)]
        public string Name { get; set; }

        public int? status { get; set; }

        [StringLength(64)]
        public string Operator { get; set; }

        public virtual Author Author { get; set; }
    }
}
