using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CollisisionEditor2
{
 /*   class DesignerItemAdorner : Adorner
    {
        private VisualCollection visuals;
        private DesignerItemAdornerChrome chrome;

        protected override int VisualChildrenCount
        {
            get
            {
                return this.visuals.Count;
            }
        }

        public DesignerItemAdorner(ContentControl designerItem)
            : base(designerItem)
        {
            this.chrome = new DesignerItemAdornerChrome();
            this.chrome.DataContext = designerItem;
            this.visuals = new VisualCollection(this);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            this.chrome.Arrange(new Rect(arrangeBounds));
            return arrangeBounds;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visuals[index];
        }
    } */
}
