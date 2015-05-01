using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CollisisionEditor2
{
    public class AnimationProperty //: DependencyObject
    {
        public string Property { get; set; }
        public string Value { get; set; }

        public AnimationProperty(string aKey = "", string aValue = "")
        {
            Property = aKey;
            Value = aValue;
        }

        //public static readonly DependencyProperty PropertyName = DependencyProperty.Register("Property", typeof(string), typeof(AnimationProperty), new UIPropertyMetadata(null));
    }
}
