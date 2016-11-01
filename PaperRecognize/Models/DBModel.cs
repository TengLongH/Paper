namespace PaperRecognize.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }

        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<DepartmentAlias> DepartmentAlias { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Paper> Paper { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Person_Department> Person_Department { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>()
                .Property(e => e.NameEN)
                .IsUnicode(false);

            modelBuilder.Entity<Author>()
                .Property(e => e.NameENAbbr)
                .IsUnicode(false);

            modelBuilder.Entity<Author>()
                .Property(e => e.Department)
                .IsUnicode(false);

            modelBuilder.Entity<Candidate>()
                .Property(e => e.PersonNo)
                .IsUnicode(false);

            modelBuilder.Entity<Candidate>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Candidate>()
                .Property(e => e.Operator)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.DepartmentAlias)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Department1)
                .WithOptional(e => e.Department2)
                .HasForeignKey(e => e.ParentId);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Paper)
                .WithOptional(e => e.Department)
                .HasForeignKey(e => e.PaperDepartmentId);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Person_Department)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DepartmentAlias>()
                .Property(e => e.Alias)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Level)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Logger)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<Log>()
                .Property(e => e.Exception)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.PressType)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.AuthorsShort)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.AuthorsFull)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.ChineseName)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.FirstAuthorSignUnit)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.DepartmentName)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.LabName)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.PaperName)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.JournalName)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.Series)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.Language)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.PaperType)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.AuthorKeyWord)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.KeyWords)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.Abstract)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.AuthorsAddress)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.CorrespondenceEN)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.CorrespondenceCN)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.CorrespondenceSignUnit)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.EmailAddress)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.Reference)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.Press)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.PressAddress)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.ISSN)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.DI)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.StandardJournalAbbr)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.ISIJournalAbbr)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.Volume)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.Issue)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.PartNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.SpecialIssue)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.ArticleNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.SubjectCategory)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.IncludeType)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.ISIDeliveryNo)
                .IsUnicode(false);

            modelBuilder.Entity<Paper>()
                .Property(e => e.ISIArticleIdentifier)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.PersonNo)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.NameCN)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.NameEN)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.NameENAbbr)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Sex)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.IDCard)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Mobile)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .Property(e => e.Tutor)
                .IsUnicode(false);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.Person_Department)
                .WithRequired(e => e.Person)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);
        }
    }
}
