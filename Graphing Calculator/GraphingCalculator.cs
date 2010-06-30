using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WPFCalculator.Contracts;

namespace GraphCalc
{
  [Calculator("Graphing Calculator")]
  public class GraphingCalculator : IVisualCalculator
  {
    private readonly IList<IOperation> _ops;
    private Grapher _grapher;
    private List<byte[]> _leaks;

    public GraphingCalculator()
    {
      _ops = new List<IOperation>
               {
                 new Operation("2D Graph", 0),
                 new Operation("2D Parametric Graph", 0),
                 new Operation("3D Parametric Graph", 0)
               };
      _grapher = new Grapher();
      _leaks = new List<byte[]>();
    }

    #region IVisualCalculator Members

    public string Name
    {
      get { return "Graphing Calculator"; }
    }

    public IList<IOperation> Operations
    {
      get { return _ops; }
    }


    public FrameworkElement Operate(IOperation op, double[] operands)
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
          var t = new TextBox();
          t.Text = "Hello there";
          return t;
      }
    }

    #endregion

    ~GraphingCalculator()
    {
    }
  }
}