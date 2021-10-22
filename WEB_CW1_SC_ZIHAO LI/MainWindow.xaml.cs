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
using CefSharp;
using System.Net;
using System.IO;
using System.Collections.Specialized;

namespace WEB_CW1_SC_ZIHAO_LI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string HomePage = "https://www2.macs.hw.ac.uk/~zl2013/";
        string defaultPage = "https://www.google.com";
        List<string> WebPage;
        int Current = 0;

        public MainWindow()
        {
            InitializeComponent();
        }



        public void GetHttpResponse(string url, int Timeout)
        {
            //url = "www.baidu.com";
            HttpWebRequest request;
            try
            {

                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.UserAgent = null;
                request.Timeout = Timeout;
                
                HttpWebResponse response = null;

                response = (HttpWebResponse)request.GetResponse();

                int statusCode = Convert.ToInt32(response.StatusCode);
                string statusCodeText = null;
                if (statusCode != 200)
                {
                    switch (statusCode)
                    {
                        case 400:
                            statusCodeText = "400 bad request";
                            break;
                        case 403:
                            statusCodeText = "403 forbidden";
                            break;
                        case 404:
                            statusCodeText = "403 notfound";
                            break;
                        default:
                            statusCodeText = "other status,pls check url";
                            break;
                    }
                }
                else
                {
                    statusCodeText = "200 status OK！";
                }




                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                Console.WriteLine(retString);
                statusCodeText_show.Text = statusCodeText;
                HTML_show.Text = retString;
              
               // return retString;
            }
            catch (WebException e)
            {
                
                //return "GET requext fail";
                statusCodeText_show.Text = e.Message;

                HTML_show.Text = "/≥﹏≤ \\";
                // return "GET requext fail";

            }


        }


        void loadWebPages(string link, bool saveToHistory = true)
        {
            GetHttpResponse(link,5000);
            //Chrome.Address = link;
            addrsBar.Text = link;

            MenuItem historyItem = new MenuItem();
            historyItem.Click += HistoryItem_Click;
            historyItem.Header = link;
            historyItem.Width = 280;

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
            // loadWebPages(WebPage[Current], false);
            loadWebPages(addrsBar.Text,true); 
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
                HistoryMenu.HorizontalOffset = -10;
                HistoryMenu.IsOpen = true;
            }
        }
    }
}
