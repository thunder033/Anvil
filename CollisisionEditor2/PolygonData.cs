using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CollisisionEditor2
{
	public class PolygonData
	{
		public PolygonType type;
		public Rect rect;

		public PolygonData()
		{
			type = 0;
			rect = new Rect();
		}

		public PolygonData(double aX, double aY, double aWidth, double aHeight, PolygonType aType)
		{
			type = aType;
			rect = new Rect(aX, aY, aWidth, aHeight);
		}
	}
}
