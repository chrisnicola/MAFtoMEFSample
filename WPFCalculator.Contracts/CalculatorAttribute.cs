using System;
using System.ComponentModel.Composition;

namespace WPFCalculator.Contracts
{
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CalculatorAttribute : ExportAttribute
	{
		public CalculatorAttribute() : base(typeof (ICalculator)) { }

		public string Name { get; set; }
	}
}