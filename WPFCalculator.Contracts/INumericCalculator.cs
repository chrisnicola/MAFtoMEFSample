namespace WPFCalculator.Contracts
{
  public interface INumericCalculator : ICalculatorPlugin
  {
    double Operate(IOperation op, double[] operands);
  }
}