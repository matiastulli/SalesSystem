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
        public string Money;
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
            Money = LSetting.Moneda;
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
                TInterest = _customer.GetInterests(id, 1, 10, _dataClient, Request),
            };
            _errorMessage = "";
            return Page();    
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public string Money { get; set; } = LSetting.Moneda;
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

            public int AmountFees { get; set; }
            public DataPaginador<TPayments_Reports_Customer_Interest> TInterest { get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var idUser = _userManager.GetUserId(User);

            var dateNow = DateTime.Now;
            var _nameCliente = $"{_dataClient.Name}{_dataClient.LastName}";
            var user = _context.TUsers.Where(u => u.IdUser.Equals(idUser)).ToList();
            var name = $"{user[0].Name} {user[0].LastName}";
            var nowDate = $"{dateNow.Day}/{dateNow.Month}/{dateNow.Year}";

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

                                        if (_currentDebt.Equals(0.0))
                                        {
                                            var report = new TReports_clients
                                            {
                                                IdReport = _dataClient.IdReport,
                                                Debt = 0.0m,
                                                DateDebt = dateNow,
                                                Monthly = 0.0m,
                                                Change = 0.0m,
                                                LastPayment = 0.0m,
                                                DatePayment = dateNow,
                                                CurrentDebt = 0.0m,
                                                Ticket = "00000000000",
                                                Deadline = null,
                                                TClients = client

                                            };

                                            _context.Update(report);
                                            _context.SaveChanges();
                                        }
                                        else
                                        {
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
                                        }
                                        
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

                case 2:
                    var strategy2 = _context.Database.CreateExecutionStrategy();
                    await strategy2.ExecuteAsync(async () => {
                        using (var transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                Decimal changes = 0;
                                List<TCustomer_interests_reports> interests = null;
                                List<TCustomer_interests> listInterests = null;
                                var fees = _customer.AmountFees(Input.AmountFees, idClient);
                                var amountFees = Convert.ToDecimal(fees);
                                using (var dbContext = new ApplicationDbContext())
                                {
                                    interests = dbContext.TCustomer_interests_reports.Where(
                                        c => c.IdClient.Equals(_dataClient.IdClient)).ToList();

                                    listInterests = dbContext.TCustomer_interests.Where(
                                        c => c.IdCustomer.Equals(_dataClient.IdClient) && c.Canceled.Equals(false)).ToList();
                                }
                                var interest = interests.Count > 0 ? interests.ElementAt(0) : new TCustomer_interests_reports();
                                var ticket = _codes.codesTickets(interest.TicketInterest);
                                if (Input.Payment >= amountFees)
                                {
                                    if (Input.Payment > amountFees)
                                    {
                                        changes = Input.Payment - amountFees;
                                    }
                                    
                                    var reports = new TPayments_Reports_Customer_Interest
                                    {
                                        Interests = amountFees,
                                        Payment = Input.Payment,
                                        Change = changes,
                                        Fee = Input.AmountFees,
                                        Date = dateNow,
                                        Ticket = ticket,
                                        IdUser = idUser,
                                        User = name,
                                        IdCustomer = _dataClient.IdClient,
                                        

                                    };
                                    _context.Add(reports);
                                    _context.SaveChanges();

                                    if (listInterests.Count > 0)
                                    {
                                        using(var dbContext = new ApplicationDbContext())
                                        {
                                            for (int i = 0; i < Input.AmountFees; i++)
                                            {
                                                var interest1 = listInterests[i];
                                                interest1.Canceled = true;
                                                dbContext.Update(interest1);
                                                dbContext.SaveChanges();
                                            }
                                        }
                                    }
                                    listInterests.Clear();
                                    listInterests = _context.TCustomer_interests.Where(c => c.IdCustomer.Equals(_dataClient.IdClient) && c.Canceled.Equals(false)).ToList();
                                    if(listInterests.Count > 0)
                                    {
                                        var report = new TCustomer_interests_reports
                                        {
                                            IdinterestReports = interest.IdinterestReports,
                                            Interests = amountFees,
                                            Payment = Input.Payment,
                                            Change = changes,
                                            Fee = Input.AmountFees,
                                            InterestDate = dateNow,
                                            TicketInterest = ticket,
                                            IdClient = _dataClient.IdClient,
                                        };
                                        _context.Update(report);
                                        _context.SaveChanges();
                                    }
                                    else
                                    {
                                        var report = new TCustomer_interests_reports
                                        {
                                            IdinterestReports = interest.IdinterestReports,
                                            Interests = 0.0m,
                                            Payment = 0.0m,
                                            Change = 0.0m,
                                            Fee = 0,
                                            InterestDate = dateNow,
                                            TicketInterest = "00000000000",
                                            IdClient = _dataClient.IdClient,
                                        };
                                        _context.Update(report);
                                        _context.SaveChanges();
                                    }

                                    var _payment = string.Format("{0:#,###,###,##0.00####}", Input.Payment);
                                    var _interest = string.Format("{0:#,###,###,##0.00####}", amountFees);
                                    var _changes = string.Format("{0:#,###,###,##0.00####}", changes);
                                    
                                    LTicket Ticket1 = new LTicket();
                                    Ticket1.AbreCajon(); //Abre el cajon
                                    Ticket1.TextoCentro("Sistema de Ventas"); //Imprime en el centro
                                    Ticket1.TextoIzquierda("Direccion");
                                    Ticket1.TextoIzquierda("Recoleta, CABA");
                                    Ticket1.TextoIzquierda("Tel 2364546014");
                                    Ticket1.LineasGuion();
                                    Ticket1.TextoCentro("FACTURA"); //Imprime en el centro
                                    Ticket1.LineasGuion();
                                    Ticket1.TextoIzquierda($"Factura: {ticket}");
                                    Ticket1.TextoIzquierda($"Cliente: {_nameCliente}");
                                    Ticket1.TextoIzquierda($"Fecha: {nowDate}");
                                    Ticket1.TextoIzquierda($"Usuario: {name}");
                                    Ticket1.LineasGuion();
                                    Ticket1.TextoCentro($"Intereses{Money}{_interest}");
                                    Ticket1.TextoExtremo($"Pago:", $"{Money}{_payment}");
                                    Ticket1.TextoExtremo($"Cambio:", $"{Money}{_changes}");
                                    Ticket1.TextoCentro("Juan Matias Tulli");
                                    Ticket1.CortaTicket(); //Corta el ticket

                                    Ticket1.ImprimirTicket("Microsoft XPS Document Writer");


                                    transaction.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                _errorMessage = ex.Message;
                                transaction.Rollback();
                            }
                        }
                     });
                     break;
            }
            return Redirect("/Customers/Reports?id=" + idClient);
        }
    }
}
