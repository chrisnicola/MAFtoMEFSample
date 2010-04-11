using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn;
using WPFCalculator.Contracts;

namespace BasicStackOperations
{
    public class BasicStackOps : ICalculator
    {
        double _current;
        List<IOperation> _ops;
        Random _r;

        public BasicStackOps()
        {
            _current = 0;
						_r = new Random();  
					_ops = new List<IOperation> {
					                            	new Operation("Pop", 2),
					                            	new Operation("Push Next", 0),
					                            	new Operation("Push Random", 0),
					                            	new Operation("Push Random Up To", 1)
					                            };
        }

        public string Name
        {
            get { return "Stack Operations"; }
        }

        public IList<IOperation> Operations
        {
            get { return _ops; }
        }

        public double Operate(IOperation op, double[] operands)
        {
            
            switch (op.Name)
            {
                case "Pop":
                    return operands[1];
                case "Push Next":
                    _current++;
                    return _current;
                case "Push Random":
                    return _r.Next(100);
                case "Push Random Up To":
                    return _r.Next((int)operands[0]);
                default:
                    throw new NotSupportedException("Can not support operation" + op.Name);
            }
        }
    }
}
