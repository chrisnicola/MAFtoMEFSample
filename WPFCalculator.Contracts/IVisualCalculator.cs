using System.Collections.Generic;
using System.Windows;

namespace WPFCalculator.Contracts {
	public interface IVisualCalculator
	{
		IList<IOperation> Operations { get;}
		FrameworkElement Operate(IOperation op, double[] operands);
		string Name { get; }
	}
}