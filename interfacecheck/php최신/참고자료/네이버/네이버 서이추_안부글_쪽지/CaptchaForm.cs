using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 네이버_서이추_안부글_쪽지
{
    public partial class CaptchaForm : Form
    {
        public CookieContainer logincookie = new CookieContainer();
        public string message = String.Empty;
        public string blogID = String.Empty;
        public string svcCode = String.Empty;
        public string token = String.Empty;
        public string key = String.Empty;
        public string captchaImage = String.Empty;

        async Task<string> SendNote(string CaptchaCode)
        {
            string returnstr = string.Empty;

            byte[] data = Encoding.ASCII.GetBytes("key=" + key + "&captchavalue=" + CaptchaCode + "&fromUrl=%2Fmobile%2FmobileSendNoteForm.nhn%3FselfSend%3D0&returnUrl=http%3A%2F%2Fm.note.naver.com%2Fmobile%2FmobileReceiveList.nhn&svcType=0&svcId=&svcName=&note=" + System.Web.HttpUtility.UrlEncode(message, Encoding.UTF8) + "&targetId=" + blogID + "&svcCode=" + svcCode + "&isBackup=true&token=" + token);
            HttpWebRequest postreq = (HttpWebRequest)HttpWebRequest.Create("http://m.note.naver.com/mobile/mobileCheckCaptcha.nhn?");
            postreq.Method = "POST";
            postreq.Host = "m.note.naver.com";
            postreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:52.0) Gecko/20100101 Firefox/52.0";
            postreq.Referer = "http://m.note.naver.com/mobile/mobileSendNoteForm.nhn";
            postreq.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
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

                string strResult = await streamReader.ReadToEndAsync();

                streamReader.Close();
                responseStream.Close();
                webResponse.Close();

                if (strResult.Contains("성공"))
                    returnstr = "성공";
                else if (strResult.Contains("쪽지 수신 설정"))
                    returnstr = "수신설정";
                else if (strResult.Contains("스팸쪽지"))
                {
                    captchaImage = Regex.Split(Regex.Split(strResult, @"captchaimg"" src=""")[1], @""" alt=""capcha")[0];
                    key = Regex.Split(Regex.Split(strResult, @"key"" value=""")[1], @"""\/>")[0];
                    LoadCaptchaImage();
                }
            }

            return returnstr;
        }

        public CaptchaForm()
        {
            InitializeComponent();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = await SendNote(txtCode.Text);
            if (result == "성공")
            {
                this.Close();
            }
            else if (result == "수신설정")
            {
                this.Close();
            }

            txtCode.Clear();
            this.ActiveControl = txtCode;
        }

        private void CaptchaForm_Load(object sender, EventArgs e)
        {
            LoadCaptchaImage();
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSubmit.PerformClick();
        }

        void LoadCaptchaImage()
        {
            HttpWebRequest lxRequest = (HttpWebRequest)WebRequest.Create(captchaImage);
            lxRequest.CookieContainer = logincookie;

            String lsResponse = string.Empty;
            using (HttpWebResponse lxResponse = (HttpWebResponse)lxRequest.GetResponse())
            {
                using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                {
                    Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                    picbCaptcha.Image = ByteToImage(lnByte); 
                    
                    //이미지저장
                    //using (FileStream lxFS = new FileStream("./captcha.jpg", FileMode.Create))
                    //{
                    //    lxFS.Write(lnByte, 0, lnByte.Length);
                    //}
                }
            }
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
    }
}
