using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Areas.Customers.Models
{
    public class TCustomer_interests_reports
    {
        [Key]
        public int IdinterestReports { get; set; }
        public Decimal Interests { get; set; }
        public Decimal Payment { get; set; }
        public Decimal Change { get; set; }
        public int Fee { get; set; }
        public DateTime InterestDate { get; set; }
        public string TicketInterest { get; set; }
        public int IdClient { get; set; }
    }
}
