using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CashDask.Models
{
    public class Purchase
    {
        [Key]
        public int ID { get; set; }

        public ICollection<Product>? products { get; set; }

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

        public DateTime return_date
        {
            get
            {
                return this.returnDate.HasValue
                   ? this.returnDate.Value
                   : DateTime.Now;
            }

            set { this.returnDate = value; }
        }
        private DateTime? returnDate = null;
        public bool _return { get; set; }
    }
}
