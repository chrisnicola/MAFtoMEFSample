using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using WPFCalculator.Contracts;

[assembly: XmlnsDefinition("http://myapplication", "DemoApplication")]

namespace DemoApplication {
	/// <summary>
	/// Interaction logic for ActionLayout.xaml
	/// </summary>
	public partial class ActionLayout : UserControl {
		private ICalculator _calc;
		private CalculatorHost _app;
		private EventHandler<StackChangedEventArgs> _stackChangedHandler;

		public ActionLayout() {
			this.InitializeComponent();
			_stackChangedHandler = new EventHandler<StackChangedEventArgs>(application_StackChanged);
		}

		public ICalculator Calculator {
			get { return _calc; }
			set {
				_calc = value;
				if (_calc == null)
					_app.StackChanged -= _stackChangedHandler;
			}
		}

		public ActionLayout(ICalculator calc, CalculatorHost application)
			: this()
		{
			_calc = calc;
			_app = application;
			this.Title.Text = calc.Name;
			foreach (Operation op in calc.Operations){
				var b = new Button {Tag = op, Content = op.Name};
				b.Click += new RoutedEventHandler(b_Click);
				Actions.Children.Add(b);
			}
			ValidateButtons(application.Stack.Items.Count);
			application.StackChanged += _stackChangedHandler;
		}

		private void application_StackChanged(object sender, StackChangedEventArgs e) { ValidateButtons(e.Count); }

		private void ValidateButtons(int numOperands) {
			foreach (Object obj in Actions.Children){
				Button b = (Button) obj;
				Operation op = (Operation) b.Tag;
				if (numOperands >= op.NumOperands)
					b.IsEnabled = true;
				else
					b.IsEnabled = false;
			}
		}

		private void b_Click(object sender, RoutedEventArgs e) {
			var b = (Button) sender;
			var op = (Operation) b.Tag;
			_app.Operate(_calc, op);
		}
	}
}