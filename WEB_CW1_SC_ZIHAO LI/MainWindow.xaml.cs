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
using System.Text.RegularExpressions;

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
            this.Title ="morgan's browser";
        }




        private String getWebTitle(String url)
        {
            WebRequest wb;
            if (url.Contains("https://"))
            {
                 wb = WebRequest.Create(url.Trim());
            }
            //WebRequest wb = WebRequest.Create(url.Trim());
            else
            {
                 wb = WebRequest.Create("https://" + url.Trim());
            }

            WebResponse webRes = null;

            Stream webStream = null;
            try
            {
                webRes = wb.GetResponse();
                webStream = webRes.GetResponseStream();
            }
            catch (Exception e)
            {
                string stCode = Regex.Replace(e.Message, @"[^0-9]+", "");
                if (stCode == null)
                {
                    return "wrong url";
                }

                //int statusCodeINT = Convert.ToInt32(stCode);
                
                string statusCodeText;

                switch (stCode)
                {
                    case "400":
                        statusCodeText = "400 bad request";
                        break;
                    case "403":
                        statusCodeText = "403 forbidden";
                        break;
                    case "404":
                        statusCodeText = "403 notfound";
                        break;
                    default:
                        statusCodeText = e.Message;
                        break;
                }


                return "statusCode:"+statusCodeText;
            }

            StreamReader sr = new StreamReader(webStream, Encoding.Default);

            StringBuilder sb = new StringBuilder();

            String str = "";
            while ((str = sr.ReadLine()) != null)
            {
                sb.Append(str);
            }

            String regex = @"<title>.+</title>";

            String title = Regex.Match(sb.ToString(), regex).ToString();
            title = Regex.Replace(title, @"[\""]+", "");
            title = Regex.Replace(title, @"<title>", "");
            title = Regex.Replace(title, @"</title>", "");

            string statusCode=GetHttpResponse(url);
            string statusCodeNUMBER = Regex.Replace(statusCode, @"[^0-9]+", "");

         /*   int statusCodeINT = Convert.ToInt32(statusCodeNUMBER);
            string statusCodeText;

            switch (statusCodeINT)
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
                    statusCodeText = statusCode;
                    break;
            }*/

            title = title + "                   statusCode:"+ statusCodeNUMBER;

            return title;
        }




        public string GetHttpResponse(string url, int Timeout=5000)
        {
            //url = "www.baidu.com";
            HttpWebRequest request;
            try
            {
                if (url.Contains("https://"))
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "text/html;charset=UTF-8";
                    request.UserAgent = null;
                    request.Timeout = Timeout;
                }
                //WebRequest wb = WebRequest.Create(url.Trim());
                else
                {
                   
                    request = (HttpWebRequest)WebRequest.Create("https://" + url);
                    request.Method = "GET";
                    request.ContentType = "text/html;charset=UTF-8";
                    request.UserAgent = null;
                    request.Timeout = Timeout;
                }

             
                
                HttpWebResponse response = null;

                response = (HttpWebResponse)request.GetResponse();

                int statusCode = Convert.ToInt32(response.StatusCode);
                string statusCodeText = null;

                statusCodeText = "200 status OK！";
                

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
                Console.WriteLine(retString);
                statusCodeText_show.Text = statusCodeText;
                HTML_show.Text = retString;
              
                return statusCodeText;
            }
            catch (WebException e)
            {
                
             
                statusCodeText_show.Text = e.Message;

                HTML_show.Text = "/≥﹏≤ \\";
                return e.Message;
                // return "GET requext fail";

            }


        }


        void loadWebPages(string link, bool saveToHistory = true)
        {
            this.Title = getWebTitle(link);
            GetHttpResponse(link);
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
