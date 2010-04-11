using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;

namespace WPFCalculator.Contracts {
	[InheritedExport]
	public interface IVisualCalculator
	{
		IList<IOperation> Operations { get;}
		FrameworkElement Operate(IOperation op, double[] operands);
		string Name { get; }
	}
}