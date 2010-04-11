using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace WPFCalculator.Contracts {
	[InheritedExport]
	public interface ICalculator
	{
		IList<IOperation> Operations { get;}
		double Operate(IOperation op, double[] operands);
		string Name { get;}
	}
}