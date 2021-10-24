using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net;
using System.IO;
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
        List<string> WebPage=null;
        int Current = 0;
        int writeHistory = 0;

        //this 3 for bulk download
        public static string localRote;
        public static string eachline_show;
        public static string localBulkRote;

        //default title is my name browser
        public MainWindow()
        {
            InitializeComponent();
            this.Title ="morgan's browser";
            
        }

        // at start the browser ,this method ruan,and load histry and favouries,the default page is google,creat a list to save pages
        private void window_loaded(object sender, RoutedEventArgs e)
        {
            WebPage = new List<string>();
            loadWebPages(defaultPage);
            readHistory();
            readFavorites();
        }


        //use advance lang features delegate------------------------------------------------------------------------------------------------------------------------------------------
        public delegate string useDelegate(string url);
        public static string useUrl(string url) 
        {
            return url;
        }
        public static string useNoneHttoUrl(string url)
        {
            return "https://" + url;
        }
        private static string chooseUrl(string url, useDelegate chooseUrl)
        {
            return chooseUrl(url);
        }


        //use advance lang features delegate-  and get status code and HTML code------------------------------------------------------------------------------------------------------


        //get windos title use <HttpWebRequest>-response, at first ,i implment delegate to choose to add "http://"
        //try/catch is a "Defensive code" for accident shutdown
        //if url is correct ,out put title and statcode
        //if url not correct ,use Regex to find stat number in Exception message ,and out put it to forntend
        private String getWebTitle(String url)
        {
            HttpWebRequest request;
            //WebRequest wb;
            bool choose;

            if (url.Contains("https://"))
            {
                url=chooseUrl(url, useUrl);
                choose = true;
            }
            else
            {
                url= chooseUrl(url, useNoneHttoUrl);
                choose = false;
            }



            if (choose)
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            //WebRequest wb = WebRequest.Create(url.Trim());
            else
            {
                try
                {
                    request = (HttpWebRequest)WebRequest.Create(url);
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
                        statusCodeText = "404 notfound";
                        break;

                    case "301":
                        statusCodeText = "301 moved permanently";
                        break;
                    case "302":
                        statusCodeText = "302 server moved";
                        break;
                    case "500":
                        statusCodeText = "500 internal server error";
                        break;
                    case "501":
                        statusCodeText = "501 not implemented";
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

            string statusCode=GetStatusCode(url);

            return title+"     "+statusCode;
        }

        //this method is very similar with last one，but this only out put the number of stat when url not correct，such as 404，400，302.......
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
                                statusCodeText = "400 ";
                                break;
                            case "403":
                                statusCodeText = "403 ";
                                break;
                            case "404":
                                statusCodeText = "404";
                                break;

                            case "301":
                                statusCodeText = "301 ";
                                break;
                            case "302":
                                statusCodeText = "302 ";
                                break;
                            case "500":
                                statusCodeText = "500 ";
                                break;
                            case "501":
                                statusCodeText = "501 ";
                                break;
                            default:
                        statusCodeText = e.Message;
                        break;
                }


                return "statusCode:" + statusCodeText;



                //return e.Message;
                // return "GET requext fail";

            }


        }
    
        //this method for get HTML cod and show it at the front 
        //if url wrong，it will show at the textblock right blow the addres bar
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
                string stCode = Regex.Replace(e.Message, @"[^0-9]+", "");
                if (stCode == null)
                {
                    return "wrong url";
                }

                switch (stCode)
                {
                    case "400":
                        statusCodeText = "400 bad request";
                        break;
                    case "403":
                        statusCodeText = "403 forbidden";
                        break;
                    case "404":
                        statusCodeText = "404 notfound";
                        break;

                    case "301":
                        statusCodeText = "301 moved permanently";
                        break;
                    case "302":
                        statusCodeText = "302 server moved";
                        break;
                    case "500":
                        statusCodeText = "500 internal server error";
                        break;
                    case "501":
                        statusCodeText = "501 not implemented";
                        break;

                    default:
                        statusCodeText = e.Message;
                        break;
                }


                statusCodeText_show.Text = statusCodeText;
                HTML_show.Text = "/≥﹏≤ \\";
                return e.Message;
                // return "GET requext fail";

            }


        }


        //this method for load page，when enter key down or botton down，--------------------------------------------------------------------------------------------------------------------
        //when load page, windos title change to web title use <getWebTitle>method
        //  creat menu ,every time load pages,add url to history menu item .and write to the TXT file for next time load program
        // if the pages is load by go back or goforward,then not add to webpage list
        public void loadWebPages(string link, bool saveToHistory = true)
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


        //those are the  response events of left click, right click, or enter----------------------------------------------------------------------------------------------------------------

        //addrsBar key:Enter  load page use url in addrs bar
        private void addrsBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {

                loadWebPages(addrsBar.Text);
            }
        }

        //page go back,if there no page to go bake, send a meassage at front.TRY is deffend code for unknow wrong
        private void backWebPage(object sender, RoutedEventArgs e)
        {

            if ((WebPage.Count + Current - 1) >= WebPage.Count)
            {
                try
                {
                    Current--;
                    loadWebPages(WebPage[Current], false);
                }
                catch (Exception)
                {
                    return;
                }

            }
            else
            {
                MessageBox.Show("pls try again,or cant go back");

            }

        }

        //page go forward,if there no page to go forward, send a meassage at front.
        private void forwardWebPage(object sender, RoutedEventArgs e)
        {
            if ((WebPage.Count - Current - 1) != 0)
            {
                try
                {
                    Current++;
                    loadWebPages(WebPage[Current], false);
                }
                catch (Exception)
                {
                    MessageBox.Show("pls try again,or cant go forward");
                    return;
                }
                
            }
            else
            {
                MessageBox.Show("pls try again,or cant go forward");

            }
        }

        //reload current page
        private void reFresh(object sender, RoutedEventArgs e)
        {
            // loadWebPages(WebPage[Current], false);
            loadWebPages(addrsBar.Text,true); 
        }

        //home button,click it ,program will load url which in homePage.txr , The details are in the <readHomePage>method
        private void home(object sender, RoutedEventArgs e)
        {
            loadWebPages(readHomePage());
        }

        //add current url to homePage.txt,The details are in the <writeTxtHomePage>method
        private void HomePageSet(object sender, RoutedEventArgs e)
        {
            string hPurl = addrsBar.Text;
            writeTxtHomePage(hPurl);
            MessageBox.Show("home page set to :"+hPurl);
        }

        //load page use menuitem.head which is also the url of it
        private void HistoryItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem historyItem = (MenuItem)sender;
            loadWebPages(historyItem.Header.ToString());
        }

        //show the history menu
        private void History_Click(object sender, RoutedEventArgs e)
        {

            if (WebPage.Count != 0)
            {
                HistoryMenu.Items.Clear();
                readHistory();
                HistoryMenu.PlacementTarget = HistoryButton;//init position at button
                HistoryMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                HistoryMenu.HorizontalOffset = -10;
                HistoryMenu.IsOpen = true;
            }
        }

        //right click history Button will delete all history.
        private void HistoryButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
            if (File.Exists("History.txt"))
            {
                File.Delete("History.txt");
                MessageBox.Show("history deleted");
            }
            
        }

        //add current url to favourites.txt and favourites menu
        private void addFavorites(object sender, RoutedEventArgs e)
        {
            string aFurl = addrsBar.Text;
            //if{} decide whether add to menu or not
            if (writeTxtFavorites(aFurl))
            {
                MenuItem FavoritesItem = new MenuItem();
                FavoritesItem.PreviewMouseLeftButtonDown += FavoritesItem_Click;
                FavoritesItem.PreviewMouseRightButtonDown += FavoritesItem_MouseRightButtonDown;
                FavoritesItem.Header = aFurl;
                FavoritesItem.Width = 280;

                FavoritesMenu.Items.Add(FavoritesItem);
            }


        }

        //load webpage use menuitem.head which is also the url of it
        private void FavoritesItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem FavoritesItem = (MenuItem)sender;
            loadWebPages(FavoritesItem.Header.ToString());
            //favoritesItem = FavoritesItem.Header.ToString();

        }

        //show the Favorites menu
        private void Favorites_Click(object sender, RoutedEventArgs e)
        {
            // readXml();
            FavoritesMenu.Items.Clear();
            readFavorites();
            FavoritesMenu.PlacementTarget = FavoritesButton;//init position at button
            FavoritesMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            FavoritesMenu.HorizontalOffset = -10;
            FavoritesMenu.IsOpen = true;

                //FavoritesMenu = null;
                //readFavorites();

        }

        //right click his FavoritesItem will delete it
        private void FavoritesItem_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuItem FavoritesItem = (MenuItem)sender;
            string deleteurl = FavoritesItem.Header.ToString();

            string text =null;
            using (StreamReader reader = new StreamReader("Favorites.txt"))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    
                    if (line.IndexOf(deleteurl) >= 0)
                    {
                    }
                    else
                    {
                        text += line + "\r\n";
                    }
                    line = reader.ReadLine();
                }
            }
            using (StreamWriter writer = new StreamWriter("Favorites.txt"))
            {
                writer.Write(text);
            }
            MessageBox.Show("Favorites Item deleted");


        }

        //load bulk.txt read each line of it ,then output <stutCode> <bytes> <url>to front 
        private void bulk_download(object sender, RoutedEventArgs e)
        {

        bulkDownload();
       
        }

        //right click home button will reset it to hw/zl2013
        private void delehome(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists("HomePage.txt"))
            {
                File.Delete("HomePage.txt");
                MessageBox.Show("home set to default");
            }

        }



        //home history favorites methods -------------------------------------------------------------------------------------------------------------------------------
        //creat file writehomepage,because homePage.txt always contain 1 url ,so always overwrite old one
        public void writeTxtHomePage(string hPurl)
        {            //file exists or not
            //if (!File.Exists("HomePage.txt"))
            //{
                //if not ,creat file
                FileStream fs1 = new FileStream("HomePage.txt", FileMode.Create, FileAccess.Write);//creat file                /
                File.SetAttributes(@"HomePage.txt", FileAttributes.Normal);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(hPurl.Trim());//input vale
                sw.Close();
                fs1.Close();
                
            //}
            //else
            //{
            //    FileStream fs = new FileStream("HomePage.txt", FileMode.Open, FileAccess.Write);
            //    File.SetAttributes(@"HomePage.txt", FileAttributes.Normal);
            //    StreamWriter sr = new StreamWriter(fs);
            //    sr.WriteLine(hPurl.Trim() );//input vale
            //    sr.Close();
            //    fs.Close();
                
            //}

        }

        //read homePage.txt and return value,if the homePage been delete by right clcik ,go to default home page. whicj is my HW page
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

        //first check file exists or not .if not ,creat one and write uri in the file. if file exists ,add url in this file.
        public  void writeTxtHistory(string Hiurl)
        {            
            if (!File.Exists("History.txt"))
            {
                
                FileStream fs1 = new FileStream("History.txt", FileMode.Create, FileAccess.Write);
                File.SetAttributes(@"History.txt", FileAttributes.Normal);
                StreamWriter sw = new StreamWriter(fs1);
                //sw.WriteLine(Hiurl.Trim());
                sw.Close();
                fs1.Close();

            }
            else
            {
                //true mean can add to this file
                using (StreamWriter outputFile = new StreamWriter( "History.txt",true))
                {
                    outputFile.WriteLine(Hiurl);
                }

            }

        }

        //read History.txt,add them to history menu,if there no such file ,then nothing need to read
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

        }

        //write Favorites.txt,just like write history,but if there same url in favorite,return fale
        public bool writeTxtFavorites(string hPurl)
        {            
            if (!File.Exists("Favorites.txt"))
            {
                FileStream fs1 = new FileStream("Favorites.txt", FileMode.Create, FileAccess.Write);//创建写入文件                /
                File.SetAttributes(@"Favorites.txt", FileAttributes.Normal);
                StreamWriter sw = new StreamWriter(fs1);
                sw.WriteLine(hPurl.Trim());
                sw.Close();
                fs1.Close();
                return true;
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
                            return false;
                        }
                    }
                }

                    using (StreamWriter outputFile = new StreamWriter("Favorites.txt", true))
                    {
                        outputFile.WriteLine(hPurl);
                     MessageBox.Show("add Favorites success :" + hPurl);
                }

                return true;
            }

        }

        //read favorites.txt and add to favorites menu
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

        }

        //bulk return:<stutCode> <bytes> <url>,base on <GetStatusCode>&<GetHttpResponse> method
        public string bulkDownloadReturn(string rote) {
            eachline_show = "";
            //localRote = addrsBar.Text;
            //localBulkRote = "bulk.txt";
            string lines;
            string eachline;
            //string eachline_show;
            using (StreamReader reader = new StreamReader(rote))
            {
                while ((lines = reader.ReadLine()) != null)
                {
                    //string eachline_show;
                    //Console.WriteLine(lines);
                    string statCode = GetStatusCode(lines);
                    //string bytes = Convert.ToString(Encoding.Default.GetBytes(GetHttpResponse(lines)));
                    byte[] bytes = Encoding.Default.GetBytes(GetHttpResponse(lines));
                    eachline = Convert.ToString(statCode + "   " + bytes.Length + "  bytes" + "   " + lines + "\n");
                    //Console.WriteLine(eachline);
                    //Console.WriteLine(eachline);
                    eachline_show += eachline;
                }
                
               // statusCodeText_show.Text = "/≥﹏≤ \\";
                return  eachline_show;
                //Console.WriteLine(eachline_show);
                //Console.WriteLine(eachline);
            }
        }

        /*
        for each situation,
                1.user input bulk.txt and it exist :use <bulkDownloadReturn>and show the srting
                2.user input bulk.txt and it NOT EXIST :generate bulk.txt and do 1st situation
                
                3.user input (PATH)xxxx.txt and the file exist:use <bulkDownloadReturn>and show the srting
                4.user input (PATH)xxxx.txt and the file NOT EXIST:generate xxxx.txt and do 3rd situation
        
        so, the default is bulk.txt and u can name any.txt file and use it
        or u can input Absolute PATH of the .txt  file u want bulk download in addres bar
        */
        public void bulkDownload()
        {
           
            eachline_show = "";
            localRote = addrsBar.Text;
            localBulkRote = "bulk.txt";
            Title = localRote + "     bulk DOWNLOAD";
            if (localRote == localBulkRote)
            {
                if (File.Exists(localBulkRote))
                {

                    HTML_show.Text =bulkDownloadReturn(localBulkRote);
                    statusCodeText_show.Text = "/≥﹏≤ \\";

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
                    HTML_show.Text = bulkDownloadReturn(localBulkRote);
                    statusCodeText_show.Text = "/≥﹏≤ \\";
                }

            }
            else
            {

                if (File.Exists(localRote))
                {
                    HTML_show.Text = bulkDownloadReturn(localRote);
                    statusCodeText_show.Text = "/≥﹏≤ \\";
                }
                else //if (!File.Exists(localBulkRote))
                {
                    FileStream fs1 = new FileStream(localRote, FileMode.Create, FileAccess.Write);//创建写入文件                /
                    File.SetAttributes(localRote, FileAttributes.Normal);
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine("www.hw.ac.uk");
                    sw.Close();
                    fs1.Close();
                    HTML_show.Text = bulkDownloadReturn(localRote);
                    statusCodeText_show.Text = "file generate success,/≥﹏≤ \\";
                   
                }

            }
        }





        //Shortcut key of 5 buttons
        //F5 for refresh
        private void refreshKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            loadWebPages(addrsBar.Text, true);
        }

        //F3 for forward
        private void forwardKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((WebPage.Count - Current - 1) != 0)
            {
                try
                {
                    Current++;
                    loadWebPages(WebPage[Current], false);
                }
                catch (Exception)
                {
                    MessageBox.Show("pls try again,or cant go forward");
                    return;
                }

            }
            else
            {
                MessageBox.Show("pls try again,or cant go forward");

            }
        }

        //F2 for back
        private void backKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((WebPage.Count + Current - 1) >= WebPage.Count)
            {
                try
                {
                    Current--;
                    loadWebPages(WebPage[Current], false);
                }
                catch (Exception)
                {
                    return;
                }

            }
            else
            {
                MessageBox.Show("pls try again,or cant go back");

            }
        }

        //F4 for go history
        private void homeKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            loadWebPages(readHomePage());
        }

        //CTRL+H for history menu
        private void historyKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (WebPage.Count != 0)
            {
                HistoryMenu.PlacementTarget = HistoryButton;//init position at button
                HistoryMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                HistoryMenu.HorizontalOffset = -10;
                HistoryMenu.IsOpen = true;
            }
        }

        //ZTRL + F for favorites menu
        private void FavoritesKey_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            FavoritesMenu.PlacementTarget = FavoritesButton;//init position at button
            FavoritesMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            FavoritesMenu.HorizontalOffset = -10;
            FavoritesMenu.IsOpen = true;
        }
       
    }
}
 