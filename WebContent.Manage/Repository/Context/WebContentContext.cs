namespace WebContent.Manage.Repository.Context
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

       
    public partial class WebContentContext : DbContext
    {
        public WebContentContext()
            : base("name=WebContentContext")
        {
        }
        
        public virtual DbSet<Node> Nodes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Node>()
                .Property(e => e.Path)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.Content)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.Summary)
                .IsUnicode(false);

            modelBuilder.Entity<Node>()
                .Property(e => e.Title)
                .IsUnicode(false);
        }
    }
}
