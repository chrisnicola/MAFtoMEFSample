using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WPFCalculator.Contracts;

namespace BasicArithmaticAddIn.cs
{
    public class BasicVisual : IVisualCalculator
    {
        private IList<IOperation> _operations;

        public BasicVisual()
        {
            _operations = new List<IOperation> {new Operation("Graph", 5)};
        }

        public string Name
        {
            get { return "Basic Visual"; }
        }

        public IList<IOperation> Operations
        {
            get { return _operations; }
        }

        public FrameworkElement Operate(IOperation op, double[] operands)
        {
            switch (op.Name)
            {
                case "Graph":
                    return Graph(operands);
                default:
                    throw new NotSupportedException("Can not support operation: " + op.Name);
            }
        }

        private System.Windows.FrameworkElement Graph(double[] operands)
        {
            Grid g = new Grid();
            double max = operands[0];
            foreach (double d in operands)
            {
                max = Math.Max(max, d);
            }
            int rows = (int)max;
            int columns = operands.Length;
            for (int i = 0;i<columns;i++)
            {
                g.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int i = 0; i < rows; i++)
            {
                g.RowDefinitions.Add(new RowDefinition());
            }
            for (int c = 0; c <columns; c++)
            {
                for (int r = (int)operands[c]; r >= 0; r--)
                {
                    Canvas canvas = new Canvas();
                    System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush();
                    brush.Color = System.Windows.Media.Colors.Red;
                    canvas.Background = brush;
                    Grid.SetColumn(canvas, c);
                    Grid.SetRow(canvas, rows-r);
                    g.Children.Add(canvas);
                }
            }
            g.Width = 229;
            g.Height = 229;
            return g;
        }
    }
}
