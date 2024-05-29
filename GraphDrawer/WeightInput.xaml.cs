using System.Windows;
using System.Windows.Controls;

namespace GraphDrawer
{
    /// <summary>
    /// TextBox window to get the user input. Specifically the weight of an edge.
    /// </summary>
    public partial class WeightInput : Window
    {
        public string InputText { get; private set; }

        public WeightInput()
        {
            InitializeComponent();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            InputText = TextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
