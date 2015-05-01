using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CollisisionEditor2
{
	public enum PolygonType
	{
		Normal,
		Base,
		Sharp,
		Platform
	}

	public class Polygon : INotifyPropertyChanged
    {

        public Rect rectangle;
        public ContentControl controller;

		public bool selected { get; set; }

		private PolygonType _type;
		public PolygonType type {
			get
			{
				return _type;
			}
			set{
				_type = value;
				if (parentCol != null)
				{
					int thisCtrHash = this.controller.GetHashCode();
					for (int i = 0; i < parentCol.Count; i++)
					{
						int ctrlHash = parentCol[i].controller.GetHashCode();
						if (ctrlHash == thisCtrHash)
						{
							parentCol[i] = this;
						}
					}
						
				}
				if(PropertyChanged != null)
				{
					PropertyChanged(this, new PropertyChangedEventArgs("type"));
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public string dimensions
		{
			get
			{
				int x = (int)Canvas.GetLeft(controller);
				int y = (int)Canvas.GetTop(controller);
				int width = (int)controller.ActualWidth;
				int height = (int)controller.ActualHeight;
				return String.Format("({0},{1}) {2}×{3}",x,y,width,height);
			}
		}

		ObservableCollection<Polygon> parentCol;

		public Polygon(Rect aRect, ContentControl aController, ObservableCollection<Polygon> collection = null)
        {
            rectangle = aRect;
            controller = aController;
			selected = false;
			type = 0;
			parentCol = collection;
        }
    }
}
