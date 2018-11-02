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

        string Category = "Diğer";

        private void btnParser_Click(object sender, EventArgs e)
        {
            GetCategory();

            string text = GetPlainTextFromHtml(rctxtResult.Text);

            //http://www.milliyet.com.tr/yazar-adi/
            if (txtUrl.Text.IndexOf("http://www.milliyet.com.tr/yazarlar/") == -1)
            {
                text = Crop(text, "A+A-", "Yazarın tüm yazıları");
            }
            else //http://www.milliyet.com.tr/yazarlar/....
            {
                text = Crop(text, "Tüm Yazıları", "Yazarın Diğer Yazıları");
            }

            rctxtResult.Text = text;
        }

        private void GetCategory()
        {
            string catRegex = "<div class=\"dtyTop\"><div class=\"dTTabs\"><div class=\"kat\"><a href=\"/.*";

            foreach (Match item in Regex.Matches(rctxtResult.Text, catRegex))
            {
                Category = Crop(item.Value, "/\">", "</a></div></div></div>");
            }
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

        private void btnWriteToFile_Click(object sender, EventArgs e)
        {
            string text = rctxtResult.Text;

            string path = "Files";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string description = "Makale kategorisi bulma";

            CreateFile("Original.txt", text);
            CreateArffFile(description, path, "Original", text, Category);
            CreateArffFile(description, path, "WithoutStopWordOriginal", StopwordTool.RemoveStopwords(text), "test");
        }

        private void CreateArffFile(string Description, string Path, string FileName, string TextData, string Category)
        {
            Arff arff = new Arff(Description, Path, FileName);
            arff.AddAttribute("text", Arff.ArffType.String);
            arff.AddAttribute(new string[] { "SİYASET", "GÜNDEM", "EKONOMİ","DÜNYA" });
            arff.AddData(new string[] { TextData, Category });
        }

        private void CreateFile(string FileName, string Text)
        {
            using (StreamWriter sw = new StreamWriter(FileName, true))
            {
                sw.WriteLine(Text);
            }
        }

    }
}

