using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using AddInView;
using System.AddIn;
using System.Windows.Controls;
using System.Threading;
using WPFCalculator.Contracts;

namespace GraphCalc
{
    [AddIn("Graphing Calculator")]
    public class GraphingCalculator : VisualCalculator
    {
        IList<IOperation> _ops;
        List<byte[]> _leaks;
        Grapher _grapher;

        public GraphingCalculator()
        {
            _ops = new List<IOperation> {
                                        	new Operation("2D Graph", 0),
                                        	new Operation("2D Parametric Graph", 0),
                                        	new Operation("3D Parametric Graph", 0)
                                        };
        	_grapher = new Grapher();
            _leaks = new List<byte[]>();
        }

        ~GraphingCalculator()
        {
        }

      
        public override string Name
        {
            get { return "Graphing Calculator"; }
        }

        public override IList<IOperation> Operations
        {
            get { return _ops; }
        }


        public override FrameworkElement Operate(IOperation op, double[] operands)
        {
           switch (op.Name)
            {
                
                case "2D Graph":
                    return new SceneInput2D();
                case "2D Parametric Graph":
                    return new SceneInput2dP();
                case "3D Parametric Graph":
                    return new SceneInput3D();
                default:
                    TextBox t = new TextBox();
                    t.Text = "Hello there";
                    return t;

            }
            
        }
    }
}
