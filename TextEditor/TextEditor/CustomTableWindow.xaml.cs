using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TextEditor
{
    public partial class CustomTableWindow : Window
    {
        public int SelectedRows { get; set; }
        public int SelectedColumns { get; set; }

        public CustomTableWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedRows = int.Parse(RowsInput.Text);
            SelectedColumns = int.Parse(ColumnsInput.Text);

            this.DialogResult = true;
            this.Close();
        }
    }
}
