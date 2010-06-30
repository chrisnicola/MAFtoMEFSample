using System;
using System.ComponentModel.Composition;

namespace WPFCalculator.Contracts
{
	[MetadataAttribute, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CalculatorAttribute : ExportAttribute
	{
		public CalculatorAttribute(string name) : base(typeof (ICalculatorPlugin))
		{
		  Name = name;
		}

	  public string Name { get; set; }
	}
}