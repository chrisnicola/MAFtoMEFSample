using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Pipeline;
using System.Windows;
using WPFCalculator.Contracts;

[assembly:System.Security.AllowPartiallyTrustedCallers]
namespace AddInView
{
    [AddInBase]
    public abstract class Calculator : ICalculator
    {
        public abstract String Name
        {
            get;
        }

        public abstract IList<IOperation> Operations
        {
            get;
        }

        public abstract double Operate(IOperation op, double[] operands);

    }

    

    [AddInBase]
    public abstract class VisualCalculator
    {
        public abstract String Name
        {
            get;
        }

        public abstract IList<IOperation> Operations
        {
            get;
        }

        public abstract FrameworkElement Operate(IOperation op, double[] operands);
    }

    public class Operation : IOperation
    {
        String _name;
        int _numOperands;
        public Operation(String name, int numOperands)
        {
            _name = name;
            _numOperands = numOperands;
        }

        public String Name
        {
            get
            {
                return _name;
            }
        }

        public int NumOperands
        {
            get
            {
                return _numOperands;
            }
        }
    }
}
