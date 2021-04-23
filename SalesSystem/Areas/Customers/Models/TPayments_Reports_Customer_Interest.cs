using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Areas.Customers.Models
{
    public class TPayments_Reports_Customer_Interest
    {
        [Key]
        public int IdPaymentsinterest { get; set; }
        public Decimal Interests { get; set; }
        public Decimal Payment { get; set; }
        public Decimal Change { get; set; }
        public int Fee { get; set; }
        public DateTime Date { get; set; }
        public string Ticket { get; set; }
        public string IdUser { get; set; }
        public string User { get; set; }
        public int IdCustomer { get; set; }
    }
}
