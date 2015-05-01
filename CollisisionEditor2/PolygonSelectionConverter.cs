using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CollisisionEditor2
{
	public class PolygonSelectionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool selected = (bool)value;

			//for the background color
			if (targetType == typeof(SolidColorBrush) || targetType == typeof(Brush))
			{
				return (selected) ? new SolidColorBrush(Color.FromRgb(190, 205, 220)) : new SolidColorBrush(Colors.White);
			}
			//for other components of the list item (textbox)
			else if (targetType == typeof(Visibility))
			{
				return (selected) ? Visibility.Visible : Visibility.Hidden;
			}

			return selected;

			
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return null;
		}
	}
}
