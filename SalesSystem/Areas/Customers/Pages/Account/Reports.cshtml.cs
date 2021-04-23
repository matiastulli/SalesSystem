using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesSystem.Areas.Customers.Models;
using SalesSystem.Data;
using SalesSystem.Library;
using SalesSystem.Models;

namespace SalesSystem.Areas.Customers.Pages.Account
{
    [Authorize]
    public class ReportsModel : PageModel
    {
        private LCustomers _customer;
        private static int idClient = 0;
        public string Money = "$";
        private static string _errorMessage;
        public static InputModelRegister _dataClient;
        public static InputModelInterests _dataInterests;
        private LCodes _codes;
        private ApplicationDbContext _context; 
        private UserManager<IdentityUser> _userManager;

        public ReportsModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _codes = new LCodes();
            _customer = new LCustomers(context);
        }
        public IActionResult OnGet(int id, InputModel input)
        {
            if (idClient == 0)
            {
                idClient = id;
            }
            else
            {
                if (idClient != id)
                {
                    idClient = 0;
                    return Redirect("/Customers/Customers?area=Customers");
                }
            }
            _dataClient = _customer.getTClientReport(id);
            _dataClient.Time1 = input.Time1;
            _dataClient.Time2 = input.Time2;
            _dataInterests = _customer.getTClientInterests(id);

            Input = new InputModel
            {
                DataClient = _dataClient,
                ErrorMessage = _errorMessage,
                TPayments = _customer.GetPayments(id, 1, 10, _dataClient, Request),
                DataInterests = _dataInterests,
            };
            _errorMessage = "";
            return Page();    
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public string Money { get; set; } = "$";
            [Required(ErrorMessage = "Seleccione una opcion.")]
            public int RadioOptions { get; set; }

            [Required(ErrorMessage = "El campo pago es obligatorio.")]
            [RegularExpression(@"^[0-9]+([.][0-9]+)?$", ErrorMessage = "El pago no es correcto.")]
            public Decimal Payment { set; get; }

            public InputModelRegister DataClient { get; set; }
            public InputModelInterests DataInterests { get; set; }

            [TempData]
            public string ErrorMessage { get; set; }

            public DataPaginador<TPayments_clients> TPayments { get; set; }

            public DateTime Time1 { get; set; } = DateTime.Now.Date;
            public DateTime Time2 { get; set; } = DateTime.Now.Date;

            [Required(ErrorMessage = "Ingrese la cantidad de cuotas a cancelar.")]
            public string AmountFees { get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var idUser = _userManager.GetUserId(User);

            switch (Input.RadioOptions)
            {
                case 1:
                    if (_dataClient.Debt.Equals(0.0m))
                    {
                        _errorMessage = "El cliente no contiene deuda";
                    }
                    else
                    {
                        string _change = "";
                        decimal _currentDebt = 0.0m, change;
                        if (Input.Payment >= _dataClient.Monthly)
                        {
                            var _ticket = _codes.codesTickets(_dataClient.Ticket);
                            var _nameCliente = $"{_dataClient.Name}{_dataClient.LastName}";
                            var dateNow = DateTime.Now;
                            var nowDate = $"{dateNow.Day}/{dateNow.Month}/{dateNow.Year}";
                            var user = _context.TUsers.Where(u => u.IdUser.Equals(idUser)).ToList();
                            var name = $"{user[0].Name} {user[0].LastName}";
                            if (Input.Payment.Equals(_dataClient.CurrentDebt) || Input.Payment > _dataClient.CurrentDebt)
                            {
                                change = Input.Payment - _dataClient.CurrentDebt;
                                _change = string.Format("{0:#,###,###,##0.00####}", change);
                                _errorMessage = $"Cambio para el cliente {Money}{_change}";
                                _currentDebt = 0.0m;
                            }
                            else
                            {
                                change = Input.Payment - _dataClient.Monthly;
                                _change = string.Format("{0:#,###,###,##0.00####}", change);
                                _errorMessage = $"Cambio para el cliente {Money}{_change}";
                                _currentDebt = _dataClient.CurrentDebt - _dataClient.Monthly;

                            }
                            //Para almacenar los valores en la base de datos
                            var strategy = _context.Database.CreateExecutionStrategy();
                            await strategy.ExecuteAsync( async () => {
                                using (var transaction = _context.Database.BeginTransaction())
                                {
                                    try
                                    {

                                        var _payment = string.Format("{0:#,###,###,##0.00####}", Input.Payment);
                                        var _debt = string.Format("{0:#,###,###,##0.00####}", _dataClient.Debt);
                                        var currentDebt = string.Format("{0:#,###,###,##0.00####}", _currentDebt);
                                        var cambio = string.Format("{0:#,###,###,##0.00####}", _change);
                                        var _currentDebtClient = string.Format("{0:#,###,###,##0.00####}", _dataClient.CurrentDebt);

                                        var _monthly = string.Format("{0:#,###,###,##0.00####}", _dataClient.Monthly);
                                        var date = DateTime.Now.AddMonths(1);

                                        var _deadLine = _dataClient.CurrentDebt.Equals(0.0m) ? "--/--/--" : $"{date.Day}/{date.Month}/{date.Year}";
                                        var client = _context.TClients.Where(c => c.IdClient.Equals(_dataClient.IdClient)).ToList().ElementAt(0);
                                        var report = new TReports_clients
                                        {
                                            IdReport = _dataClient.IdReport,
                                            Debt = _dataClient.Debt,
                                            DateDebt = _dataClient.DateDebt,
                                            Monthly = _dataClient.Monthly,
                                            Change = change,
                                            LastPayment = Input.Payment,
                                            DatePayment = dateNow,
                                            CurrentDebt = _currentDebt,
                                            Ticket = _ticket,
                                            Deadline = date,
                                            TClients = client

                                        };

                                        _context.Update(report);
                                        _context.SaveChanges();

                                        var payments = new TPayments_clients
                                        {
                                            Debt = _dataClient.Debt,
                                            Change = change,
                                            Payment = Input.Payment,
                                            Date = dateNow,
                                            CurrentDebt = _currentDebt,
                                            Ticket = _ticket,
                                            DeadLine = date,
                                            DateDebt = _dataClient.DateDebt,
                                            Monthly = _dataClient.Monthly,
                                            PreviousDebt = _dataClient.CurrentDebt,
                                            User = name,
                                            IdClient = _dataClient.IdClient,
                                            IDUser = idUser
                                        };

                                        _context.Update(payments);
                                        _context.SaveChanges();

                                        LTicket Ticket1 = new LTicket();
                                        Ticket1.AbreCajon(); //Abre el cajon
                                        Ticket1.TextoCentro("Sistema de Ventas"); //Imprime en el centro
                                        Ticket1.TextoIzquierda("Direccion");
                                        Ticket1.TextoIzquierda("Recoleta, CABA");
                                        Ticket1.TextoIzquierda("Tel 2364546014");
                                        Ticket1.LineasGuion();
                                        Ticket1.TextoCentro("FACTURA"); //Imprime en el centro
                                        Ticket1.LineasGuion();
                                        Ticket1.TextoIzquierda($"Factura: {_ticket}");
                                        Ticket1.TextoIzquierda($"Cliente: {_nameCliente}");
                                        Ticket1.TextoIzquierda($"Fecha: {nowDate}");
                                        Ticket1.TextoIzquierda($"Usuario: {name}");
                                        Ticket1.LineasGuion();
                                        Ticket1.TextoCentro($"Su credito{Money}{_debt}");
                                        Ticket1.TextoExtremo($"Cuotas por meses:", $"{Money}{_monthly}");
                                        Ticket1.TextoExtremo($"Deuda anterior:", $"{Money}{_currentDebtClient}");
                                        Ticket1.TextoExtremo($"Pago:", $"{Money}{_payment}");
                                        Ticket1.TextoExtremo($"Cambio:", $"{Money}{_change}");
                                        Ticket1.TextoExtremo($"Deuda actual:", $"{Money}{_currentDebt}");
                                        Ticket1.TextoExtremo($"Próximo pago:", $"{_deadLine}");
                                        Ticket1.TextoCentro("Juan Matias Tulli");
                                        Ticket1.CortaTicket(); //Corta el ticket

                                        Ticket1.ImprimirTicket("Microsoft XPS Document Writer");

                                        transaction.Commit();

                                    }
                                    catch (Exception ex)
                                    {
                                        _errorMessage = ex.Message;
                                        transaction.Rollback();
                                    }
                                }
                            });
                        }
                        else
                        {
                            var monthly = string.Format("{0:#,###,###,##0.00####}", _dataClient.Monthly);
                            _errorMessage = $"El pago debe ser {Money}{monthly}";
                        }
                    }
                    break;
            }
            return Redirect("/Customers/Reports?id=" + idClient);
        }
    }
}
