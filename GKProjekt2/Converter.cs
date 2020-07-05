using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GKProjekt2
{
    public class FillingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            if (Enum.TryParse<FillingMode>(param, out FillingMode result) == true && value is FillingMode fm)
            {
                if (result == fm)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool b)
            {
                if(b == true)
                {
                    if (Enum.TryParse<FillingMode>(parameter as string, out FillingMode result) == true)
                    {
                        return result;
                    }
                }
            }
            return Binding.DoNothing;
        }
    }

    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return new SolidColorBrush(c);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VectorNModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string param = parameter as string;
            if (Enum.TryParse<VectorNMode>(param, out VectorNMode result) == true && value is VectorNMode fm)
            {
                if (result == fm)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (b == true)
                {
                    if (Enum.TryParse<VectorNMode>(parameter as string, out VectorNMode result) == true)
                    {
                        return result;
                    }
                }
            }
            return Binding.DoNothing;
        }
    }
}
