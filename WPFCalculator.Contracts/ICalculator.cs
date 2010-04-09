using System;
using System.Collections.Generic;

namespace WPFCalculator.Contracts {
	public interface ICalculator
	{
		IList<IOperation> Operations { get;}
		double Operate(IOperation op, double[] operands);
		string Name { get;}
	}
}