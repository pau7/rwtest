using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RWProject.Models
{
    public partial class CategoryViewModel
    {
        public long Id { get; set; }
        public string Category { get; set; }
    }
}
