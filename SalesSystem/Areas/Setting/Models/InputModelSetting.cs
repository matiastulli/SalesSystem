using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Areas.Setting.Models
{
    public class InputModelSetting
    {
        [Required(ErrorMessage = "Seleccione una opcion.")]
        public int RadioOptiones { get; set; }
        public string ErrorMessage { get; set; }
        public string Type_Money { get; set; }
        [Required(ErrorMessage = "Ingrese los intereses.")]
        public Decimal? Interests { get; set; } = null;
        public string FormatInterests { get; set; }
    }
}
