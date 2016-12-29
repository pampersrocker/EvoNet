using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graph
{
	public class GraphTimeDoubleValue : IGenericGraphValue<DateTime, double>, IGraphValue
	{
		double value;
		DateTime position;

		public GraphTimeDoubleValue()
		{
			value = 0;
			position = new DateTime();
		}

		public GraphTimeDoubleValue(DateTime position, double value)
		{
			this.position = position;
			this.value = value;
		}

		public override string ToString()
		{
			return "{ Pos=" + DisplayPosition.ToString() + "; Value=" + DisplayValue.ToString() + "}";
		}

		#region IGenericGraphValue<DateTime,double> Member

		public double Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		public DateTime Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}

		#endregion

		#region IGraphValue Member

		public decimal DisplayPosition
		{
			get
			{
				return position.Ticks;
			}
		}

		public decimal DisplayValue
		{
			get
			{
				return (decimal)value;
			}
		}

		#endregion


	}

}
