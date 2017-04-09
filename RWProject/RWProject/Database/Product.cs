namespace RWProject.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public long? Category_id { get; set; }

        public string Model { get; set; }

        public decimal? Price { get; set; }

        public string Colour { get; set; }

        public string Size { get; set; }

        public string Description { get; set; }

        public virtual Category Category { get; set; }
    }
}
