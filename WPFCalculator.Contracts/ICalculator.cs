using System.Collections.Generic;

namespace WPFCalculator.Contracts
{
  public interface ICalculatorPlugin
  {
    IList<IOperation> Operations { get; }
    string Name { get; }
  }
}