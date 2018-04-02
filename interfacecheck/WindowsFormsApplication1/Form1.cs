using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using WinHttp;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        WebClient WC = new WebClient();
        public WinHttpRequest winhttp = new WinHttpRequest();

        public Form1()
        {
            InitializeComponent();
        }
        private void msgbox(string 내용, string 제목)
        {
            MessageBox.Show(내용, 제목);
        }
        public void ReDim(ref String[] Ary)
        {
            int i;
            i = Ary.Length;
            Array.Resize(ref Ary, i + 1);
            Ary[i] = null;
        }

        private String[] PersingM(String Str, String Str_Start, String Str_End)
        {
            String Source = Str;
            String[] Result = { null };
            int Count = 0;
            while (Source.IndexOf(Str_Start) > -1)
            {
                ReDim(ref Result);
                Source = Source.Substring(Source.IndexOf(Str_Start) + Str_Start.Length);
                if (Source.IndexOf(Str_End) != -1)
                {
                    Result[Count] = Source.Substring(0, Source.IndexOf(Str_End));
                }
                else return Result;
                Count++;
            }
            return Result;
        }

        private String PersingS(String Str, String Str_Start, String Str_End)
        {
            String Source = Str;
            String Result = null;
            Source = Source.Substring(Source.IndexOf(Str_Start) + Str_Start.Length);
            Result = Source.Substring(0, Source.IndexOf(Str_End));
            return Result;
        }

  


        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("학번", 100); listView1.Columns.Add("이름", 100);
            listView1.Columns.Add("번호", 100); /* * 다섯가지 모양을 가질 수 있다. * 큰아이콘, 작은아이콘, 리스트, 상세히, 타일모양 등 */ listView1.View = View.Details; listView1.FullRowSelect = true; listView1.GridLines = true;
            listView1.Columns.Add("학과", 100);
            listView1.View = View.Details; listView1.FullRowSelect = true; listView1.GridLines = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            WebClient WC = new WebClient();
            WC.Encoding = Encoding.UTF8;//한글 안깨지게.

            string DownLoad = WC.DownloadString("http://interface518.dothome.co.kr/inter/getjson.php");
            textBox1.Text = DownLoad;
            //using System.Text.RegularExpressions;

            List<string> hide = new List<string>();

            string data = WC.DownloadString("http://interface518.dothome.co.kr/inter/getjson.php");

            Regex regex = new Regex("id\": \"(.*)\",");

            MatchCollection mc = regex.Matches(data);

            foreach (Match m in mc)

            {

                for (int i = 1; i < m.Groups.Count; i++)

                {
                    listView1.Items.Add(m.Groups[i].Value);
                    
                    listBox1.Items.Add(m.Groups[i].Value);
                    //hide.Add(m.Groups[i].Value);



                }

            }
            Regex regex2 = new Regex("name\": \"(.*)\",");

            MatchCollection mc2 = regex2.Matches(data);

            foreach (Match m2 in mc2)
            {

                for (int i = 1; i < m2.Groups.Count; i++)

                {
                    listView1.Items[i].SubItems.Add(m2.Groups[i].Value);

                    listBox1.Items.Add(m2.Groups[i].Value);
                    //hide.Add(m.Groups[i].Value);



                }

            }




            /*
            WebClient WC = new WebClient();
            WC.Encoding = Encoding.UTF8;//한글 안깨지게.
            
            string DownLoad = WC.DownloadString("http://interface518.dothome.co.kr/inter/getjson.php");
            textBox1.Text = DownLoad;
            //": "1234"
            
            listBox1.Items.Add(PersingM(DownLoad, "id\": \"", "\",") 
                +" "+PersingM(DownLoad, "name\": \"", "\",") 
                + " " + PersingM(DownLoad, "address\": \"", "\",") 
                + " " + PersingM(DownLoad, "department\": \"", "\""));

            for (int a = 1; a <= 2; a++)
            {
                string[] parsing1 = System.Text.RegularExpressions.Regex.Split(winhttp.ResponseText, "<form action=\"http://search.naver.com/search.naver\">");
                

                listBox1.Items.Add(a + "위 " + parsing1[0]);
            }

            
           string[] parsing = System.Text.RegularExpressions.Regex.Split(winhttp.ResponseText, "<form action=\"http://search.naver.com/search.naver\">");

           for (int a = 1; a <= 10; a++)
           {
               string[] parsing1 = parsing[1].Split(new string[] { "<option value=\"" }, 0);
               string[] parsing2 = parsing1[a].Split(new string[] { "\"" }, 0);

               listBox1.Items.Add(a + "위 " + parsing2[0]);
           }*/
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string text = "";
            //text = Encoding.UTF8.GetString(textBox2.Text);
            winhttp.Open("POST", "http://interface518.dothome.co.kr/inter/ADadd.php?");
            winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            winhttp.Send("contents=" + textBox2.Text);
            winhttp.WaitForResponse();
            //textBox1.ToString();

             textBox1.Text = Encoding.Default.GetString(winhttp.ResponseBody);


           
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string text = "";
            //text = Encoding.UTF8.GetString(textBox2.Text);
            winhttp.Open("POST", "http://interface518.dothome.co.kr/inter/query.php?", false);
            winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=euckr");
            winhttp.Send("id=" + textBox3.Text);
            winhttp.WaitForResponse();
            //textBox1.ToString();

            textBox1.Text = Encoding.Default.GetString(winhttp.ResponseBody);
            if (textBox1.Text.Contains("찾을"))
                listBox1.Items.Add("없는 사람일세");
            else
            listBox1.Items.Add(PersingS(textBox1.Text, "[id] => ", "["));
            listBox1.Items.Add(PersingS(textBox1.Text, "[name] =>", "["));
            listBox1.Items.Add(PersingS(textBox1.Text, "[address] =>", "["));
            listBox1.Items.Add(PersingS(textBox1.Text, " [department] =>", ")"));

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString("http://interface518.dothome.co.kr/inter/getjson.php"); //API 사이트에서 json 받아옴
                JObject jobj = JObject.Parse(json); //json 객체로
                
                    MessageBox.Show(jobj["id"].ToString() + " , 버전 : " + jobj["name"].ToString() + "￦n" + jobj["address"].ToString()); //플러그인명,버전,url 출력
            }
        }

        private void listView1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            //http://interface518.dothome.co.kr/inter/delete.php
            winhttp.Open("POST", "http://interface518.dothome.co.kr/inter/delete.php?");
            winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
            winhttp.Send("id=" + textBox3.Text);
            winhttp.WaitForResponse();
            //textBox1.ToString();

            textBox1.Text = Encoding.Default.GetString(winhttp.ResponseBody);
        }
    }
}
