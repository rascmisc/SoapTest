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
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace SoapTest
{
    public partial class Form1 : Form
    {
        public FiapXml fiapXml;

        public Form1()
        {
            InitializeComponent();
            Settings settings = new Settings();

            FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + "settings.xml", FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            settings = (Settings)serializer.Deserialize(fs);

            textBox1.Text = settings.keyid;
            textBox4.Text = settings.lteq;
            textBox5.Text = settings.gteq;

            fiapXml = new FiapXml();
            fs.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            fiapXml.MakeFiapMessage(textBox1.Text,textBox4.Text,textBox5.Text);
            try
            {
                //WebRequestの作成
                string url = "https://storage.m-access.net/soaps/";

                string postData;
                byte[] postDataBytes;
                postData = fiapXml.xml;

                postDataBytes = System.Text.Encoding.ASCII.GetBytes(postData);
                //文字コードを指定する
                System.Text.Encoding enc =
                    System.Text.Encoding.GetEncoding("shift_jis");

                //SSL/TLS対応
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                //メソッドにPOSTを指定
                req.Method = "POST";
                //ContentTypeを"application/x-www-form-urlencoded"にする
                req.ContentType = "text/xml";
                //POST送信するデータの長さを指定
                req.ContentLength = postDataBytes.Length;
                req.Timeout = 3000;

                //データをPOST送信するためのStreamを取得
                System.IO.Stream reqStream = req.GetRequestStream();
                //送信するデータを書き込む
                reqStream.Write(postDataBytes, 0, postDataBytes.Length);
                reqStream.Close();
                textBox2.Text = postData;

                //サーバーからの応答を受信するためのWebResponseを取得
                System.Net.WebResponse res = req.GetResponse();
                //応答データを受信するためのStreamを取得
                System.IO.Stream resStream = res.GetResponseStream();
                //受信して表示
                System.IO.StreamReader sr = new System.IO.StreamReader(resStream, enc);
                textBox3.Text = sr.ReadToEnd();
                //閉じる
                sr.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }
        }
        public class MyPolicy : ICertificatePolicy
        {
            public bool CheckValidationResult(
                  ServicePoint srvPoint
                , X509Certificate certificate
                , WebRequest request
                , int certificateProblem)
            {

                //Return True to force the certificate to be accepted.
                return true;

            } // end CheckValidationResult
        } // class MyPolicy

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //格納
            Settings settings = new Settings();

            settings.keyid = textBox1.Text;
            settings.lteq = textBox4.Text;
            settings.gteq = textBox5.Text;

            // XmlSerializerを使ってファイルに保存
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            // カレントディレクトリに"settings.xml"というファイル名で書き出す
            FileStream fs = new FileStream(Directory.GetCurrentDirectory() + "\\" + "settings.xml", FileMode.Create);
            // オブジェクトをシリアル化してXMLファイルに書き込む
            serializer.Serialize(fs, settings);
            fs.Close();

        }
    }
}
