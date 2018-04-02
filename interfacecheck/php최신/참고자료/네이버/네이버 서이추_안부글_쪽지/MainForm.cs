using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace 네이버_서이추_안부글_쪽지
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int hWnd1, int hWnd2, string lpsz1, string lpsz2);
        [DllImport("user32")]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        public string NID = string.Empty;
        public string NPW = string.Empty;
        public CookieContainer logincookie = new CookieContainer();

        private ChromeDriver driver; // 크롬드라이버
        private Thread th; // Threading

        bool workstatus = false;
        string nowip = string.Empty;
        string selectedImagePath = string.Empty;
        string selectedImageName = string.Empty;
        string selectedImageSize = string.Empty;
        string selectedImageRealSize = string.Empty;
        List<string> postIDList = new List<string>();
        #region 로그작성 함수
        private delegate void SetTextCallback(string strText);

        public void log(string text)
        {
            if (this.logbox.InvokeRequired)
                this.Invoke((Delegate)new MainForm.SetTextCallback(this.log), (object)text);
            else
            {
                this.logbox.AppendText("[Program] - " + text + "\r\n");
                logbox.SelectionStart = logbox.Text.Length;
                logbox.ScrollToCaret();
            }
        }
        #endregion

        #region SetParent
        [DllImport("User32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        static public void SetParent(IntPtr parentHandle, string childClassName, string childCaptionName)
        {
            // 지정한 윈도우를 찾음
            IntPtr childHandle = (IntPtr)FindWindow(childClassName, childCaptionName);
            if (childHandle != IntPtr.Zero) // 존재할 경우만
            {
                // 현재폼의 자식으로 설정
                SetParent(childHandle, parentHandle);

                // 현재폼에서의 위치와 크기로 다시 설정
                SetWindowPos(childHandle, IntPtr.Zero, -8, -85, 468, 421, 0);
            }
        }
        #endregion

        public async Task<string> curl_get(string siteurl)
        {
            string str = "";
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(siteurl);
                httpWebRequest.Method = "Get";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.106 Safari/535.2";
                httpWebRequest.ContentType = "application/x-www-form-urlencoded";
                httpWebRequest.KeepAlive = true;
                HttpWebResponse httpWebResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                TextReader textReader = (TextReader)new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                str = await textReader.ReadToEndAsync();
                httpWebResponse.Close();
                textReader.Close();
            }
            catch { }
            return str;
        }

        public async Task<bool> GetBlogs(string keyword, int findindex)
        {
            postIDList.Clear();
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromHours(180.00));
            int pagecount = 1;
            int findcount = 0;
            int tempindex = 1;
            for (; ; )
            {
                if (!workstatus)
                    break;

                try
                {
                    HttpWebRequest getreq = (HttpWebRequest)WebRequest.Create("https://search.naver.com/search.naver?where=post&sm=tab_pge&query=" + keyword + "&st=sim&date_option=0&date_from=&date_to=&dup_remove=1&post_blogurl=&post_blogurl_without=&srchby=all&nso=&ie=utf8&start=" + Convert.ToString(pagecount));
                    getreq.Method = "GET";
                    getreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
                    getreq.Referer = "http://www.naver.com/";
                    getreq.ContentType = "application/x-www-form-urlencoded";
                    getreq.KeepAlive = true;
                    HttpWebResponse webResponse = (HttpWebResponse)await getreq.GetResponseAsync();
                    if (webResponse.StatusCode == HttpStatusCode.OK)
                    {
                        TextReader streamReader = (TextReader)new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                        string strResult = await streamReader.ReadToEndAsync();
                        webResponse.Close();
                        streamReader.Close();

                        strResult = strResult.Replace("txt84", Environment.NewLine);

                        Regex r_bloger = new Regex(@"_sp_each_title"" href=""(.*)"" target=_blank", RegexOptions.IgnoreCase);

                        foreach (Match match in r_bloger.Matches(strResult))
                        {
                            bool continuestatus = true;
                            string blogerID = match.Groups[1].Value;

                            if (!workstatus)
                                break;

                            if (findcount >= findindex)
                                break;

                            try
                            {
                                if (blogerID.Contains("blog.me"))
                                {
                                    blogerID = Regex.Split(Regex.Split(blogerID, @"http:\/\/")[1], ".blog.me")[0];
                                    continuestatus = false;
                                }
                                else if (blogerID.Contains("blog.naver.com"))
                                {
                                    blogerID = Regex.Split(Regex.Split(blogerID, "blog.naver.com/")[1], @"\?Redirect")[0];
                                    continuestatus = false;
                                }

                                if (continuestatus)
                                    continue;

                                var items = lvBloger.Items.Cast<ListViewItem>();
                                bool exists = items.Where(item => (item.SubItems[1].Text == blogerID)).Any();
                                if (exists)
                                {
                                    this.log(blogerID + " 블로그는 이미 있어 추가되지않았습니다.");
                                }
                                else
                                {
                                    bool todaystatus = true;
                                    string blogtoday = string.Empty;
                                    //블로그 년도 감지
                                    this.driver.Navigate().GoToUrl("http://m.blog.naver.com/PostList.nhn?blogId=" + blogerID + "&categoryNo=0");
                                    wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
                                    if (chbToday.Checked)
                                    {
                                        blogtoday = this.driver.FindElements(By.CssSelector("div[class*='count']"))[0].GetAttribute("innerHTML");
                                        blogtoday = blogtoday.Replace(",", string.Empty);
                                        blogtoday = Regex.Split(Regex.Split(blogtoday, "오늘 ")[1], " <span")[0];
                                        if (Convert.ToInt32(blogtoday) >= Convert.ToInt32(txtToday1.Text) && Convert.ToInt32(blogtoday) <= Convert.ToInt32(txtToday2.Text))
                                        { }
                                        else
                                        {
                                            todaystatus = false;
                                            this.log(blogerID + " 블로그 추가실패 [투데이가 '" + blogtoday + "' 입니다.]");
                                        }
                                    }

                                    if (todaystatus)
                                    {
                                        string blogCategoryJson = (string)js.ExecuteScript(@"function httpGet(theUrl){var xmlHttp = new XMLHttpRequest();xmlHttp.open(""GET"", theUrl, false);xmlHttp.send(null);return xmlHttp.responseText;} return httpGet(""http://m.blog.naver.com/rego/CategoryList.nhn?blogId=" + blogerID + @""");");
                                        blogCategoryJson = blogCategoryJson.Replace(")]}',", string.Empty);
                                        var cateJsonConvert = JsonConvert.DeserializeObject<NaverBlogCategory.MainSer>(blogCategoryJson);
                                        var totalPostNum = cateJsonConvert.result.mylogPostCount;
                                        int total1 = Convert.ToInt32(totalPostNum) / 10;
                                        int total2 = Convert.ToInt32(totalPostNum) % 10;
                                        string blogPostsJson = string.Empty;
                                        if (total2 == 0)
                                        {
                                            blogPostsJson = (string)js.ExecuteScript(@"function httpGet(theUrl){var xmlHttp = new XMLHttpRequest();xmlHttp.open(""GET"", theUrl, false);xmlHttp.send(null);return xmlHttp.responseText;} return httpGet(""http://m.blog.naver.com/rego/PostListInfo.nhn?blogId=" + blogerID + @"&categoryNo=0&currentPage=" + Convert.ToString(total1) + @"&logCode=0"");");
                                        }
                                        else
                                        {
                                            blogPostsJson = (string)js.ExecuteScript(@"function httpGet(theUrl){var xmlHttp = new XMLHttpRequest();xmlHttp.open(""GET"", theUrl, false);xmlHttp.send(null);return xmlHttp.responseText;} return httpGet(""http://m.blog.naver.com/rego/PostListInfo.nhn?blogId=" + blogerID + @"&categoryNo=0&currentPage=" + Convert.ToString(total1 + 1) + @"&logCode=0"");");
                                        }
                                        blogPostsJson = blogPostsJson.Replace(")]}',", string.Empty);
                                        var blogPosts = JsonConvert.DeserializeObject<NaverBlogPosts.MainSer>(blogPostsJson);
                                        int postCount = blogPosts.result.postViewList.Count;
                                        if (postCount == 0)
                                        {
                                            blogPostsJson = (string)js.ExecuteScript(@"function httpGet(theUrl){var xmlHttp = new XMLHttpRequest();xmlHttp.open(""GET"", theUrl, false);xmlHttp.send(null);return xmlHttp.responseText;} return httpGet(""http://m.blog.naver.com/rego/PostListInfo.nhn?blogId=" + blogerID + @"&categoryNo=0&currentPage=" + Convert.ToString(total1) + @"&logCode=0"");");
                                            blogPostsJson = blogPostsJson.Replace(")]}',", string.Empty);
                                            blogPosts = JsonConvert.DeserializeObject<NaverBlogPosts.MainSer>(blogPostsJson);
                                            postCount = blogPosts.result.postViewList.Count;
                                        }
                                        string postnumber = blogPosts.result.postViewList[postCount - 1].logNo.ToString();
                                        this.driver.Navigate().GoToUrl("http://m.blog.naver.com/" + blogerID + "/" + postnumber);
                                        wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                                        Regex year = new Regex(@"num"">(.*)<\/em>", RegexOptions.IgnoreCase);
                                        Match yearMatch = year.Match(this.driver.PageSource.ToString());
                                        string postyear = yearMatch.Groups[1].Value;
                                        string[] DateSplit = postyear.Split(new string[1] { "." }, StringSplitOptions.RemoveEmptyEntries);
                                        if (Convert.ToInt32(DateSplit[0]) <= Convert.ToInt32(dtpYear.Text))
                                        {
                                            ++findcount;
                                            this.log(blogerID + " 블로그 추가완료");
                                            this.log("첫번째 글은 '" + postyear + "' 입니다.");
                                            if (chbToday.Checked)
                                                this.log("투데이는 '" + blogtoday + "' 입니다.");
                                            ListViewItem lvi = new ListViewItem(Convert.ToString(tempindex));
                                            lvi.SubItems.Add(blogerID);
                                            lvi.SubItems.Add("대기중");
                                            lvBloger.Items.Add(lvi);
                                            postIDList.Add(postnumber);
                                            ++tempindex;
                                        }
                                        else
                                        {
                                            this.log(blogerID + " 블로그 추가실패");
                                            this.log("첫번째 글은 '" + postyear + "' 입니다.");
                                            if (chbToday.Checked)
                                                this.log("투데이는 '" + blogtoday + "' 입니다.");
                                        }
                                    }
                                }
                            }
                            catch { this.log("알 수 없는 오류"); }
                            this.log("====================================================");
                            Thread.Sleep(2000);
                            Application.DoEvents();
                        }

                        if (findcount >= findindex)
                            break;

                        pagecount += 10;
                    }
                    Application.DoEvents();
                }
                catch { this.log("더 이상 키워드에 대한 탭이 없어 블로그추가를 종료합니다."); break; }
            }

            this.log("총 " + Convert.ToString(lvBloger.Items.Count) + " 개의 블로그를 추출하였습니다.");
            return true;
        }

        public async Task<string> GetBlogInfo(string blogID)
        {
            string str = string.Empty;
            HttpWebRequest getreq = (HttpWebRequest)WebRequest.Create("http://blog.naver.com/PostList.nhn?blogId=" + blogID);
            getreq.Method = "GET";
            getreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
            getreq.Referer = "http://www.naver.com/";
            getreq.ContentType = "application/x-www-form-urlencoded";
            getreq.KeepAlive = true;
            HttpWebResponse webResponse = (HttpWebResponse)await getreq.GetResponseAsync();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                TextReader streamReader = (TextReader)new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                str = await streamReader.ReadToEndAsync();
                webResponse.Close();
                streamReader.Close();
            }

            return str;
        }

        public async void GetGroups(string myid)
        {
            cmbGroup.Items.Clear();
            HttpWebRequest getreq = (HttpWebRequest)WebRequest.Create("http://section.blog.naver.com/connect/OpenBuddyGroupManage.nhn?blogId=" + myid + "&widgetSeq=364990");
            getreq.Method = "GET";
            getreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
            getreq.CookieContainer = logincookie;
            HttpWebResponse webResponse = (HttpWebResponse)await getreq.GetResponseAsync();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                TextReader streamReader = (TextReader)new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                string strResult = await streamReader.ReadToEndAsync();
                webResponse.Close();
                streamReader.Close();

                Regex r_groups = new Regex(@"buddygroup""><span>(.*)</span> <img src=", RegexOptions.IgnoreCase);

                foreach (Match match in r_groups.Matches(strResult))
                {
                    cmbGroup.Items.Add(match.Groups[1].Value);
                    Application.DoEvents();
                }
            }

            cmbGroup.SelectedIndex = 0;
        }

        public async Task<string> SendBuddy(string blogerID, string Message, string GroupID, string GroupName, int index)
        {
            string strResult = string.Empty;
            try
            {
                byte[] data = Encoding.ASCII.GetBytes("blogId=" + blogerID + "&buddyGroupView.groupId=" + GroupID + "&buddyGroupView.groupName=" + HttpUtility.UrlEncode(GroupName) + "&buddyGroupView.groupOpenYn=true&inviteMessage=" + HttpUtility.UrlEncode(Message) + "&returnUrl=http:%2F%2Fm.blog.naver.com%2F" + blogerID);
                HttpWebRequest postreq = (HttpWebRequest)HttpWebRequest.Create("http://m.blog.naver.com/rego/BothBuddyInviteCompleteAsync.nhn");
                postreq.Method = "POST";
                postreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0";
                postreq.Referer = "http://m.blog.naver.com/BuddyAddForm.nhn?blogId=" + blogerID + "&returnUrl=http:%2F%2Fm.blog.naver.com%2F" + blogerID;
                postreq.ContentType = "application/x-www-form-urlencoded";
                postreq.CookieContainer = logincookie;
                postreq.ContentLength = data.Length;

                Stream requestStream = await postreq.GetRequestStreamAsync();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();

                HttpWebResponse webResponse = (HttpWebResponse)await postreq.GetResponseAsync();
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = webResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                    strResult = await streamReader.ReadToEndAsync();

                    streamReader.Close();
                    responseStream.Close();
                    webResponse.Close();

                    return strResult;
                }
                else
                    return strResult;
            }
            catch
            {
                lvBloger.Items[index].SubItems[2].Text = "오류";
            }

            return strResult;
        }

        public async Task<string> SendNote(string blogID, string message)
        {
            string svcCode = "";
            string token = "";

            HttpWebRequest getreq = (HttpWebRequest)WebRequest.Create("http://m.note.naver.com/mobile/mobileSendNoteForm.nhn?returnUrl=http%3a%2f%2fm.note.naver.com%2fmobile%2fmobileReceiveList.nhn");
            getreq.Method = "GET";
            getreq.Host = "m.note.naver.com";
            getreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
            getreq.Referer = "http://m.note.naver.com/mobile/mobileReceiveList.nhn";
            getreq.ContentType = "application/x-www-form-urlencoded";
            getreq.KeepAlive = true;
            getreq.CookieContainer = logincookie;

            HttpWebResponse webResponse = (HttpWebResponse)await getreq.GetResponseAsync();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                TextReader streamReader = (TextReader)new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                string strResult = await streamReader.ReadToEndAsync();
                webResponse.Close();
                streamReader.Close();

                if (!String.IsNullOrEmpty(strResult))
                {
                    svcCode = Regex.Split(Regex.Split(strResult, @"svcCode"" value=""")[1], @""">")[0];
                    token = Regex.Split(Regex.Split(strResult, @"token"" value=""")[1], @""">")[0];
                }
            }

            byte[] data = Encoding.ASCII.GetBytes("fromUrl=%2Fmobile%2FmobileSendNoteForm.nhn%3FselfSend%3D0&returnUrl=%2Fmobile%2FmobileReceiveList.nhn&svcType=0&svcId=&svcName=&note=" + System.Web.HttpUtility.UrlEncode(message, Encoding.UTF8) + "&targetId=" + blogID + "&isBackup=true&svcCode=" + svcCode + "&isReplyNote=false&token=" + token);
            HttpWebRequest postreq = (HttpWebRequest)HttpWebRequest.Create("http://m.note.naver.com/mobile/mobileCaptchaViewCheck.nhn?");
            postreq.Method = "POST";
            postreq.Host = "m.note.naver.com";
            postreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
            postreq.Referer = "http://m.note.naver.com/mobile/mobileSendNoteForm.nhn";
            postreq.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
            postreq.CookieContainer = logincookie;
            postreq.ContentLength = data.Length;

            Stream requestStream = await postreq.GetRequestStreamAsync();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse webResponse2 = (HttpWebResponse)await postreq.GetResponseAsync();
            if (webResponse2.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = webResponse2.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);

                string strResult = await streamReader.ReadToEndAsync();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();

                if (strResult.Contains("스팸쪽지"))
                {
                    string captchaimage = Regex.Split(Regex.Split(strResult, @"captchaimg"" src=""")[1], @""" alt=""capcha")[0];
                    string key = Regex.Split(Regex.Split(strResult, @"key"" value=""")[1], @"""\/>")[0];
                    CaptchaForm frm = new CaptchaForm();
                    frm.logincookie = logincookie;
                    frm.message = message;
                    frm.blogID = blogID;
                    frm.svcCode = svcCode;
                    frm.token = token;
                    frm.key = key;
                    frm.captchaImage = captchaimage;
                    frm.ShowDialog();

                    return "성공";
                }
                else
                {
                    if (strResult.Contains("성공"))
                        return "성공";
                    else if (strResult.Contains("쪽지 수신 설정"))
                        return "수신설정";
                    else
                        return String.Empty;
                }
            }
            else
                return String.Empty;
        }

        public async Task<string> HttpUploadFile(string blogID, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string str = string.Empty;
            this.log(string.Format("{0} 을 {1} 에 업로드 시작", file, blogID));
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("http://upload.blog.naver.com/upload/AddImage.nhn?blogId=" + blogID + "&Cnt=&noencode=&imgPosition=top");
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.Host = "upload.blog.naver.com";
            wr.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
            wr.Referer = "http://blog.naver.com/post/add_image.jsp?blogId=" + blogID + "&Sposition=1";
            wr.KeepAlive = true;
            wr.CookieContainer = logincookie;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            Stream rs = await wr.GetRequestStreamAsync();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await rs.WriteAsync(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = await wr.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                str = await reader2.ReadToEndAsync();
                this.log("파일 업로드 완료");
            }
            catch (Exception ex)
            {
                this.log("업로드 실패 - 오류내용 : " + ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            return str;
        }

        public async Task<string> WriteGuest(string LoginID, string LoginNo, string LoginName, string blogID, string filelist, string html)
        {
            byte[] data = Encoding.ASCII.GetBytes("blogId=" + blogID + "&x=&y=&attachAllrealsize=" + selectedImageRealSize + "&attachMultimediaSize=0&attach=&attachsize=0&wpaperid=" + LoginID + "&wpaperno=" + LoginNo + "&wnickname=" + LoginName + "&totalfilelist=" + filelist + "&contents=" + html + "&attachAllsize=" + selectedImageSize);
            HttpWebRequest postreq = (HttpWebRequest)HttpWebRequest.Create("http://guestbook.blog.naver.com/guestbook/GuestBookWriteUpdate.nhn?blogId=" + blogID);
            postreq.Method = "POST";
            postreq.Host = "guestbook.blog.naver.com";
            postreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:49.0) Gecko/20100101 Firefox/49.0";
            postreq.Referer = "http://guestbook.blog.naver.com/guestbook/GuestBookWrite.nhn?blogId=" + blogID;
            postreq.ContentType = "application/x-www-form-urlencoded";
            postreq.CookieContainer = logincookie;
            postreq.ContentLength = data.Length;

            Stream requestStream = await postreq.GetRequestStreamAsync();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse webResponse = (HttpWebResponse)await postreq.GetResponseAsync();
            if (webResponse.StatusCode == HttpStatusCode.OK)
            {
                logincookie.Add(webResponse.Cookies);

                Stream responseStream = webResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.Default);

                string strResult = await streamReader.ReadToEndAsync();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();

                if (strResult.Contains("parent.location.href"))
                    return "성공";
                else
                    return strResult;
            }
            else
                return String.Empty;
        }

        public MainForm()
        {
            InitializeComponent();

            txtNoteText.Text = Properties.Settings.Default.Note;
            txtBuddyText.Text = Properties.Settings.Default.Buddy;
            txtGuestBookText.Text = Properties.Settings.Default.GuestBook;
            txtCommentText.Text = Properties.Settings.Default.Comment;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            GetGroups(NID);

            ChromeDriverService defaultService = ChromeDriverService.CreateDefaultService();
            defaultService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("window-size=500,500");
            options.AddArgument("window-position=-9999,-9999");
            options.AddArgument("--user-agent=Mozilla/5.0 (Linux; Android 4.0.3; HTC One X Build/IML74K) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166 Mobile Safari/535.19");
            options.AddArgument("--disable-notifications");
            options.AddArgument("disable-notifications");
            //패스워드 저장 요구창 제거
            options.AddUserProfilePreference("profile.password_manager_enabled", false);
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile", "{\"password_manager_enabled\":false");
            //크롬 내용 저장 (세션유지가능)
            options.AddArguments("--user-data-dir=" + Application.StartupPath + "\\Chrome\\User Data");

            this.driver = new ChromeDriver(defaultService, options);
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromHours(180.00));
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            this.driver.Manage().Cookies.DeleteAllCookies();
            SetParent(panel1.Handle, "Chrome_WidgetWin_1", "새 탭 - Chrome");
            driver.Navigate().GoToUrl("https://nid.naver.com/nidlogin.login?svctype=262144&url=http://m.naver.com/aside/");
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            driver.FindElement(By.Id("id")).Clear();
            driver.FindElement(By.Id("id")).SendKeys(NID);
            driver.FindElement(By.Id("pw")).Clear();
            driver.FindElement(By.Id("pw")).SendKeys(NPW);
            driver.FindElement(By.CssSelector("input.btn_global")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Note = txtNoteText.Text;
            Properties.Settings.Default.Buddy = txtBuddyText.Text;
            Properties.Settings.Default.GuestBook = txtGuestBookText.Text;
            Properties.Settings.Default.Comment = txtCommentText.Text;
            Properties.Settings.Default.Save();

            workstatus = false;
            try { th.Abort(); }
            catch { }
            driver.Quit();
            Application.Exit();
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "이미지파일을 선택해주세요.";
            ofd.Filter = "GIF|*.gif|JPG|*.jpg;*.jpeg|PNG|*.png|"
       + "모든 이미지 파일|*.jpg;*.jpeg;*.png;*.gif";
            ofd.InitialDirectory = ".";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fInfo = new FileInfo(ofd.FileName);
                string strFileSize = GetFileSize(fInfo.Length);
                if (Convert.ToInt32(fInfo.Length) > 524288)
                {
                    MessageBox.Show("이미지 크기가 500kb가 넘어갑니다.");
                    return;
                }
                picbGuestBookImage.Load(ofd.FileName);
                selectedImagePath = ofd.FileName;
                selectedImageName = ofd.SafeFileName;
                selectedImageSize = SplitFirst(strFileSize, '.');
                selectedImageRealSize = Convert.ToString(fInfo.Length);
            }
        }

        private void btnWork_Click(object sender, EventArgs e)
        {
            if (!workstatus)
            {
                workstatus = true;
                txtKeyword.Enabled = false;
                txtFindBlogNum.Enabled = false;
                txtChangeIPWorkcount.Enabled = false;
                txtNoteText.Enabled = false;
                txtBuddyText.Enabled = false;
                txtGuestBookText.Enabled = false;
                btnSelectImage.Enabled = false;
                chbUserDataring.Enabled = false;
                btnWork.Text = "정지";

                this.log("작업을 시작합니다.");

                this.th = new Thread(new ThreadStart(this.WorkProgram));
                this.th.Start();
                while (this.th.IsAlive)
                {
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
                this.th.Join();
            }
            else
            {
                workstatus = false;
                btnWork.Text = "시작";

                btnWork.Enabled = false;
                this.log("작업을 중지합니다. 기다려주세요.");

                th.Abort();

                btnWork.Enabled = true;
                txtKeyword.Enabled = true;
                txtFindBlogNum.Enabled = true;
                txtChangeIPWorkcount.Enabled = true;
                txtNoteText.Enabled = true;
                txtBuddyText.Enabled = true;
                txtGuestBookText.Enabled = true;
                btnSelectImage.Enabled = true;
                chbUserDataring.Enabled = true;
            }
        }

        [STAThread]
        public async void WorkProgram()
        {
            int workcount = 0;
            int tempcount = Convert.ToInt32(txtChangeIPWorkcount.Text);
            lvBloger.Items.Clear();
            lvUseIP.Items.Clear();
            
            //블로그 추가
            await GetBlogs(txtKeyword.Text, Convert.ToInt32(txtFindBlogNum.Text));

            for (int i = 0; i < lvBloger.Items.Count; ++i)
            {
                lvBloger.EnsureVisible(i);

                if (!workstatus)
                {
                    this.log("작업 중지요청이 있어 작업을 멈춥니다.");
                    return;
                }

                if (chbUserDataring.Checked)
                {
                    if (workcount > tempcount)
                    {
                        tempcount = tempcount * 2;

                        do
                        {
                            string AirModeOn = "shell settings put global airplane_mode_on 1; am broadcast -a android.intent.action.AIRPLANE_MODE --ez state true";
                            string AirModeOff = "shell settings put global airplane_mode_on 0; am broadcast -a android.intent.action.AIRPLANE_MODE --ez state false";

                            this.log("테더링 아이피를 변경합니다.");

                            var p = new System.Diagnostics.Process();
                            p.StartInfo.FileName = Application.StartupPath + @"\adb.exe";
                            p.StartInfo.Arguments = AirModeOn;
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            p.Start();
                            p.WaitForExit();

                            Thread.Sleep(500);

                            this.log("모바일 비행기모드를 켰습니다.");

                            var p2 = new System.Diagnostics.Process();
                            p2.StartInfo.FileName = Application.StartupPath + @"\adb.exe";
                            p2.StartInfo.Arguments = AirModeOff;
                            p2.StartInfo.CreateNoWindow = true;
                            p2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            p2.Start();
                            p2.WaitForExit();

                            this.log("모바일 비행기모드를 껐습니다.");
                            this.log("테더링 아이피 변경완료!");

                            this.log("정상적인 이용을위해 20초동안 기다립니다.");
                            Thread.Sleep(20000);
                            nowip = await getnowIP();

                            var items = lvUseIP.Items.Cast<ListViewItem>();
                            bool exists = items.Where(item => (item.Text == nowip)).Any();
                            if (!exists)
                            {
                                lvUseIP.Items.Add(nowip);
                                lvUseIP.Items[lvUseIP.Items.Count - 1].EnsureVisible();
                                break;
                            }
                            else
                                this.log("아이피가 이미 사용된 적이있어 다시변경합니다.");

                            if (!workstatus)
                            {
                                this.log("작업 중지요청이 있어 작업을 멈춥니다.");
                                return;
                            }
                        } while (true);
                    }
                }

                if (chbHaiIP.Checked)
                {
                    if (workcount > tempcount)
                    {
                        tempcount = tempcount * 2;

                        Process process = Process.GetProcessesByName("HaiipClientMulti")[0];
                        int hwnd1 = FindWindow(null, process.MainWindowTitle);
                        int hwnd2 = FindWindowEx(hwnd1, 0, "Button", null);
                        SendMessage(hwnd2, 0x00F5, 0, 1);

                        nowip = await getnowIP();
                    }
                }

                string blogerid = lvBloger.Items[i].SubItems[1].Text;

                this.log(blogerid + " 블로그 작업진행");
                //쪽지시작
                if (chbNote.Checked)
                {
                    try
                    {
                        string result = await SendNote(blogerid, txtNoteText.Text);
                        if (result == "성공")
                            this.log("쪽지 전송완료");
                        else if (result == "수신설정")
                            this.log("쪽지 수신설정에 의해 전송실패");
                        else
                            this.log("쪽지 전송실패");
                    }
                    catch
                    {
                        this.log("쪽지 전송실패");
                    }
                }
                //쪽지끝
                
                //서이추시작
                if (chbBuddy.Checked)
                {
                    string result = await SendBuddy(blogerid, txtBuddyText.Text, Convert.ToString(cmbGroup.SelectedIndex), cmbGroup.Text, i);
                    if (result.Contains("서로이웃 신청 진행중"))
                        this.log("서로이웃 신청 진행중");
                    else if (result.Contains("이미 서로이웃 상태"))
                        this.log("이미 서로이웃 상태");
                    else if (result.Contains("서로이웃 신청을 받지 않는"))
                        this.log("서로이웃 신청을 받지 않는 블로그");
                    else
                        this.log("신청완료");
                }
                //서이추끝

                //비밀댓글작성시작
                if (chbComment.Checked)
                {
                    SendComment(blogerid, txtCommentText.Text, postIDList[i]);
                }
                //비밀댓글작성끝

                //안부글시작
                if (chbGuestBook.Checked)
                {
                    try
                    {
                        string bloginfoResult = await GetBlogInfo(NID);
                        string wpaperid = Regex.Split(Regex.Split(bloginfoResult, "var blogId = '")[1], "' ||")[0];
                        string wpaperno = Regex.Split(Regex.Split(bloginfoResult, "var blogNo = '")[1], "';")[0];
                        string wpapername = Regex.Split(Regex.Split(bloginfoResult, "var nickName = '")[1], "';")[0];

                        Random r = new Random();
                        string RandomImageNo = Convert.ToString(r.Next(100000, 99999999));
                        string imageType = string.Empty;
                        string imagePosition = string.Empty;
                        string html = string.Empty;
                        if (selectedImagePath.Contains("jpg") & selectedImagePath.Contains("jpeg"))
                            imageType = "image/jpeg";
                        else if (selectedImagePath.Contains("png"))
                            imageType = "image/png";
                        else if (selectedImagePath.Contains("gif"))
                            imageType = "image/gif";

                        if (chbTop.Checked)
                            imagePosition = "top";
                        else if (chbBottom.Checked)
                            imagePosition = "bottom";
                        else if (chbLeft.Checked)
                            imagePosition = "left";
                        else if (chbRight.Checked)
                            imagePosition = "right";
                        NameValueCollection nvc = new NameValueCollection();
                        nvc.Add("imgOnlyYn", "1");
                        nvc.Add("attachAllsize", "0");
                        nvc.Add("myfilesize", "0");
                        nvc.Add("maxfilesize", "524288000");
                        nvc.Add("uploadPolicy", "GENERAL_IMAGE");
                        nvc.Add("isProfileThumbUpload", "false");
                        nvc.Add("imgPosition", imagePosition);
                        string resultStr = await HttpUploadFile(blogerid, selectedImagePath, selectedImageName, imageType, nvc);
                        Image img = Image.FromFile(selectedImagePath);
                        string fileurl = Regex.Split(Regex.Split(resultStr, @"SetValueImg\(""")[1], @"""\, """ + selectedImageRealSize)[0];
                        string filelist = System.Web.HttpUtility.UrlEncode(fileurl, Encoding.Default) + "@nhn@" + selectedImageRealSize + "@nhn@1@nhn@" + Convert.ToString(img.Width) + "@nhn@" + Convert.ToString(img.Height) + "@nhn@userImg" + RandomImageNo;
                        string content = txtGuestBookText.Text;
                        content = content.Replace(Environment.NewLine, "<br>");
                        content = System.Web.HttpUtility.UrlEncode(txtGuestBookText.Text, Encoding.Default);
                        content = content.Replace("%0a", "<br>");

                        if (chbTop.Checked)
                            html = @"<center><img src=""" + System.Web.HttpUtility.UrlEncode("http://blogfiles.naver.net" + fileurl, Encoding.Default) + @""" style=""cursor:hand"" onclick=""popview(this.src)"" width=""" + img.Width + @""" height=""" + img.Height + @"""><br>" + content + "</center><br>";
                        else if (chbBottom.Checked)
                            html = "<br>" + content + @"<center<img src=""" + System.Web.HttpUtility.UrlEncode("http://blogfiles.naver.net" + fileurl, Encoding.Default) + @""" style=""cursor:hand"" onclick=""popview(this.src)"" width=""" + img.Width + @""" height=""" + img.Height + @"""><br></center>";
                        else if (chbLeft.Checked)
                            html = @"<img src=""" + System.Web.HttpUtility.UrlEncode("http://blogfiles.naver.net" + fileurl, Encoding.Default) + @""" style=""cursor:hand"" onclick=""popview(this.src)"" width=""" + img.Width + @""" height=""" + img.Height + @""" align=""left"">" + content;
                        else if (chbRight.Checked)
                            html = content + @"<img src=""" + System.Web.HttpUtility.UrlEncode("http://blogfiles.naver.net" + fileurl, Encoding.Default) + @""" style=""cursor:hand"" onclick=""popview(this.src)"" width=""" + img.Width + @""" height=""" + img.Height + @""" align=""right"">";
                        string uploadResult = await WriteGuest(wpaperid, wpaperno, wpapername, blogerid, filelist, html);
                        if (uploadResult == "성공")
                        {
                            this.log("안부글 작성완료");
                        }
                        else
                        {
                            this.log("안부글 작성실패");
                        }
                    }
                    catch
                    {
                        this.log("안부글 작성실패");
                    }
                }
                //안부글끝

                this.log("====================================================");
                lvBloger.Items[i].SubItems[2].Text = "작업완료";
                ++workcount;
            }

            workstatus = false;
            btnWork.Text = "시작";
            btnWork.Enabled = true;
            btnWork.Enabled = true;
            txtKeyword.Enabled = true;
            txtFindBlogNum.Enabled = true;
            txtChangeIPWorkcount.Enabled = true;
            txtNoteText.Enabled = true;
            txtBuddyText.Enabled = true;
            txtGuestBookText.Enabled = true;
            btnSelectImage.Enabled = true;
            chbUserDataring.Enabled = true;

            try { th.Abort(); }
            catch { }
        }

        public static string SplitFirst(string source, char find)
        {
            string returnStr = string.Empty;
            string str = source;
            string[] parts = str.Split(find);
            foreach (string part in parts)
            {
                returnStr = part;
                break;

            }
            return returnStr;
        }

        private string GetFileSize(double byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0);
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0);
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0);
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString();

            return size;
        }

        public async Task<string> getnowIP()
        {
            string result = await curl_get("http://x-y.net/ip/");
            return Regex.Split(Regex.Split(result, @"아이피는 ")[1], @" 입니다.")[0];
        }

        private void chbTop_CheckedChanged(object sender, EventArgs e)
        {
            if (chbTop.Checked)
            {
                chbTop.Enabled = false;
                chbBottom.Enabled = true;
                chbLeft.Enabled = true;
                chbRight.Enabled = true;
                chbTop.Checked = true;
                chbBottom.Checked = false;
                chbLeft.Checked = false;
                chbRight.Checked = false;
            }
        }

        private void chbBottom_CheckedChanged(object sender, EventArgs e)
        {
            if (chbBottom.Checked)
            {
                chbTop.Enabled = true;
                chbBottom.Enabled = false;
                chbLeft.Enabled = true;
                chbRight.Enabled = true;
                chbTop.Checked = false;
                chbBottom.Checked = true;
                chbLeft.Checked = false;
                chbRight.Checked = false;
            }
        }

        private void chbLeft_CheckedChanged(object sender, EventArgs e)
        {
            if (chbLeft.Checked)
            {
                chbTop.Enabled = true;
                chbBottom.Enabled = true;
                chbLeft.Enabled = false;
                chbRight.Enabled = true;
                chbTop.Checked = false;
                chbBottom.Checked = false;
                chbLeft.Checked = true;
                chbRight.Checked = false;
            }
        }

        private void chbRight_CheckedChanged(object sender, EventArgs e)
        {
            if (chbRight.Checked)
            {
                chbTop.Enabled = true;
                chbBottom.Enabled = true;
                chbLeft.Enabled = true;
                chbRight.Enabled = false;
                chbTop.Checked = false;
                chbBottom.Checked = false;
                chbLeft.Checked = false;
                chbRight.Checked = true;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            chbTop.Checked = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            chbBottom.Checked = true;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            chbLeft.Checked = true;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            chbRight.Checked = true;
        }

        private void chbUserDataring_CheckedChanged(object sender, EventArgs e)
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Application.StartupPath + @"\\adb.exe";
            p.StartInfo.Arguments = "devices";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();

            string Output = null;
            using (StreamReader reader = p.StandardOutput)
            {
                Output = reader.ReadToEnd();
            }

            string reOutput = GetLine(Output, 2);
            if (reOutput == "")
            {
                MessageBox.Show("모바일 테더링이 연결되어 있지 않습니다." + Environment.NewLine + "연결후에 프로그램을 다시 켜주세요.", "테더링 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                chbUserDataring.Checked = false;
            }
            else if (!reOutput.Contains("device"))
            {
                MessageBox.Show("모바일 테더링과의 연결이 정상적이지 않습니다." + Environment.NewLine +
                                "========다음경우에 오류가 발생합니다========" + Environment.NewLine +
                                "1.USB 디버깅모드가 꺼졌을경우" + Environment.NewLine +
                                "2.USB 디버깅 권한 승인이 되지않았을경우" + Environment.NewLine +
                                "3.연결모드가 미디어전송(MTP)가 아닐경우" + Environment.NewLine +
                                "4.핸드폰 드라이버가 컴퓨터에 설치가 되지않았을경우", "테더링 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                chbUserDataring.Checked = false;
            }
        }

        string GetLine(string text, int lineNo)
        {
            string[] lines = text.Replace("\r", "").Split('\n');
            return lines.Length >= lineNo ? lines[lineNo - 1] : null;
        }

        private void chbHaiIP_CheckedChanged(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("HaiipClientMulti");
            if (processes.Length > 0)
            {}
            else
            {
                MessageBox.Show("하이IP 프로그램이 켜져있지 않습니다.");
                chbHaiIP.Checked = false;
            }
        }

        public void SendComment(string blogerID, string Message, string PostID)
        {
            IWait<IWebDriver> wait = new WebDriverWait(driver, TimeSpan.FromHours(180.00));

            driver.Navigate().GoToUrl("http://m.blog.naver.com/CommentList.nhn?blogId=" + blogerID + "&logNo=" + PostID);
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(500);

            driver.FindElement(By.CssSelector("a[class='link_reply']")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(500);

            driver.FindElement(By.XPath("//div[@id='ct']/div[3]/form/fieldset/div[3]/div/textarea")).Clear();
            driver.FindElement(By.XPath("//div[@id='ct']/div[3]/form/fieldset/div[3]/div/textarea")).SendKeys(Message);
            driver.FindElement(By.CssSelector("button.btn_private")).Click();
            driver.FindElement(By.LinkText("완료")).Click();
            wait.Until(driver1 => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
            Thread.Sleep(500);

            this.log("댓글 작성완료");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendComment("ubin1024", txtCommentText.Text, "10153079395");
        }
    }
}
