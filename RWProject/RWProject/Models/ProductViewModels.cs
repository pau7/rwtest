using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RWProject.Models
{
    public partial class ProductViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string Model { get; set; }
        public decimal? Price { get; set; }
        public string Colour { get; set; }
        public string Size { get; set; }
        public string Description { get; set; }
        public string Actions { get; set; }
        
    }
}
