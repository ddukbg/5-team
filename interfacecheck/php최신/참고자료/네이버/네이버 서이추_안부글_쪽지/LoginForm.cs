using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 네이버_서이추_안부글_쪽지
{
    public partial class LoginForm : Form
    {
        CookieContainer logincookie = new CookieContainer();

        public async Task<bool> Login(string id, string pw)
        {
            byte[] data = Encoding.ASCII.GetBytes("id=" + id + "&pw=" + pw + "&saveID=0&enctp=2&smart_level=-1&svctype=0");
            HttpWebRequest postreq = (HttpWebRequest)HttpWebRequest.Create("https://nid.naver.com/nidlogin.login");
            postreq.Method = "POST";
            postreq.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:51.0) Gecko/20100101 Firefox/51.0";
            postreq.Referer = "https://nid.naver.com/nidlogin.login?svctype=262144&url=http://m.naver.com/aside/";
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
                if (strResult.Contains("https://nid.naver.com/login/sso/finalize.nhn?url"))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public LoginForm()
        {
            InitializeComponent();

            txtID.Text = Properties.Settings.Default.NID;
            txtPW.Text = Properties.Settings.Default.NPW;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text) && string.IsNullOrEmpty(txtPW.Text))
            {
                MessageBox.Show("아이디 또는 비밀번호를 입력해주세요.");
                return;
            }

            btnLogin.Text = "잠시만 기다려주세요...";
            txtID.Enabled = false;
            txtPW.Enabled = false;
            btnClearNID.Enabled = false;
            btnClearNPW.Enabled = false;
            btnLogin.Enabled = false;

            bool loginstatus = await Login(txtID.Text, txtPW.Text);

            if (loginstatus)
            {
                Properties.Settings.Default.NID = txtID.Text;
                Properties.Settings.Default.NPW = txtPW.Text;
                Properties.Settings.Default.Save();
                MainForm frm = new MainForm();
                frm.NID = txtID.Text;
                frm.NPW = txtPW.Text;
                frm.logincookie = this.logincookie;
                frm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("로그인실패!\n아이디 또는 비밀번호를 확인해주세요.");
            }

            btnLogin.Text = "로그인";
            txtID.Enabled = true;
            txtPW.Enabled = true;
            btnClearNID.Enabled = true;
            btnClearNPW.Enabled = true;
            btnLogin.Enabled = true;
        }

        private void txtPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin.PerformClick();
        }

        private void btnClearNID_Click(object sender, EventArgs e)
        {
            txtID.Clear();
        }

        private void btnClearNPW_Click(object sender, EventArgs e)
        {
            txtPW.Clear();
        }
    }
}
