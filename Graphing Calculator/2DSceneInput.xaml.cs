using System.Windows;

namespace GraphCalc
{
    /// <summary>
    /// Interaction logic for _DSceneInput.xaml
    /// </summary>
    public partial class SceneInput2D : System.Windows.Controls.UserControl
    {
        public SceneInput2D()
        {
            InitializeComponent();
            GraphIt.Click += new RoutedEventHandler(GraphIt_Click);
        }

        void GraphIt_Click(object sender, RoutedEventArgs e)
        {
            Display.Children.Clear();
            Grapher grapher = new Grapher();
            Display.Children.Add(grapher.Show2D(this.Equation.Text));
        }
    }
}
