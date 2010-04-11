namespace WPFCalculator.Contracts
{
	public class Operation : IOperation
	{
		private string _name;
		private readonly int _numOperands;

		public Operation(string name, int numOperands)
		{
			_name = name;
			_numOperands = numOperands;
		}

		public string Name
		{
			get { return _name; }
		}

		public int NumOperands
		{
			get { return _numOperands; }
		}
	}
}