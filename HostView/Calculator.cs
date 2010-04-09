using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using WPFCalculator.Contracts;

namespace HostView
{
    public abstract class CalculatorBase
    {
        public abstract String Name
        {
            get;
        }

        public abstract IList<IOperation> Operations
        {
            get;
        }
       
    }

    public abstract class Calculator : CalculatorBase, ICalculator
    {
       

        public abstract double Operate(IOperation op, double[] operands);
    }

    public abstract class VisualCalculator : CalculatorBase
    {

        public abstract FrameworkElement Operate(IOperation op, double[] operands);
    }

    public abstract class Operation : IOperation
    {
        public abstract string Name
        {
            get;            
        }

        public abstract int NumOperands
        {
            get;
        }
    }
}
