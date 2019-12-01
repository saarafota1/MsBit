using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CashDask.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }

        public DateTime created
        {
            get
            {
                return this.dateCreated.HasValue
                   ? this.dateCreated.Value
                   : DateTime.Now;
            }

            set { this.dateCreated = value; }
        }
        private DateTime? dateCreated = null;
        public float price { get; set; }

        public ProductType type { get; set; }

    }
}
