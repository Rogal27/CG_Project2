using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GKProjekt2
{
    public class GridSizeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is string s)
            {
                if (int.TryParse(s, out int size) == true)
                {
                    if (size > 0)
                        return ValidationResult.ValidResult;
                    else
                        return new ValidationResult(false, "Value should be greater than 0!");
                }
            }
            return new ValidationResult(false, "Value should be integer!");
        }
    }
}
