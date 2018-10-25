using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrawlerWinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            rctxtResult.Text = GetResponse(txtUrl.Text);
        }

        private void btnParser_Click(object sender, EventArgs e)
        {

            string text = GetPlainTextFromHtml(rctxtResult.Text);

            //http://www.milliyet.com.tr/aksal-yavuz/yusuf-ve-abdulkadir-e-destek-2766396-skorer-yazar-yazisi/
            if (txtUrl.Text.IndexOf("http://www.milliyet.com.tr/yazarlar/") == -1)
            {
                text = Crop(text, "A+A-", "Yazarın tüm yazıları");
            }
            else //http://www.milliyet.com.tr/yazarlar/abbas-guclu/en-guclu-pasaport--2759676/
            {
                text = Crop(text, "Tüm Yazıları", "Yazarın Diğer Yazıları");
            }

            rctxtResult.Text = text;

        }

        private static string Crop(string text, string topCrop, string botomCrop)
        {
            int index = text.IndexOf(topCrop);
            if (index > 0)
                text = text.Remove(0, index + topCrop.Length);

            index = text.IndexOf(botomCrop);
            if (index > 0)
                text = text.Remove(index).Trim();

            return text;
        }

        private string GetResponse(string url)
        {
            WebRequest webRequest = HttpWebRequest.Create(url);
            WebResponse webResponse = webRequest.GetResponse();
            StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());

            return streamReader.ReadToEnd();
        }

        private string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }

    }
}
