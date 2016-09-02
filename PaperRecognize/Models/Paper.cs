namespace PaperRecognize.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Paper")]
    public partial class Paper
    {
        public Paper()
        {
            Author = new HashSet<Author>();
        }

        public int Id { get; set; }

        [StringLength(255)]
        public string PressType { get; set; }

        [Required]
        public string AuthorsShort { get; set; }

        [Required]
        public string AuthorsFull { get; set; }

        public string ChineseName { get; set; }

        [StringLength(1024)]
        public string FirstAuthorSignUnit { get; set; }

        public int? SignOrder { get; set; }

        [StringLength(512)]
        public string DepartmentName { get; set; }

        [StringLength(512)]
        public string LabName { get; set; }

        public string PaperName { get; set; }

        [StringLength(512)]
        public string JournalName { get; set; }

        [StringLength(255)]
        public string Series { get; set; }

        [StringLength(64)]
        public string Language { get; set; }

        [StringLength(64)]
        public string PaperType { get; set; }

        public string AuthorKeyWord { get; set; }

        public string KeyWords { get; set; }

        public string Abstract { get; set; }

        public string AuthorsAddress { get; set; }

        [StringLength(1024)]
        public string CorrespondenceEN { get; set; }

        [StringLength(1024)]
        public string CorrespondenceCN { get; set; }

        [StringLength(1024)]
        public string CorrespondenceSignUnit { get; set; }

        [StringLength(1024)]
        public string EmailAddress { get; set; }

        [Column(TypeName = "text")]
        public string Reference { get; set; }

        public int? ReferenceCount { get; set; }

        public int? CitedCount { get; set; }

        [StringLength(512)]
        public string Press { get; set; }

        [StringLength(512)]
        public string City { get; set; }

        [StringLength(200)]
        public string PressAddress { get; set; }

        [StringLength(128)]
        public string ISSN { get; set; }

        [StringLength(128)]
        public string DI { get; set; }

        [StringLength(512)]
        public string StandardJournalAbbr { get; set; }

        [StringLength(512)]
        public string ISIJournalAbbr { get; set; }

        [Column(TypeName = "date")]
        public DateTime? PublishDate { get; set; }

        public int? Year { get; set; }

        [StringLength(64)]
        public string Volume { get; set; }

        [StringLength(64)]
        public string Issue { get; set; }

        [StringLength(64)]
        public string PartNumber { get; set; }

        public bool? Supplement { get; set; }

        [StringLength(255)]
        public string SpecialIssue { get; set; }

        public int? StartPage { get; set; }

        public int? EndPage { get; set; }

        public int? PageCount { get; set; }

        [StringLength(255)]
        public string ArticleNumber { get; set; }

        [StringLength(1024)]
        public string SubjectCategory { get; set; }

        [StringLength(100)]
        public string IncludeType { get; set; }

        [StringLength(200)]
        public string ISIDeliveryNo { get; set; }

        [StringLength(50)]
        public string ISIArticleIdentifier { get; set; }

        public int? status { get; set; }

        public int? PaperDepartmentId { get; set; }

        public virtual ICollection<Author> Author { get; set; }

        public virtual Department Department { get; set; }
    }
}
