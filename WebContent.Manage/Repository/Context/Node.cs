namespace WebContent.Manage.Repository.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Node")]
    public partial class Node
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Path { get; set; }

        public DateTime DateCreated { get; set; }

        public int ParentId { get; set; }

        [Required]
        [StringLength(5000)]
        public string Content { get; set; }

        [Required]
        [StringLength(250)]
        public string Summary { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }
    }
}
