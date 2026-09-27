using System.Collections.Generic;
using NirmalaAstrology_Web.Models;

namespace NirmalaAstrology_Web.ViewModels
{
    /// <summary>
    /// ViewModel for the Home page.
    /// Contains all data needed to render the homepage.
    /// </summary>
    public class HomeViewModel
    {
        /// <summary>
        /// Customer form model for Kundli generation.
        /// </summary>
        public CustomerModel Customer { get; set; } = new CustomerModel();

        /// <summary>
        /// List of all 12 zodiac signs for the zodiac cards section.
        /// </summary>
        public List<ZodiacModel> ZodiacList { get; set; } = new List<ZodiacModel>();

        public List<PlaceModel> Places { get; set; } = new List<PlaceModel>();

        /// <summary>
        /// Today's panchang snapshot for the public homepage card.
        /// </summary>
        public LegacyPanchangModel TodayPanchang { get; set; }

        /// <summary>
        /// Display date label for panchang card.
        /// </summary>
        public string TodayPanchangDateLabel { get; set; }

        /// <summary>
        /// Today's horoscope preview text.
        /// </summary>
        public string TodaysHoroscopeText { get; set; }

        /// <summary>
        /// List of latest articles (if any).
        /// </summary>
        public List<ArticlePreview> LatestArticles { get; set; } = new List<ArticlePreview>();

        /// <summary>
        /// List of testimonials for the carousel.
        /// </summary>
        public List<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

        /// <summary>
        /// List of FAQ items.
        /// </summary>
        public List<FAQItem> FAQs { get; set; } = new List<FAQItem>();
    }

    /// <summary>
    /// Lightweight article preview model for the homepage.
    /// </summary>
    public class ArticlePreview
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Slug { get; set; }
    }

    /// <summary>
    /// Testimonial model for the testimonials carousel.
    /// </summary>
    public class Testimonial
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Text { get; set; }
        public string Rating { get; set; }
    }

    /// <summary>
    /// FAQ item for the accordion section.
    /// </summary>
    public class FAQItem
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}