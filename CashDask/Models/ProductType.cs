using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CashDask.Models
{
    public class ProductType
    {
        [Key]
        public int ID { get; set; }
        public string name { get; set; }
        public bool can_return { get; set; }
    }
}
