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
using System.Xml;

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

        private void window_loaded(object sender, RoutedEventArgs e)
        {
            WebPage = new List<string>();
            loadWebPages(defaultPage);
            readHistory();
            readFavorites();
        }


        private String getWebTitle(String url)
        {
            HttpWebRequest request;
            //WebRequest wb;
            if (url.Contains("https://"))
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            //WebRequest wb = WebRequest.Create(url.Trim());
            else
            {
                try
                {
                    request = (HttpWebRequest)WebRequest.Create("https://" + url);
                }
                catch (Exception e)
                {

                    HTML_show.Text = e.Message;
                    request= (HttpWebRequest)WebRequest.Create("https://savanttools.com/ test-http-status-codes");
                    statusCodeText_show.Text = "404";
                    return "404";
                }
                //request = (HttpWebRequest)WebRequest.Create("https://" + url);
            }
  

            WebResponse webRes = null;

            Stream webStream = null;
            try
            {
                webRes = request.GetResponse();
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

            string title = Regex.Match(sb.ToString(), regex).ToString();
            title = Regex.Replace(title, @"[\""]+", "");
            title = Regex.Replace(title, @"<title>", "");
            title = Regex.Replace(title, @"</title>", "");

            return title;
        }


        public string GetStatusCode(string url)
        {
            HttpWebRequest request;
            try
            {
                if (url.Contains("https://"))
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    request.ContentType = "text/html;charset=UTF-8";
                    request.UserAgent = null;
                    
                }
                //WebRequest wb = WebRequest.Create(url.Trim());
                else
                {

                    request = (HttpWebRequest)WebRequest.Create("https://" + url);
                    request.Method = "GET";
                    request.ContentType = "text/html;charset=UTF-8";
                    request.UserAgent = null;
                    
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
                //Console.WriteLine(retString);
                //statusCodeText_show.Text = statusCodeText;
                //HTML_show.Text = retString;

                return statusCodeText;
            }
            catch (WebException e)
            {
                //statusCodeText_show.Text = e.Message;
                //HTML_show.Text = "/≥﹏≤ \\";
                return e.Message;
                // return "GET requext fail";

            }


        }
    
    

            public string GetHttpResponse(string url, int Timeout=5000)
            {
            //url = "www.baidu.com";
            HttpWebRequest request;
            string statusCodeText = null;
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
                    try
                    {
                        request = (HttpWebRequest)WebRequest.Create("https://" + url);
                        request.Method = "GET";
                        request.ContentType = "text/html;charset=UTF-8";
                        request.UserAgent = null;
                        request.Timeout = Timeout;
                    }
                    catch (Exception e)
                    {


                        HTML_show.Text = e.Message;
                        request = (HttpWebRequest)WebRequest.Create("https://savanttools.com/ test-http-status-codes");
                        statusCodeText_show.Text= "404";
                        return "404";
                    }

                }

             
                
                HttpWebResponse response = null;

                response = (HttpWebResponse)request.GetResponse();

                int statusCode = Convert.ToInt32(response.StatusCode);
                

                statusCodeText = "200 status OK！";
                

                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
               // Console.WriteLine(retString);
                statusCodeText_show.Text = statusCodeText;
                HTML_show.Text = retString;
              
                return retString;
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
            writeTxtHistory(link);
            //readHistory();

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
            loadWebPages(readHomePage());
        }

        private void HomePageSet(object sender, RoutedEventArgs e)
        {
            string hPurl = addrsBar.Text;
            writeTxtHomePage(hPurl);
            MessageBox.Show("home page set to :"+hPurl);
        }


        public void writeTxtHomePage(string hPurl)
        {            //判断是否已经有了这个文件
            if (!File.Exists("HomePage.txt"))
            {
                //没有则创建这个文件
                FileStream fs1 = new FileStream("HomePage.txt", FileMode.Create, FileAccess.Write);//创建写入文件                /
                File.SetAttributes(@"HomePage.txt", FileAttributes.Normal);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(hPurl.Trim());//开始写入值
                sw.Close();
                fs1.Close();
                
            }
            else
            {
                FileStream fs = new FileStream("HomePage.txt", FileMode.Open, FileAccess.Write);
                File.SetAttributes(@"HomePage.txt", FileAttributes.Normal);
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(hPurl.Trim() );//开始写入值
                sr.Close();
                fs.Close();
                
            }

        }

        public string readHomePage()
        {

            if (File.Exists("HomePage.txt"))
            {
                string lines = File.ReadAllText("HomePage.txt", Encoding.Default);
                return lines;


            }
            else
            {
                return HomePage;
            }
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
                HistoryMenu.PlacementTarget = HistoryButton;//init position at button
                HistoryMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                HistoryMenu.HorizontalOffset = -10;
                HistoryMenu.IsOpen = true;
            }
        }

        private void HistoryButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (File.Exists("History.txt"))
            {
                File.Delete("History.txt");
                MessageBox.Show("history deleted");
            }
            
        }

            //home page setting




        public  void writeTxtHistory(string hPurl)
        {            //判断是否已经有了这个文件
            if (!File.Exists("History.txt"))
            {
                //没有则创建这个文件
                FileStream fs1 = new FileStream("History.txt", FileMode.Create, FileAccess.Write);//创建写入文件                /
                File.SetAttributes(@"History.txt", FileAttributes.Normal);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(hPurl.Trim());//开始写入值
                sw.Close();
                fs1.Close();

            }
            else
            {

                using (StreamWriter outputFile = new StreamWriter( "History.txt",true))
                {
                    outputFile.WriteLine(hPurl);
                }

            }

        }

        public void readHistory()
        {
            var count = 0;
            if (File.Exists("History.txt"))
            {
                //string lines = File.ReadAllText("History.txt", Encoding.Default);
                // return lines;

                string lines;
                using(StreamReader reader = new StreamReader("History.txt"))
                {

                    while ((lines = reader.ReadLine()) != null)
                    {
                        MenuItem historyItem = new MenuItem();
                        historyItem.Click += HistoryItem_Click;
                        historyItem.Header = lines;
                        historyItem.Width = 280;
                        HistoryMenu.Items.Add(historyItem);
                        count++;
                    }
                }

                //HistoryMenu.Items.Add(lines);
               
            }
            else
            {
                
            }
        }


        private void addFavorites(object sender, RoutedEventArgs e)
        {
            string aFurl = addrsBar.Text;
            //string aFtitle = getWebTitle(addrsBar.Text);
            //addFavoritesXml( aFurl);
            MenuItem FavoritesItem = new MenuItem();
            FavoritesItem.PreviewMouseLeftButtonDown += FavoritesItem_Click;
            FavoritesItem.PreviewMouseRightButtonDown += FavoritesItem_MouseRightButtonDown;
            FavoritesItem.Header = aFurl;
            FavoritesItem.Width = 280;

            FavoritesMenu.Items.Add(FavoritesItem);
            writeTxtFavorites(aFurl);


            //MessageBox.Show("add Favorites success :" + aFurl);

        }



        //string favoritesItem;
        public void FavoritesItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem FavoritesItem = (MenuItem)sender;
            loadWebPages(FavoritesItem.Header.ToString());
            //favoritesItem = FavoritesItem.Header.ToString();

        }
        private void Favorites_Click(object sender, RoutedEventArgs e)
        {
               // readXml();
         
                FavoritesMenu.PlacementTarget = FavoritesButton;//init position at button
                FavoritesMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                FavoritesMenu.HorizontalOffset = -10;
                FavoritesMenu.IsOpen = true;
            

        }

        public void writeTxtFavorites(string hPurl)
        {            //判断是否已经有了这个文件
            if (!File.Exists("Favorites.txt"))
            {
                //没有则创建这个文件
                FileStream fs1 = new FileStream("Favorites.txt", FileMode.Create, FileAccess.Write);//创建写入文件                /
                File.SetAttributes(@"Favorites.txt", FileAttributes.Normal);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(hPurl.Trim());//开始写入值
                sw.Close();
                fs1.Close();

            }
            else
            {
                string lines;
                using (StreamReader reader = new StreamReader("Favorites.txt"))
                {

                    while ((lines = reader.ReadLine()) != null)
                    {
                        if (lines == hPurl)
                        {
                            MessageBox.Show("this url already in Favorites ");
                            return;
                        }
                    }
                }

                    using (StreamWriter outputFile = new StreamWriter("Favorites.txt", true))
                    {
                        outputFile.WriteLine(hPurl);
                     MessageBox.Show("add Favorites success :" + hPurl);
                }

                
            }

        }

        public void readFavorites()
        {
            
            if (File.Exists("Favorites.txt"))
            {
                //string lines = File.ReadAllText("History.txt", Encoding.Default);
                // return lines;

                string lines;
                using (StreamReader reader = new StreamReader("Favorites.txt"))
                {

                    while ((lines = reader.ReadLine()) != null)
                    {
                        MenuItem FavoritesItem = new MenuItem();
                        FavoritesItem.PreviewMouseLeftButtonDown += FavoritesItem_Click;
                        FavoritesItem.PreviewMouseRightButtonDown += FavoritesItem_MouseRightButtonDown;
                        FavoritesItem.Header = lines;
                        FavoritesItem.Width = 280;

                        FavoritesMenu.Items.Add(FavoritesItem);


                    }
                }

                //HistoryMenu.Items.Add(lines);

            }
            else
            {

            }
        }

        public void FavoritesItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuItem FavoritesItem = (MenuItem)sender;
            string deleteurl = FavoritesItem.Header.ToString();


            //定义一个变量用来存读到的东西
            string text =null;
            //用一个读出流去读里面的数据
            using (StreamReader reader = new StreamReader("Favorites.txt"))
            {
                //读一行
                string line = reader.ReadLine();
                while (line != null)
                {
                    //如果这一行里面有abe这三个字符，就不加入到text中，如果没有就加入
                    if (line.IndexOf(deleteurl) >= 0)
                    {
                    }
                    else
                    {
                        text += line + "\r\n";
                    }
                    //一行一行读
                    line = reader.ReadLine();
                }
            }
            //定义一个写入流，将值写入到里面去
            using (StreamWriter writer = new StreamWriter("Favorites.txt"))
            {
                writer.Write(text);
            }
            MessageBox.Show("Favorites Item deleted");

        }

        public static string localRote;
        public static string eachline_show;
        public static string localBulkRote;


        public void bulkDownload()
        {

            eachline_show = "";
            localRote = addrsBar.Text;
            localBulkRote = "bulk.txt";
            //string eachline_show;
            if (localRote == localBulkRote)
            {
                if (File.Exists(localBulkRote))
                {


                    string lines;
                    string eachline;
                    //string eachline_show;
                    using (StreamReader reader = new StreamReader(localBulkRote))
                    {
                        while ((lines = reader.ReadLine()) != null)
                        {
                            //string eachline_show;
                            //Console.WriteLine(lines);
                            string statCode = GetStatusCode(lines);
                            //string bytes = Convert.ToString(Encoding.Default.GetBytes(GetHttpResponse(lines)));
                            byte[] bytes = Encoding.Default.GetBytes(GetHttpResponse(lines));
                            eachline = Convert.ToString(statCode + "   " + bytes.Length + "   " + lines + "\n");
                            //Console.WriteLine(eachline);
                            //Console.WriteLine(eachline);
                            eachline_show += eachline;
                        }
                        HTML_show.Text = eachline_show;
                        statusCodeText_show.Text = "/≥﹏≤ \\";
                        //Console.WriteLine(eachline_show);
                        //Console.WriteLine(eachline);
                    }

                }
                else //if (!File.Exists(localBulkRote))
                {

                    FileStream fs1 = new FileStream(localBulkRote, FileMode.Create, FileAccess.Write);//创建写入文件                /
                    File.SetAttributes(@localBulkRote, FileAttributes.Normal);
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine("https://www.google.com");//开始写入值
                    sw.WriteLine("https://www.baidu.com");
                    sw.WriteLine("www.hw.ac.uk");
                    sw.WriteLine("https://www2.macs.hw.ac.uk/~zl2013/");
                    sw.Close();
                    fs1.Close();


                    string lines;
                    string eachline;
                    //string eachline_show;
                    using (StreamReader reader = new StreamReader(localBulkRote))
                    {
                        while ((lines = reader.ReadLine()) != null)
                        {
                            //string eachline_show;
                            //Console.WriteLine(lines);
                            string statCode = GetStatusCode(lines);
                            //string bytes = Convert.ToString(Encoding.Default.GetBytes(GetHttpResponse(lines)));
                            byte[] bytes = Encoding.Default.GetBytes(GetHttpResponse(lines));
                            eachline = Convert.ToString(statCode + "   " + bytes.Length + "   " + lines + "\n");
                            //Console.WriteLine(eachline);
                            //Console.WriteLine(eachline);
                            eachline_show += eachline;
                        }
                        HTML_show.Text = eachline_show;
                        statusCodeText_show.Text = "/≥﹏≤ \\";
                        //Console.WriteLine(eachline_show);
                        //Console.WriteLine(eachline);
                    }
                }


            }
            else
            {

                if (File.Exists(localRote))
                {
                    string lines;
                    string eachline;
                    //string eachline_show;
                    using (StreamReader reader = new StreamReader(localRote))
                    {
                        while ((lines = reader.ReadLine()) != null)
                        {
                            //string eachline_show;
                            //Console.WriteLine(lines);
                            string statCode = GetStatusCode(lines);
                            //string bytes = Convert.ToString(Encoding.Default.GetBytes(GetHttpResponse(lines)));
                            byte[] bytes = Encoding.Default.GetBytes(GetHttpResponse(lines));
                            eachline = Convert.ToString(statCode + "   " + bytes.Length + "   " + lines + "\n");
                            //Console.WriteLine(eachline);
                            //Console.WriteLine(eachline);
                            eachline_show += eachline;
                        }
                        HTML_show.Text = eachline_show;
                        statusCodeText_show.Text = "/≥﹏≤ \\";
                        //Console.WriteLine(eachline_show);
                        //Console.WriteLine(eachline);
                    }

                }
                else //if (!File.Exists(localBulkRote))
                {
                    FileStream fs1 = new FileStream(localRote, FileMode.Create, FileAccess.Write);//创建写入文件                /
                    File.SetAttributes(localRote, FileAttributes.Normal);
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine("www.hw.ac.uk");
                    sw.Close();
                    fs1.Close();


                    string lines;
                    string eachline;
                    //string eachline_show;
                    using (StreamReader reader = new StreamReader(localRote))
                    {
                        while ((lines = reader.ReadLine()) != null)
                        {
                            //string eachline_show;
                            //Console.WriteLine(lines);
                            string statCode = GetStatusCode(lines);
                            //string bytes = Convert.ToString(Encoding.Default.GetBytes(GetHttpResponse(lines)));
                            byte[] bytes = Encoding.Default.GetBytes(GetHttpResponse(lines));
                            eachline = Convert.ToString(statCode + "   " + bytes.Length + "   " + lines + "\n");
                            //Console.WriteLine(eachline);
                            //Console.WriteLine(eachline);
                            eachline_show += eachline;
                        }
                        HTML_show.Text = eachline_show;
                        statusCodeText_show.Text = "file generate success,/≥﹏≤ \\";
                        //Console.WriteLine(eachline_show);
                        //Console.WriteLine(eachline);
                    }

                    
                  //  statusCodeText_show.Text = "file generate success,/≥﹏≤ \\";


                }

            }
        }
    

        private void bulk_download(object sender, RoutedEventArgs e)
        {

        bulkDownload();

        }
    }
}
 