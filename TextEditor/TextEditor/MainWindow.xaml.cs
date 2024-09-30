using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TextEditor
{
    public partial class MainWindow : RibbonWindow
    {
        public bool IsSaved = false;
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Document files (*.rtf)|*.rtf";
            var result = dlg.ShowDialog();
            if (result.Value == true)
            {
                try
                {
                    TextRange t = new TextRange(doc1.Document.ContentStart, doc1.Document.ContentEnd);
                    using (FileStream file = new FileStream(dlg.FileName, FileMode.Open))
                    {
                        t.Load(file, DataFormats.Rtf);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "unknown";
            savefile.Filter = "Document files (*.rtf)|*.rtf";
            if (savefile.ShowDialog() == true)
            {
                TextRange t = new TextRange(doc1.Document.ContentStart, doc1.Document.ContentEnd);
                this.Title = this.Title + " " + savefile.FileName;
                FileStream file = new FileStream(savefile.FileName, FileMode.Create);
                t.Save(file, System.Windows.DataFormats.Rtf);
                file.Close();
            }
            IsSaved = true;

        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaved == false)
                if (MessageBox.Show("Do you want save changes ?", "Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    this.btnSave_Click(sender, e);
            this.Close();
        }
        public double[] FontSizes
        {
            get
            {
                return new double[]
                {
                     3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0, 10.5, 11.0, 11.5, 12.0, 12.5,
                     13.0,13.5,14.0, 15.0,16.0, 17.0, 18.0, 19.0,
                     20.0, 22.0, 24.0, 26.0, 28.0, 30.0,32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0,
                     60.0, 64.0, 68.0, 72.0, 76.0,80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
                };
            }
        }
        void ApplyPropertyValueToSelectedText(DependencyProperty formattingProperty, object value)
        {
            if (value == null)
                return;
            doc1.Selection.ApplyPropertyValue(formattingProperty, value);
        }
        private void FontFamili_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            FontFamily editValue = (FontFamily)e.AddedItems[0];
            ApplyPropertyValueToSelectedText(TextElement.FontFamilyProperty, editValue);
        }
        private void FontSize_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            ApplyPropertyValueToSelectedText(TextElement.FontSizeProperty, e.AddedItems[0]);
        }
        private void InsertTable(int rows, int columns)
        {
            Table table = new Table();

            for (int i = 0; i < columns; i++)
            {
                TableColumn tableColumn = new TableColumn();
                tableColumn.Width = new GridLength(70);
                table.Columns.Add(tableColumn);
            }

            for (int i = 0; i < rows; i++)
            {
                TableRow row = new TableRow();

                for (int j = 0; j < columns; j++)
                {
                    TableCell cell = new TableCell(new Paragraph(new Run($" ")));
                    cell.BorderBrush = Brushes.Black;
                    cell.BorderThickness = new Thickness(1);

                    row.Cells.Add(cell);
                }

                TableRowGroup trg = new TableRowGroup();
                trg.Rows.Add(row);
                table.RowGroups.Add(trg);
            }

            doc1.Document.Blocks.Add(table);
        }
        private void InsertTable_Click(object sender, RoutedEventArgs e)
        {
            RibbonMenuItem menuItem = sender as RibbonMenuItem;

            string[] sizes = menuItem.Tag.ToString().Split(',');
            int rows = int.Parse(sizes[0]);
            int columns = int.Parse(sizes[1]);

            InsertTable(rows, columns);
        }
        private void CustomTable_Click(object sender, RoutedEventArgs e)
        {
            CustomTableWindow customTableWindow = new CustomTableWindow();
            if (customTableWindow.ShowDialog() == true)
            {
                int rows = customTableWindow.SelectedRows;
                int columns = customTableWindow.SelectedColumns;
                InsertTable(rows, columns);
            }
        }
        private void InsertImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName));
                Image image = new Image
                {
                    Source = bitmap,
                    Stretch = Stretch.Fill,
                    StretchDirection = StretchDirection.Both,
                    Width = 300,
                };

                double scaleFactor = 20;

                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(image);
                paragraph.MouseLeftButtonDown += (s, e) =>
                {
                    if (Keyboard.IsKeyDown(Key.LeftShift))
                    {
                        image.Width = Math.Max(0, image.Width - scaleFactor);
                        image.Height = Math.Max(0, image.Height - scaleFactor);
                    }
                    else if (Keyboard.IsKeyDown(Key.RightShift))
                    {
                        image.Width += scaleFactor;
                        image.Height += scaleFactor;
                    }
                };
                doc1.Document.Blocks.Add(paragraph);
            }
        }
        private void InsertRectangle_Click(object sender, RoutedEventArgs e)
        {
            Rectangle rectangle = new Rectangle
            {
                Width = 200,
                Height = 100,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            BlockUIContainer container = new BlockUIContainer();
            container.Child = rectangle;

            doc1.Document.Blocks.Add(container);
        }
        private void InsertCircle_Click(object sender, RoutedEventArgs e)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = 100,
                Height = 100,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
            };

            BlockUIContainer container = new BlockUIContainer();
            container.Child = ellipse;

            doc1.Document.Blocks.Add(container);
        }
        private void InsertLine_Click(object sender, RoutedEventArgs e)
        {
            Line line = new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 200,
                Y2 = 100,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            BlockUIContainer container = new BlockUIContainer();
            container.Child = line;
            doc1.Document.Blocks.Add(container);
        }
        private void InsertPolygon_Click(object sender, RoutedEventArgs e)
        {
            Polygon polygon = new Polygon
            {
                Points = new PointCollection(new[] { new Point(50, 0), new Point(100, 100), new Point(0, 100) }),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            BlockUIContainer container = new BlockUIContainer();
            container.Child = polygon;
            doc1.Document.Blocks.Add(container);
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (doc1.Selection.Text.Length > 0)
                doc1.Selection.Text = string.Empty;
        }
        private void doc1_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var textSelected = doc1.Selection.Text.Length > 0;

            ContextMenu contextMenu = doc1.ContextMenu;

            if (contextMenu.Items.Count > 0)
            {
                if (contextMenu.Items[0] is MenuItem cutItem)
                    cutItem.IsEnabled = textSelected;

                if (contextMenu.Items[1] is MenuItem copyItem)
                    copyItem.IsEnabled = textSelected;

                var fontMenu = contextMenu.Items[4] as MenuItem; 
                var lineSpacingMenu = contextMenu.Items[5] as MenuItem; 

                fontMenu.IsEnabled = textSelected;
                lineSpacingMenu.IsEnabled = textSelected;
            }
        }
        private void FontBold_Click(object sender, RoutedEventArgs e)
        {
            var isBold = (doc1.Selection.GetPropertyValue(Inline.FontWeightProperty) as FontWeight?) == FontWeights.Bold;
            doc1.Selection.ApplyPropertyValue(Inline.FontWeightProperty, isBold ? FontWeights.Normal : FontWeights.Bold);
        }
        private void FontItalic_Click(object sender, RoutedEventArgs e)
        {
            var isItalic = (doc1.Selection.GetPropertyValue(Inline.FontStyleProperty) as FontStyle?) == FontStyles.Italic;
            doc1.Selection.ApplyPropertyValue(Inline.FontStyleProperty, isItalic ? FontStyles.Normal : FontStyles.Italic);
        }
        private void FontUnderline_Click(object sender, RoutedEventArgs e)
        {
            var isUnderlined = (doc1.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection) == TextDecorations.Underline;
            if (isUnderlined)
                doc1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            else
                doc1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
        }
        private void LineSpacingSingle_Click(object sender, RoutedEventArgs e)
        {
            doc1.Selection.ApplyPropertyValue(Paragraph.LineHeightProperty, 1.0);
        }
        private void LineSpacing1_5_Click(object sender, RoutedEventArgs e)
        {
            doc1.Selection.ApplyPropertyValue(Paragraph.LineHeightProperty, 1.5);
        }
        private void LineSpacingDouble_Click(object sender, RoutedEventArgs e)
        {
            doc1.Selection.ApplyPropertyValue(Paragraph.LineHeightProperty, 2.0);
        }
        public MainWindow()
        {
            InitializeComponent();
            _fontSize.ItemsSource = FontSizes;
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)colorComboBox.SelectedItem;

            if (selectedItem != null)
            {
                string selectedColor = selectedItem.Content.ToString();

                System.Windows.Media.Color color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(selectedColor);
                ApplyPropertyValueToSelectedText(TextElement.ForegroundProperty, new SolidColorBrush(color));
            }
        }

    }
}