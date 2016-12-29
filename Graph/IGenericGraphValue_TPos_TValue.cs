using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graph
{
	public interface IGenericGraphValue<TPos, TValue>
	{
		TValue Value { get; set; }

		TPos Position { get; set; }

	} 
}

