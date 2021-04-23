using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesSystem.Areas.Customers.Models;
using SalesSystem.Data;
using SalesSystem.Library;

namespace SalesSystem.Areas.Customers.Pages.Account
{
    [Authorize]
    public class DetailsDebt : PageModel
    {
        private static int _idDebt = 0;
        private static int _idClient = 0;
        public string Money = "$";
        private static string _errorMessage;
        public static InputModelRegister _dataClient;
        private LCodes _codes;
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private LCustomers _customer;

        public DetailsDebt(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _codes = new LCodes();
            _customer = new LCustomers(context);
        }
        public IActionResult OnGet(int idDebt, int idClient)
        {
            if (_idDebt.Equals(0) && _idClient.Equals(0))
            {
                _idDebt = idDebt;
                _idClient = idClient;
            }
            else
            {
                if (_idDebt != idDebt || _idClient != idClient)
                {
                    _idDebt = 0;
                    return Redirect("/Customers/Reports?id=" + _idClient + "&area=Customers");
                }
            }
            _dataClient = _customer.getTClientPayment(idDebt);
            Input = new InputModel
            {
                DataClient = _dataClient,
            };
            return Page();
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public string Money { get; set; } = "$";

            public InputModelRegister DataClient { get; set; }
        }

        public async Task<IActionResult> OnPost()
        {
            var _nameClient = $"{_dataClient.Name} {_dataClient.LastName}";
            var _debt = string.Format("{0:#,###,###,##0.00####}", _dataClient.Debt);
            var _currentDebt = string.Format("{0:#,###,###,##0.00####}", _dataClient.CurrentDebt);
            var _payment = string.Format("{0:#,###,###,##0.00####}", _dataClient.Payment);
            var _change = string.Format("{0:#,###,###,##0.00####}", _dataClient.Change);
            var monthly = string.Format("{0:#,###,###,##0.00####}", _dataClient.Monthly); //Falta
            var previousDebt = string.Format("{0:#,###,###,##0.00####}", _dataClient.PreviousDebt); //Falta

            LTicket Ticket1 = new LTicket();
            Ticket1.AbreCajon(); //Abre el cajon
            Ticket1.TextoCentro("Sistema de Ventas"); //Imprime en el centro
            Ticket1.TextoIzquierda("Direccion");
            Ticket1.TextoIzquierda("Recoleta, CABA");
            Ticket1.TextoIzquierda("Tel 2364546014");
            Ticket1.LineasGuion();
            Ticket1.TextoCentro("FACTURA"); //Imprime en el centro
            Ticket1.LineasGuion();
            Ticket1.TextoIzquierda($"Factura: {_dataClient.Ticket}");
            Ticket1.TextoIzquierda($"Cliente: {_nameClient}");
            Ticket1.TextoIzquierda($"Fecha: {_dataClient.Date.ToString("dd/MM/yy")}");
            Ticket1.TextoIzquierda($"Usuario: {_dataClient.User}");
            Ticket1.LineasGuion();
            Ticket1.TextoCentro($"Su credito {Money}{_debt}");
            Ticket1.TextoExtremo($"Cuotas por mes:", $"{Money}{monthly}");
            Ticket1.TextoExtremo($"Deuda anterior:", $"{Money}{previousDebt}");
            Ticket1.TextoExtremo($"Pago:", $"{Money}{_payment}");
            Ticket1.TextoExtremo($"Cambio:", $"{Money}{_change}");
            Ticket1.TextoExtremo($"Deuda actual:", $"{Money}{_currentDebt}");
            Ticket1.TextoExtremo($"Próximo pago:", $"{_dataClient.Deadline.ToString("dd/MM/yy")}");
            Ticket1.TextoCentro("Juan Matias Tulli");
            Ticket1.CortaTicket(); //Corta el ticket

            Ticket1.ImprimirTicket("Microsoft XPS Document Writer");

            return Redirect("/Customers/Reports?id=" + _idClient + "&area=Customers");
        }
    }
}
