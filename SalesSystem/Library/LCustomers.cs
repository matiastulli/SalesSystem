using Microsoft.AspNetCore.Http;
using SalesSystem.Areas.Customers.Models;
using SalesSystem.Data;
using SalesSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Library
{
    public class LCustomers : ListObject
    {
        public LCustomers(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<InputModelRegister> getTClients(String valor, int id)
        {
            List<TClients> listTClients;
            var clientsList = new List<InputModelRegister>();
            if (valor == null && id.Equals(0))
            {
                listTClients = _context.TClients.ToList();
            }
            else
            {
                if (id.Equals(0))
                {
                    listTClients = _context.TClients.Where(u => u.Nid.StartsWith(valor) || u.Name.StartsWith(valor) ||
              u.LastName.StartsWith(valor) || u.Email.StartsWith(valor)).ToList();
                }
                else
                {
                    listTClients = _context.TClients.Where(u => u.IdClient.Equals(id)).ToList();
                }
            }
            if (!listTClients.Count.Equals(0))
            {
                foreach (var item in listTClients)
                {
                    clientsList.Add(new InputModelRegister
                    {
                        IdClient = item.IdClient,
                        Nid = item.Nid,
                        Name = item.Name,
                        LastName = item.LastName,
                        Email = item.Email,
                        Phone = item.Phone,
                        Credit = item.Credit,
                        Direction = item.Direction,
                        Image = item.Image,
                    });
                }
            }
            return clientsList;
        }
        public List<TClients> getTClient(String Nid)
        {
            var listTClients = new List<TClients>();
            using (var dbContext = new ApplicationDbContext())
            {
                listTClients = dbContext.TClients.Where(u => u.Nid.Equals(Nid)).ToList();
            }
                
            return listTClients;
        }
        public InputModelRegister getTClientReport(int id)
        {
            var dataClients = new InputModelRegister();
            using (var dbContext = new ApplicationDbContext())
            {
                var query = dbContext.TClients.Join(dbContext.TReports_clients,
               c => c.IdClient, r => r.TClientsIdClient, (c, r) => new
               {
                   c.IdClient,
                   c.Nid,
                   c.Name,
                   c.LastName,
                   c.Phone,
                   c.Email,
                   c.Direction,
                   c.Credit,
                   r.IdReport,
                   r.Debt,
                   r.Monthly,
                   r.Change,
                   r.CurrentDebt,
                   r.DatePayment,
                   r.LastPayment,
                   r.Ticket,
                   r.Deadline,
               }).Where(c => c.IdClient.Equals(id)).ToList();
                if (!query.Count.Equals(0))
                {
                    var data = query.ToList().Last();
                    dataClients = new InputModelRegister
                    {
                        IdClient = data.IdClient,
                        Nid = data.Nid,
                        Name = data.Name,
                        LastName = data.LastName,
                        Phone = data.Phone,
                        Email = data.Email,
                        Direction = data.Direction,
                        Credit = data.Credit,
                        IdReport = data.IdReport,
                        Debt = data.Debt,
                        Monthly = data.Monthly,
                        Change = data.Change,
                        CurrentDebt = data.CurrentDebt,
                        DatePayment = data.DatePayment,
                        LastPayment = data.LastPayment,
                        Ticket = data.Ticket,
                        Deadline = data.Deadline,
                    };
                }
            }
            return dataClients;
        }

        public DataPaginador<TPayments_clients> GetPayments(int id, int page, int num, InputModelRegister input, HttpRequest request)
        {
            Object[] objects = new Object[3];
            var url = request.Scheme + "://" + request.Host.Value;
            var data = GetTPayments_Clients(input, id);
            if (0 < data.Count)
            {
                //Historial de pagos en reversa
                //De mayor a menor
                data.Reverse();
                objects = new LPaginador<TPayments_clients>().paginador(data, page, num, "Customers", "Customers", "Customers/Reports", url);
            }
            else
            {
                objects[0] = "No data";
                objects[1] = "No data";
                objects[2] = new List<TPayments_clients>();

            }
            var models = new DataPaginador<TPayments_clients>
            {
                List = (List<TPayments_clients>) objects[2],
                Pagi_info = (string)objects[0],
                Pagi_navegacion = (string)objects[1],
                Input = new TPayments_clients()
            };
            return models;
        
        }

        //VIDEO 71
        public List<TPayments_clients> GetTPayments_Clients(InputModelRegister input, int id)
        {
            var listTPayments = new List<TPayments_clients>();
            var listTPayments2 = new List<TPayments_clients>();

            /* Menos de cero: si t1 es anterior a t2.
             * Cero: si t1 es igual a t2.
             * Mayor que cero: si t1 es posterior a t2
            */
            var t1 = input.Time1.ToString("dd/MM/yyyy");
            var t2 = input.Time2.ToString("dd/MM/yyyy");

            if (t1.Equals(t2) && DateTime.Now.ToString("dd/MM/yyyy").Equals(t1) && DateTime.Now.ToString("dd/MM/yy").Equals(t2))
            {
                listTPayments2 = _context.TPayments_clients.Where(c => c.IdClient.Equals(id)).ToList();
            }
            else
            {
                foreach (var item in _context.TPayments_clients.Where(c => c.IdClient.Equals(id)).ToList())
                {
                    int fecha1 = DateTime.Compare(DateTime.Parse(
                                    item.Date.ToString("dd/MM/yyyy")), DateTime.Parse(t1));

                    if (fecha1.Equals(0) || fecha1 > 0)
                    {
                        listTPayments.Add(item);
                    }
                }
                foreach (var item in listTPayments)
                {
                    int fecha2 = DateTime.Compare(DateTime.Parse(
                                    item.Date.ToString("dd/MM/yyyy")), DateTime.Parse(t2));

                    if (fecha2.Equals(0) || fecha2 < 0)
                    {
                        listTPayments2.Add(item);
                    }
                }
            }
            return listTPayments2;
        }

        public InputModelRegister getTClientPayment(int idDebt)
        {
            var dataClients = new InputModelRegister();
            using (var dbContext = new ApplicationDbContext())
            {
                var query = dbContext.TPayments_clients.Join(dbContext.TClients,
                p => p.IdClient, c => c.IdClient, (p, c) => new
                {
                    c.IdClient,
                    c.Nid,
                    c.Name,
                    c.LastName,
                    c.Email,
                    c.Phone,
                    c.Direction,
                    c.Credit,
                    p.IdPayments,
                    p.Debt,
                    p.Payment,
                    p.Change,
                    p.CurrentDebt,
                    p.Date,
                    p.DeadLine,
                    p.DateDebt,
                    p.Monthly,
                    p.PreviousDebt,
                    p.Ticket,
                    p.IDUser,
                    p.User
                }).Where(c => c.IdPayments.Equals(idDebt)).ToList();
                if (!query.Count.Equals(0))
                {
                    var data = query.ToList().Last();
                    dataClients = new InputModelRegister
                    {
                        IdClient = data.IdClient,
                        Nid = data.Nid,
                        Name = data.Name,
                        LastName = data.LastName,
                        Email = data.Email,
                        Phone = data.Phone,
                        Direction = data.Direction,
                        Credit = data.Credit,
                        IdPayments = data.IdPayments,
                        Debt = data.Debt,
                        Payment = data.Payment,
                        Change = data.Change,
                        CurrentDebt = data.CurrentDebt,
                        Date = data.Date,
                        Ticket = data.Ticket,
                        Deadline = data.DeadLine,
                        DateDebt = data.DateDebt,
                        Monthly = data.Monthly,
                        PreviousDebt = data.PreviousDebt,
                        IdUser = data.IDUser,
                        User = data.User,
                          
                    };
                }
            }
            return dataClients;
        }

        public int _interestsCuotas = 0;
        private Decimal _interests;

        //VIDEO 76 cargo datos

        public InputModelInterests getTClientInterests(int id)
        {
            var dataInterests = new InputModelInterests();
            using (var dbContext = new ApplicationDbContext())
            {
                var query = dbContext.TCustomer_interests_reports.Where(c => c.IdClient.Equals(id)).ToList();
                var listInterests = dbContext.TCustomer_interests.Where(c => c.IdCustomer.Equals(id) && c.Canceled.Equals(false)).ToList();

                if (listInterests.Count.Equals(0))
                {
                    _interestsCuotas = 0;
                    _interests = 0.00m;
                }
                else
                {
                    _interestsCuotas = 0;
                    _interests = 0;
                    foreach (var item in listInterests)
                    {
                        _interests += item.Interests;
                        _interestsCuotas++;
                    }
                }
                var data = query.Count.Equals(0) ? new TCustomer_interests_reports() : query.ToList().Last();
                dataInterests = new InputModelInterests
                {
                    IdClient = data.IdClient,
                    IdinterestReports = data.IdinterestReports,
                    Interests = _interests,
                    Payment = data.Payment,
                    Change = data.Change,
                    Fee = _interestsCuotas,
                    InterestDate = data.InterestDate,
                    TicketInterest = data.TicketInterest,
                };
            }
            return dataInterests;
        }
        public string AmountFees(int fees, int idClient)
        {
            Decimal interests = 0;
            var listInterests = _context.TCustomer_interests.Where(c => c.IdCustomer.Equals(idClient) && c.Canceled.Equals(false)).ToList();

            if (!listInterests.Count.Equals(0))
            {
                if (listInterests.Count <= fees && fees <= listInterests.Count)
                {
                    for (int i = 0; i < fees; i++)
                    {
                        interests += listInterests[i].Interests;
                    }
                    return string.Format("{0:#,###,###,##0.00####}", interests);
                }
                else
                {
                    return "Se sobrepaso de las cuotas a pagar";
                }
            }
            else
            {
                return "El cliente no debe intereses";
            }
        }

        public DataPaginador<TPayments_Reports_Customer_Interest> GetInterests
            (int id, int page, int num, InputModelRegister input, HttpRequest request)
        {
            Object[] objects = new Object[3];
            var url = request.Scheme + "://" + request.Host.Value;
            var data = GetInterests_Clients(input, id);
            if (0 < data.Count)
            {
                //Historial de pagos en reversa
                //De mayor a menor
                data.Reverse();
                objects = new LPaginador<TPayments_Reports_Customer_Interest>().paginador(data, page, num, "Customers", "Customers", "Customers/Reports", url);
            }
            else
            {
                objects[0] = "No data";
                objects[1] = "No data";
                objects[2] = new List<TPayments_Reports_Customer_Interest>();

            }
            var models = new DataPaginador<TPayments_Reports_Customer_Interest>
            {
                List = (List<TPayments_Reports_Customer_Interest>)objects[2],
                Pagi_info = (string)objects[0],
                Pagi_navegacion = (string)objects[1],
                Input = new TPayments_Reports_Customer_Interest()
            };
            return models;
        }

        public List<TPayments_Reports_Customer_Interest> GetInterests_Clients(InputModelRegister input, int id)
        {
            var listTPayments = new List<TPayments_Reports_Customer_Interest>();
            var listTPayments2 = new List<TPayments_Reports_Customer_Interest>();

            /* Menos de cero: si t1 es anterior a t2.
             * Cero: si t1 es igual a t2.
             * Mayor que cero: si t1 es posterior a t2
            */
            var t1 = input.Time1.ToString("dd/MM/yyyy");
            var t2 = input.Time2.ToString("dd/MM/yyyy");

            if (t1.Equals(t2) && DateTime.Now.ToString("dd/MM/yyyy").Equals(t1) && DateTime.Now.ToString("dd/MM/yy").Equals(t2))
            {
                listTPayments2 = _context.TPayments_Reports_Customer_Interest.Where(c => c.IdCustomer.Equals(id)).ToList();
            }
            else
            {
                foreach (var item in _context.TPayments_Reports_Customer_Interest.Where(c => c.IdCustomer.Equals(id)).ToList())
                {
                    int fecha1 = DateTime.Compare(DateTime.Parse(
                                    item.Date.ToString("dd/MM/yyyy")), DateTime.Parse(t1));

                    if (fecha1.Equals(0) || fecha1 > 0)
                    {
                        listTPayments.Add(item);
                    }
                }
                foreach (var item in listTPayments)
                {
                    int fecha2 = DateTime.Compare(DateTime.Parse(
                                    item.Date.ToString("dd/MM/yyyy")), DateTime.Parse(t2));

                    if (fecha2.Equals(0) || fecha2 < 0)
                    {
                        listTPayments2.Add(item);
                    }
                }
            }
            return listTPayments2;
        }

    }
}
