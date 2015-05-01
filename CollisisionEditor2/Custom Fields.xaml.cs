using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CollisisionEditor2
{
    /// <summary>
    /// Interaction logic for Custom_Fields.xaml
    /// </summary>
    public partial class Custom_Fields : Window
    {
        public Custom_Fields()
        {
            InitializeComponent();

            DataContext = this;
            fields.ItemsSource = MainWindow.customFields;

            return;
            for (int f = 0; f < Settings.customFields.Length; f++)
            {
                Label label = new Label();
                label.Content = Settings.customFields[f];
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Top;
                label.Margin = new Thickness(20, 40 * (f + 1), 0, 0);

                Content.Children.Add(label);

                TextBox textBox = new TextBox();
                textBox.HorizontalAlignment = HorizontalAlignment.Left;
                textBox.VerticalAlignment = VerticalAlignment.Top;
                textBox.Margin = new Thickness(0, 40 * (f + 1), 0, 0);
                textBox.Width = 200;
                Grid.SetColumn(textBox, 1);

                Binding text = new Binding();
                text.Source = this;
                text.Path = new PropertyPath(f.ToString());
                textBox.SetBinding(TextBox.TextProperty, text);

                Content.Children.Add(textBox);
            }
        }

        //http://stackoverflow.com/questions/13561171/find-all-controls-inside-wpf-listbox
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
			    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if(child != null && child is T)
                {
                    yield return (T)child;
                }
                else 
                {
                    var childOfChild = FindVisualChildren<T>(child);
                    if(childOfChild != null)
                    {
                        foreach(var subchild in childOfChild)
                        {
                            yield return subchild;
                        }
                    }

                } 
			}
        }
    }
}
