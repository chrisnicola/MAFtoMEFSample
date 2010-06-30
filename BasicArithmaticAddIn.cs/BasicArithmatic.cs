using System;
using System.Collections.Generic;
using WPFCalculator.Contracts;

namespace BasicArithmaticAddIn.cs
{
  [Calculator("Basic Arithmetic")]
	public class BasicArithmatic : INumericCalculator
	{
		private readonly IList<IOperation> _operations;

		public BasicArithmatic()
		{
			_operations = new List<IOperation>
			              	{
			              		new Operation("+", 2),
			              		new Operation("-", 2),
			              		new Operation("/", 2),
			              		new Operation("*", 2)
			              	};
		}

		#region ICalculator Members

		public string Name
		{
			get { return "Basic Arithmatic"; }
		}

    public IList<IOperation> Operations
		{
			get { return _operations; }
		}

		public double Operate(IOperation op, double[] operands)
		{
			switch (op.Name)
			{
				case "+":
					return operands[0] + operands[1];
				case "-":
					return operands[0] - operands[1];
				case "*":
					return operands[0]*operands[1];
				case "/":
					return operands[0]/operands[1];
				default:
					throw new InvalidOperationException("Can not perform operation: " + op.Name);
			}
		}

		#endregion
	}
}