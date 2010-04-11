using System;
using System.AddIn.Hosting;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using WPFCalculator.Contracts;

[assembly: XmlnsDefinition("http://foo", "DemoApplication")]

namespace DemoApplication
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class CalculatorHost : Window
	{
		[ImportMany]
		private IList<ICalculator> _calcs = new List<ICalculator>();

		[ImportMany]
		private IEnumerable<ICalculator> _calc2;

		private MemoryStatusDisplay _memDisp;
		private Timer _timer;
		private List<AppDomain> _toUnload;

		public CalculatorHost()
		{
			Application.App.Exit += new ExitEventHandler(App_Exit);
			InitializeComponent();
			LoadAddIns();
			Input.KeyUp += new KeyEventHandler(Input_KeyUp);
			_toUnload = new List<AppDomain>();
			Stack.SelectionChanged += new SelectionChangedEventHandler(Stack_SelectionChanged);
			Actions.SelectionChanged += new SelectionChangedEventHandler(Actions_SelectionChanged);
			UpdateStack();
			CalculatorVisual.SizeChanged += new SizeChangedEventHandler(CalculatorVisual_SizeChanged);
			RefreshAddIns.Click += new RoutedEventHandler(RefreshAddIns_Click);
			PerformGC.Click += new RoutedEventHandler(PerformGC_Click);
			ShowMemUse.Click += new RoutedEventHandler(ShowMemUse_Click);
			IsVisibleChanged += new DependencyPropertyChangedEventHandler(CalculatorHost_IsVisibleChanged);
		}

		private void App_Exit(object sender, ExitEventArgs e)
		{
			UnloadAllAddIns();
			GC.WaitForPendingFinalizers();
		}


		private void Actions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Actions.SelectedItem = null;
		}

		private void CalculatorHost_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (_memDisp != null && _memDisp.IsActive)
			{
				_memDisp.Close();
			}
		}

		private void CalculatorHost_Closed(object sender, EventArgs e)
		{
			if (_memDisp != null && _memDisp.IsActive)
			{
				_memDisp.Close();
			}
		}

		private void ShowMemUse_Click(object sender, RoutedEventArgs e)
		{
			if (_memDisp == null)
			{
				_memDisp = new MemoryStatusDisplay();
				_memDisp.Closed += new EventHandler(_memDisp_Closed);
			}
			_memDisp.Show();
			var timer = new Timer();
			timer.Interval = 500;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			timer_Elapsed(null, null);
			timer.Start();
			_timer = timer;
		}

		private void _memDisp_Closed(object sender, EventArgs e)
		{
			_timer.Stop();
			_memDisp = null;
			_timer = null;
		}

		private void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (_memDisp != null && _memDisp.IsVisible)
			{
				_memDisp.UpdateUsage(GC.GetTotalMemory(false).ToString());
			}
		}

		private void PerformGC_Click(object sender, RoutedEventArgs e)
		{
			PerformFullGC();
		}


		private void RefreshAddIns_Click(object sender, RoutedEventArgs e)
		{
			var calcs = new List<ICalculator>();
			calcs.AddRange(_calcs);
			foreach (ICalculator calc in calcs)
			{
				UnloadAddIn(calc);
			}
			AddInMenuList.Items.Clear();
			LoadAddIns();
		}

		private void CalculatorVisual_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		private void UnloadAllAddIns()
		{
			//Actions.Children.Clear();
			Actions.Items.Clear();
			_calcs.Clear();
			CalculatorVisual.Children.Clear();
		}

		private void PerformFullGC()
		{
			double start = GC.GetTotalMemory(false);
			try
			{
				foreach (AppDomain domain in _toUnload)
				{
					AppDomain.Unload(domain);
				}
			}
			catch (AppDomainUnloadedException e)
			{
			}
			_toUnload.Clear();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			double end = GC.GetTotalMemory(false);
			double diff = end - start;
		}

		private void Stack_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Stack.SelectedItem = null;
		}

		private void UpdateStack()
		{
			StackChanged.Invoke(this, new StackChangedEventArgs(Stack.Items.Count));
		}


		private void Input_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					Push();
					Input.Text = "";
					return;
			}
		}

		internal void Push()
		{
			double num;
			if (double.TryParse(Input.Text, out num))
			{
				var item = new ListViewItem();
				item.Content = num;
				Stack.Items.Insert(0, item);
				UpdateStack();
			}
		}

		internal event EventHandler<StackChangedEventArgs> StackChanged;

		internal void LoadAddIns()
		{
//			_calcs = new List<ICalculator>();
//			String path = Environment.CurrentDirectory;
//			AddInStore.Rebuild(path);
//			IList<AddInToken> tokens = AddInStore.FindAddIns(typeof (ICalculator), path);
//			IList<AddInToken> visualTokens = AddInStore.FindAddIns(typeof (IVisualCalculator), path);
//			foreach (AddInToken token in tokens)
//			{
//				_calcs.Add(token.Activate<ICalculator>(AddInSecurityLevel.FullTrust));
//			}
//			foreach (AddInToken token in visualTokens)
//			{
//				_calcs.Add(token.Activate<ICalculator>(AddInSecurityLevel.FullTrust));
//			}
//			Actions.Items.Clear();
			var catalog = new DirectoryCatalog(".\\addins");
			var container = new CompositionContainer();
			container.ComposeParts(this);
			
			foreach (ICalculator calc in _calcs)
			{
				InitAddIn(calc, true);
			}
		}

		private void InitAddIn(ICalculator calc, bool addMenu)
		{
			Actions.Items.Add(new ActionLayout(calc, this));
			if (addMenu)
			{
				var cb = new CheckBox();
				cb.Content = calc.Name;
				cb.IsChecked = true;
				cb.Tag = calc;
				cb.Checked += new RoutedEventHandler(cb_Checked);
				cb.Unchecked += new RoutedEventHandler(cb_Unchecked);
				AddInMenuList.Items.Add(cb);
			}
		}

		private void cb_Unchecked(object sender, RoutedEventArgs e)
		{
			var calc = (ICalculator) ((CheckBox) sender).Tag;
			AddInController controller = AddInController.GetAddInController(calc);
			AddInToken token = controller.Token;
			((CheckBox) sender).Tag = token;
			UnloadAddIn(calc);
			calc = null;
			_toUnload.Add(controller.AppDomain);
		}

		private void cb_Checked(object sender, RoutedEventArgs e)
		{
			var token = (AddInToken) ((FrameworkElement) sender).Tag;
			var calc = token.Activate<ICalculator>(AddInSecurityLevel.Internet);
			_calcs.Add(calc);
			((FrameworkElement) sender).Tag = calc;
			InitAddIn(calc, false);
		}

		private void UnloadAddIn(ICalculator calc)
		{
			ActionLayout calcsAl = null;
			foreach (ActionLayout al in Actions.Items)
			{
				if (al.Calculator.Equals(calc))
				{
					calcsAl = al;
					break;
				}
			}
			calcsAl.Calculator = null;
			Actions.Items.Remove(calcsAl);
			_calcs.Remove(calc);
			CalculatorVisual.Children.Clear();
		}

		internal void Operate(ICalculator calc, Operation op)
		{
			var operands = new double[op.NumOperands];
			for (int i = 0; i < op.NumOperands; i++)
			{
				var item = (ListViewItem) Stack.Items[0];
				operands[i] = double.Parse(item.Content.ToString());
				Stack.Items.RemoveAt(0);
			}
			if (typeof (ICalculator).IsAssignableFrom(calc.GetType()))
			{
				double result = ((ICalculator) calc).Operate(op, operands);
				var newItem = new ListViewItem();
				newItem.Content = result;
				Stack.Items.Insert(0, newItem);
			}
			else
			{
				UIElement visual = ((IVisualCalculator) calc).Operate(op, operands);
				CalculatorVisual.Children.Clear();
				CalculatorVisual.Children.Add(visual);
			}
			UpdateStack();
		}
	}

	internal class StackChangedEventArgs : EventArgs
	{
		private int _count;

		public StackChangedEventArgs(int count)
		{
			_count = count;
		}

		public int Count
		{
			get { return _count; }
		}
	}
}