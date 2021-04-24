using SalesSystem.Areas.Setting.Models;
using SalesSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Library
{
    public class LSetting : ListObject
    {
        public static string Moneda { get; set; }
        public static Decimal Interests { get; set; }

        public List<TSetting> setting;

        public static string _errorMessage = null;

        public LSetting(ApplicationDbContext context)
        {
            _context = context;
            GetSetting();
        }
        public async Task<string> TypeMoneyAsync(int radioOptions)
        {
            try
            {
                using (var dbContext = new ApplicationDbContext())
                {
                    setting = dbContext.TSetting.ToList();
                }
                if (setting.Count.Equals(0))
                {
                    Moneda = "ARS.";
                    var data = new TSetting
                    {
                        TypeMoney = "ARS.",
                        Interests = 0.0m,
                    };

                    await _context.AddAsync(data);
                    _context.SaveChanges();
                }
                else
                {
                    var data = setting.Last();
                    switch (radioOptions)
                    {
                        case 1:
                            Moneda = "ARS";
                            break;
                        case 2:
                            Moneda = "$";
                            break;
                    }
                    using(var dbContext = new ApplicationDbContext())
                    {
                        var data1 = new TSetting
                        {
                            ID = data.ID,
                            TypeMoney = Moneda,
                            Interests = data.Interests,
                        };

                        dbContext.Update(data1);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            return _errorMessage;
        }

        public void GetSetting()
        {
            var setting = _context.TSetting.ToList();
            if (!setting.Count.Equals(0))
            {
                var data = setting.Last();
                Moneda = data.TypeMoney;
                Interests = data.Interests;
            }
        }

        public async Task<string> SetInterests (Decimal? interests)
        {
            try
            {
                using(var dbContext = new ApplicationDbContext())
                {
                    setting = dbContext.TSetting.ToList();
                }
                if (setting.Count.Equals(0))
                {
                    Moneda = "ARS";
                    var data = new TSetting
                    {
                        TypeMoney = "ARS.",
                        Interests = (Decimal)interests,
                    };

                    await _context.AddAsync(data);
                    _context.SaveChanges();
                }
                else
                {
                    var data = setting.Last();
                    using (var dbContext = new ApplicationDbContext())
                    {
                        var data1 = new TSetting
                        {
                            ID = data.ID,
                            TypeMoney = data.TypeMoney,
                            Interests = (Decimal)interests,
                        };

                        dbContext.Update(data1);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
            }
            return _errorMessage;
        }
    }
}
