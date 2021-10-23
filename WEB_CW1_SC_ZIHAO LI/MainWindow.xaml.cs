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
                request = (HttpWebRequest)WebRequest.Create("https://" + url.Trim());
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

            title = title + "       statusCode:"+ statusCodeNUMBER;

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
            writeTxtHistory(link);

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
                HistoryMenu.PlacementTarget = HistoryButton;//init position at button
                HistoryMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                HistoryMenu.HorizontalOffset = -10;
                HistoryMenu.IsOpen = true;
            }

        }


        //home page setting
        //public void homePageSet_KeyDown(object sender, KeyEventArgs e)
        //{

        //   string hPurl= homePageSet.Text;
        //   writeTxtHomePage(hPurl);

        //}

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






        public void writeTxtHistory(string hPurl)
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
                FileStream fs = new FileStream("History.txt", FileMode.Open, FileAccess.Write);
                File.SetAttributes(@"History.txt", FileAttributes.Normal);
                StreamWriter sr = new StreamWriter(fs);
                sr.WriteLine(hPurl.Trim());//开始写入值
                sr.Close();
                fs.Close();

            }

        }

        public string readHistory()
        {

            if (File.Exists("History.txt"))
            {
                string lines = File.ReadAllText("HomePageHistorytxt", Encoding.Default);
                return lines;

            }
            else
            {
                return HomePage;
            }
        }


        private void addFavorites(object sender, RoutedEventArgs e)
        {
            string aFurl = addrsBar.Text;
            //string aFtitle = getWebTitle(addrsBar.Text);
            //addFavoritesXml( aFurl);
            MenuItem FavoritesItem = new MenuItem();
            FavoritesItem.Click += FavoritesItem_Click;
            FavoritesItem.Header = aFurl;
            FavoritesItem.Width = 280;

            FavoritesMenu.Items.Add(FavoritesItem);

            MessageBox.Show("add success :" + aFurl);

        }


        //public static void CreateXmlFile( string u)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    //2、创建第一行描述信息
        //    XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
        //    //3、将创建的第一行描述信息添加到文档中
        //    doc.AppendChild(dec);
        //    //4、给文档添加根节点
        //    XmlElement item = doc.CreateElement("item");
        //    doc.AppendChild(item);


        //    XmlElement url = doc.CreateElement("url");
        //    url.InnerText = u;
        //    item.AppendChild(url);



        //    doc.Save("Favorites.xml");
        //    //Console.WriteLine("保存成功！");
        //   // Console.ReadKey();
        //}

        //public static void addFavoritesXml( string u)
        //{
        //    XmlDocument doc = new XmlDocument();
        //    
        //    if (File.Exists("Favorites.xml"))
        //    {
        //        
        //        doc.Load("Favorites.xml");
        //        
        //        XmlElement item = doc.DocumentElement;

        //        XmlElement url = doc.CreateElement("url");
        //        url.InnerText = u;
        //        item.AppendChild(url);
        //        doc.Save("Favorites.xml");
        //    }
        //    else
        //    {
        //        CreateXmlFile(u);
        //    }
            
        //    //Console.WriteLine("Favorites.xml success");
        //}

        private void FavoritesItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem FavoritesItem = (MenuItem)sender;
            loadWebPages(FavoritesItem.Header.ToString());
        }
        private void Favorites_Click(object sender, RoutedEventArgs e)
        {
               // readXml();
         
                FavoritesMenu.PlacementTarget = FavoritesButton;//init position at button
                FavoritesMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                FavoritesMenu.HorizontalOffset = -10;
                FavoritesMenu.IsOpen = true;
            

        }


        //public  void readXml()
        //{
        //    if (File.Exists("Favorites.xml"))
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load("Favorites.xml");
        //        
        //        XmlElement itemElement = doc.DocumentElement;
        //        XmlNodeList orderChildr = itemElement.ChildNodes;
        //        foreach (XmlNode item in orderChildr)
        //        {
        //            FavoritesMenu.Items.Add(item.InnerText);
        //            //item.InnerText;
        //        }
        //        //XmlElement orderitem = orderElement["Items"];
        //        //XmlNodeList itemlist = orderitem.ChildNodes;
        //        //foreach (XmlNode item in itemlist)
        //        //{
        //        //    Console.WriteLine(item.Attributes["Name"].Value + " " + item.Attributes["Count"].Value);
        //        //}
        //    }
        //    else
        //    {
              
        //            //Console.WriteLine("文件不存在！");
        //    }
        //    //Console.ReadKey();
        //    // doc.Save("Student.xml");
        //    //Console.WriteLine("Student.xml 保存成功");
        //}




    }
}
 