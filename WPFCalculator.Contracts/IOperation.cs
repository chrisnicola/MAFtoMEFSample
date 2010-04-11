using System.ComponentModel.Composition;

namespace WPFCalculator.Contracts {
	[InheritedExport]
	public interface IOperation
	{
		string Name { get;}
		int NumOperands { get;}
	}
}