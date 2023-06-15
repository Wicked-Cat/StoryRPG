using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;


namespace StoryRPG.Converters
{
    class TimeOfDayToColourConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cuture)
        {
            
            switch (value.ToString().ToLower())
            {
                case "day":
                    return Brushes.Beige.Color;
                    break;
                case "dawn":
                    return Brushes.Plum.Color;
                    break;
                case "night":
                    return Brushes.RoyalBlue.Color;
                    break;
                case "dusk":
                    return Brushes.LightGoldenrodYellow.Color;
                    break;
            }
            return Brushes.Beige.Color;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cuture)
        {
            throw new NotImplementedException();
        }
        
    }
}

