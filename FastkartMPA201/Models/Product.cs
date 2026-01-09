using System.ComponentModel.DataAnnotations;
using FastkartMPA201.Models.Common;

namespace FastkartMPA201.Models
{
    public class Product : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal DiscountedPrice { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        [Required]
        public bool IsInStock { get; set; }
        [Required]
        [Range(0, 5)]
        public int Rating { get; set; }
    }
}
