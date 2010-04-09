using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFCalculator.Contracts;

namespace BasicArithmaticAddIn.cs
{
    [System.AddIn.AddIn("Basic Arithmatic")]
    public class BasicArithmatic :AddInView.Calculator
    {
        private IList<IOperation> _operations;

        public BasicArithmatic()
        {
            _operations = new List<IOperation> {
                                               	new AddInView.Operation("+", 2),
                                               	new AddInView.Operation("-", 2),
                                               	new AddInView.Operation("/", 2),
                                               	new AddInView.Operation("*", 2)
                                               };
        }

        public override string Name
        {
            get { return "Basic Arithmatic"; }
        }

        public override IList<IOperation> Operations
        {
            get { return _operations; }
        }

        public override double Operate(IOperation op, double[] operands)
        {
            switch (op.Name)
            {
                case "+":
                    return operands[0] + operands[1];
                case "-":
                    return operands[0] - operands[1];
                case "*":
                    return operands[0] * operands[1];
                case "/":
                    return operands[0] / operands[1];
                default:
                    throw new InvalidOperationException("Can not perform operation: " +op.Name);
            }
        }
    }
}
