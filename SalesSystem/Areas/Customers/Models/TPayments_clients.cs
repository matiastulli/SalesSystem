using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Areas.Customers.Models
{
    public class TPayments_clients
    {
        [Key]
        public int IdPayments { get; set; }
        [Display(Name = "Deuda")]
        public Decimal Debt { get; set; }
        [Display(Name = "Cambio")]
        public Decimal Change { get; set; }
        [Display(Name = "Pago")]
        public Decimal Payment { get; set; }
        [Display(Name = "Fecha")]
        public DateTime Date { get; set; }
        public DateTime DeadLine { get; set; }
        [Display(Name = "Deuda corriente")]
        public DateTime DateDebt { get; set; }
        public Decimal CurrentDebt { get; set; }
        public Decimal Monthly { get; set; }
        public Decimal PreviousDebt { get; set; }
        public string Ticket { get; set; }
        public string IDUser { get; set; }
        public string User { get; set; }
        public int IdClient { get; set; }
    }
}
