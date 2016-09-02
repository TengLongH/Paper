namespace PaperRecognize.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Log")]
    public partial class Log
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [StringLength(50)]
        public string Level { get; set; }

        [Required]
        [StringLength(255)]
        public string Logger { get; set; }

        [Required]
        [StringLength(4000)]
        public string Message { get; set; }

        [StringLength(2000)]
        public string Exception { get; set; }

        public DateTime? Time { get; set; }
    }
}
