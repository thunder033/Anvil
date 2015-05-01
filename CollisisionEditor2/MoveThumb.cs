using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CollisisionEditor2
{
	public class MoveThumb : Thumb
	{
		public MoveThumb()
		{
			DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
		}

		private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			Control item = this.DataContext as Control;

			if (item != null)
			{
				double left = Canvas.GetLeft(item);
				double top = Canvas.GetTop(item);

                double newLeft = left + e.HorizontalChange;
                if (newLeft < 0) newLeft = 0;
                if (newLeft + item.ActualWidth > MainWindow.textureCanvas.ActualWidth) newLeft = MainWindow.textureCanvas.ActualWidth - item.ActualWidth;

                double newTop = top + e.VerticalChange;
                if (newTop < 0) newTop = 0;
                if (newTop + item.ActualHeight > MainWindow.textureCanvas.ActualHeight) newTop = MainWindow.textureCanvas.ActualHeight - item.ActualHeight;

				Canvas.SetLeft(item, newLeft);
				Canvas.SetTop(item, newTop);
			}
		}
	}
}
