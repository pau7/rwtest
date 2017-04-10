using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RWProject.Models
{
    public class ProductViewModel
    {
        public ProductViewModel() {
            Categories = new List<CategoryViewModel>();
            Subcategories = new List<CategoryViewModel>();
        }
        public long Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Product { get; set; }
        public string Category { get; set; }        
        public string Subcategory { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        [Required]
        public string Description { get; set; }
        public string Actions { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<CategoryViewModel> Subcategories { get; set; }
        public long CategoryId { get; set; }
        public long SubcategoryId { get; set; }        
    }
}
