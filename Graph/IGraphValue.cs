using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Graph
{
	public interface IGraphValue
	{
		decimal DisplayPosition { get; }

		decimal DisplayValue { get; }

	}

	public interface IGraphValueList : IList<IGraphValue>
	{
		void Sort();
		bool DrawGraph { get; set; }

		double GraphAlpha { get; set; }
		bool DrawPlane { get; set; }
		double PlaneAlpha { get; set; }
		float LineThickness { get; set; }
		Color Color { get; set; }
	}

	public class GraphValueComparer : IComparer<IGraphValue>
	{
		#region IComparer<IGraphValue> Member

		public int Compare(IGraphValue x, IGraphValue y)
		{
			decimal erg = (x.DisplayPosition - y.DisplayPosition);
			if (erg > 0)
			{
				return 1;
			}
			else if (erg < 0)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}

		#endregion
	} 
}

