using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
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
    private readonly List<AppDomain> _toUnload;
    [ImportMany] private IEnumerable<Lazy<ICalculatorPlugin, ICalculatorMetadata>> _calcs;
    private MemoryStatusDisplay _memDisp;
    private Timer _timer;

    public CalculatorHost()
    {
      Application.App.Exit += App_Exit;
      InitializeComponent();
      LoadAddIns();
      Input.KeyUp += Input_KeyUp;
      _toUnload = new List<AppDomain>();
      Stack.SelectionChanged += Stack_SelectionChanged;
      Actions.SelectionChanged += Actions_SelectionChanged;
      UpdateStack();
      CalculatorVisual.SizeChanged += CalculatorVisual_SizeChanged;
      RefreshAddIns.Click += RefreshAddIns_Click;
      PerformGC.Click += PerformGC_Click;
      ShowMemUse.Click += ShowMemUse_Click;
      IsVisibleChanged += CalculatorHost_IsVisibleChanged;
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
        _memDisp.Closed += _memDisp_Closed;
      }
      _memDisp.Show();
      var timer = new Timer();
      timer.Interval = 500;
      timer.Elapsed += timer_Elapsed;
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
      LoadAddIns();
    }

    private void CalculatorVisual_SizeChanged(object sender, SizeChangedEventArgs e)
    {
    }

    private void UnloadAllAddIns()
    {
      Actions.Items.Clear();
      _calcs = null;
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
      if (StackChanged != null)
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
      var catalog = new DirectoryCatalog(".\\addins");
      var container = new CompositionContainer(catalog);
      container.ComposeParts(this);
      foreach (var calc in _calcs)
      {
        InitAddIn(calc.Metadata.Name, true);
      }
    }

    private void InitAddIn(string name, bool addMenu)
    {
      if (AddInMenuList.Items.Cast<CheckBox>().Any(c => Equals(c.Content, name))) return;
      var cb = new CheckBox {Content = name};
      cb.Checked += cb_Checked;
      cb.Unchecked += cb_Unchecked;
      AddInMenuList.Items.Add(cb);
    }

    private void cb_Unchecked(object sender, RoutedEventArgs e)
    {
      var cb = (CheckBox) sender;
      UnloadAddIn(cb.Content.ToString());
    }

    private void cb_Checked(object sender, RoutedEventArgs e)
    {
      var cb = (CheckBox) sender;
      Lazy<ICalculatorPlugin, ICalculatorMetadata> lazyCalc = _calcs.Single(lc => lc.Metadata.Name.Equals(cb.Content));
      Actions.Items.Add(new ActionLayout(lazyCalc.Value, this));
    }

    private void UnloadAddIn(string calc)
    {
      var calcsAl = Actions.Items.Cast<ActionLayout>().FirstOrDefault(al => al.Calculator.Name == calc);
      if (calcsAl == null) return;
      calcsAl.Calculator = null;
      Actions.Items.Remove(calcsAl);
      CalculatorVisual.Children.Clear();
    }

    internal void Operate(ICalculatorPlugin calc, Operation op)
    {
      var operands = new double[op.NumOperands];
      for (int i = 0; i < op.NumOperands; i++)
      {
        var item = (ListViewItem) Stack.Items[0];
        operands[i] = double.Parse(item.Content.ToString());
        Stack.Items.RemoveAt(0);
      }
      if (typeof (INumericCalculator).IsAssignableFrom(calc.GetType()))
      {
        double result = ((INumericCalculator) calc).Operate(op, operands);
        var newItem = new ListViewItem {Content = result};
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
    private readonly int _count;

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