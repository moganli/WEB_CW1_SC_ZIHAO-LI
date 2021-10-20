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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp.Wpf;
using System.Net;

namespace WEB_CW1_SC_ZIHAO_LI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string HomePage = "https://www2.macs.hw.ac.uk/~yw2007/";
        string defaultPage = "www.google.com";
        List<string> WebPage;
        int Current = 0;

        public MainWindow()
        {
            InitializeComponent();
        }



        void loadWebPages(string link, bool saveToHistory = true)
        {
            Chrome.Address = link;
            addrsBar.Text = link;

            MenuItem historyItem = new MenuItem();
            historyItem.Click += HistoryItem_Click;
            historyItem.Header = link;
            historyItem.Width = 150;

            HistoryMenu.Items.Add(historyItem);

            if (saveToHistory)
            {
                Current++;
                WebPage.Add(link);
            }
        }

        private void addrsBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                loadWebPages(addrsBar.Text);
            }
        }

        private void backWebPage(object sender, RoutedEventArgs e)
        {

            if ((WebPage.Count + Current - 1) >= WebPage.Count)
            {
                Current--;
                loadWebPages(WebPage[Current], false);
                Chrome.CanGoBack.ToString();
            }
            else
            {
               
            }

        }

        private void forwardWebPage(object sender, RoutedEventArgs e)
        {
            if ((WebPage.Count - Current - 1) != 0)
            {
                Current++;
                loadWebPages(WebPage[Current], false);
            }
            else
            {
               
            }
        }

        private void reFresh(object sender, RoutedEventArgs e)
        {
            loadWebPages(WebPage[Current], false);

        }



        private void home(object sender, RoutedEventArgs e)
        {
            loadWebPages(HomePage);
        }

        private void window_loaded(object sender, RoutedEventArgs e)
        {
            WebPage = new List<string>();
            loadWebPages(defaultPage);

        }

        private void HistoryItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem historyItem = (MenuItem)sender;
            loadWebPages(historyItem.Header.ToString());
        }
        private void History_Click(object sender, RoutedEventArgs e)
        {
            if (WebPage.Count != 0)
            {
                HistoryMenu.PlacementTarget = HistoryButton;
                HistoryMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                //HistoryMenu.HorizontalOffset = -10;
                HistoryMenu.IsOpen = true;
            }
        }
    }
}
