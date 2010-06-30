using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;

namespace WPFCalculator.Contracts {
	[InheritedExport]
	public interface IVisualCalculator : ICalculatorPlugin
	{
		FrameworkElement Operate(IOperation op, double[] operands);
	}
}