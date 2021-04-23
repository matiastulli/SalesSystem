using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Areas.Customers.Models
{
    public class TCustomer_interests
    {
        [Key]
        public int Idinterest { get; set; }
        public Decimal Debt { get; set; }
        public Decimal Monthly { get; set; }
        public Decimal Interests { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime Date { get; set; }
        public bool Canceled { get; set; }
        public int IdCustomer { get; set; }
    }
}
