using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace NirmalaAstrology_Web.Models
{
    public sealed class ArticleRecord
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string BodyHtml { get; set; }
        public string ImageUrl { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public sealed class ArticleEditModel
    {
        public int ArticleId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(220)]
        public string Slug { get; set; }

        [StringLength(500)]
        public string Summary { get; set; }

        [Required(ErrorMessage = "Article content is required.")]
        [AllowHtml]
        public string BodyHtml { get; set; }

        public string ImageUrl { get; set; }

        public bool IsPublished { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
    }
}
