using PDFHighlights;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDFHighlightsDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path;
        public MainWindow()
        {
            InitializeComponent();
            path = Directory.GetCurrentDirectory() + "\\Images\\Sample.pdf";
        }

        private void Btn_BrowsePDF_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                DefaultExt = "*.pdf",
                CheckFileExists = true,
                CheckPathExists = true,
                Title = "Select PDF File"

            };

            if (dlg.ShowDialog() == true)
            {
                path = dlg.FileName;
                tb_Info.Text = System.IO.Path.GetFileName(path);
                myStackPanel.Children.Clear();
            }
        }

        private async void Btn_Process_Click(object sender, RoutedEventArgs e)
        {
            Btn_BrowsePDF.IsEnabled = false;
            Btn_Process.IsEnabled = false;
            myStackPanel.Visibility = Visibility.Collapsed;
            //mySpinner.Visibility = Visibility.Visible;
            mySpinner.IsLoading = true;
            myStackPanel.Children.Clear();

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            HighlightedTextCollections collections = await Process();
            //stopwatch.Stop();
            //tb_Info.Text = $"Time Taken: {(stopwatch.ElapsedMilliseconds).ToString()}";

            mySpinner.IsLoading = false;
            //mySpinner.Visibility = Visibility.Collapsed;
            myStackPanel.Visibility = Visibility.Visible;
            Btn_BrowsePDF.IsEnabled = true;
            Btn_Process.IsEnabled = true;

            foreach (HighlightedTextCollection collection in collections.TextCollections)
            {
                Color color = Color.FromRgb(collection.HighLightColor.R, collection.HighLightColor.G, collection.HighLightColor.B);
                SolidColorBrush textBackgroundBrush = new SolidColorBrush(color);
                foreach (PDFText pdfText in collection.TextCollection)
                {
                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;

                    TextBlock text = new TextBlock();
                    text.Foreground = Brushes.Black;
                    text.Background = textBackgroundBrush;
                    text.FontSize = 24;
                    text.Text = "> " + pdfText.Text;
                    text.Margin = new Thickness(20, 0, 0, 0);

                    TextBlock pageText = new TextBlock();
                    pageText.Foreground = Brushes.Black;
                    pageText.Background = textBackgroundBrush;
                    pageText.FontSize = 12;
                    pageText.Text = "   [Page " + pdfText.PageNo + "]";
                    //pageText.Margin = new Thickness(20,0,0,0);

                    panel.Children.Add(text);
                    panel.Children.Add(pageText);
                    myStackPanel.Children.Add(panel);

                    TextBlock text2 = new TextBlock();
                    text2.Foreground = Brushes.Black;
                    //text2.Background = textBackgroundBrush;
                    text2.FontSize = 8;
                    text2.Text = "-------------------------------------------------";
                    myStackPanel.Children.Add(text2);
                }

                TextBlock text3 = new TextBlock();
                text3.Foreground = Brushes.Black;
                //text2.Background = textBackgroundBrush;
                text3.FontSize = 14;
                text3.Text = "-------------------------------------------------";
                myStackPanel.Children.Add(text3);

            }




        }

        public async Task<HighlightedTextCollections> Process()
        {
            HighlightedTextCollections collections;
            collections = await Task<HighlightedTextCollections>.Run(() =>
            {
                PDFHighlightCollector collector = new PDFHighlightCollector();
                return collector.PDFReadFile(path, "@87654321");
            });
            return collections;
        }
    }
}
