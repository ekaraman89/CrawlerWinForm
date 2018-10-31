using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CrawlerWinForm
{
    public class Arff
    {
        string Description;
        string Path;
        StreamWriter streamWriter;

        public enum ArffType
        {
            String,
            Number,
        }

        Dictionary<string, string> attibutes;

        public Arff(string Description, string Path, string FileName)
        {
            this.Description = Description;
            this.Path = System.IO.Path.Combine(Path, $"{FileName}.arff");

            attibutes = new Dictionary<string, string>();
        }

        public void AddAttribute(string name, ArffType type)
        {
            attibutes.Add(type.ToString(), name);
        }

        public void AddAttribute(params string[] values)
        {
            string att = string.Empty;
            foreach (string item in values)
            {
                att += $"{item},";
            }

            attibutes.Add("class", "{" + att.TrimEnd(',') + "}");
        }

        private void CreateFile()
        {           
            using (streamWriter = new StreamWriter(Path))
            {
                CreateHeader();
                CreateAttribute();
            }
        }

        private void CreateHeader()
        {
            streamWriter.WriteLine("@relation {0}", QuoteAndEscape(Description));
            streamWriter.WriteLine();
        }

        private void CreateAttribute()
        {
            foreach (var item in attibutes)
            {
                streamWriter.WriteLine("@attribute {0} {1}", item.Key, QuoteAndEscape(item.Value).Replace("'", string.Empty));
            }
            streamWriter.WriteLine();
            streamWriter.WriteLine("@data");
            streamWriter.WriteLine();
        }

        public void AddData(params string[] data)
        {
            if (!File.Exists(Path))
            {
                CreateFile();
            }

            string _data = string.Empty;
            foreach (string item in data)
            {
                _data += $"{QuoteAndEscape(item)},";
            }

            File.AppendAllText(Path, $"{_data.TrimEnd(',')}" + Environment.NewLine);
        }

        static string QuoteAndEscape(string s)
        {
            if (s == string.Empty)
                return "''";
            if (s == "?")
                return "'?'";

            StringBuilder stringBuilder = new StringBuilder(s.Length + 2);

            bool quote = false;

            foreach (char c in s)
                switch (c)
                {
                    case '"':
                        stringBuilder.Append("\\\"");
                        quote = true;
                        break;
                    case '\'':
                        stringBuilder.Append("\\'");
                        quote = true;
                        break;
                    case '%':
                        stringBuilder.Append("\\%");
                        quote = true;
                        break;
                    case '\\':
                        stringBuilder.Append("\\\\");
                        quote = true;
                        break;
                    case '\r':
                        stringBuilder.Append("\\r");
                        quote = true;
                        break;
                    case '\n':
                        stringBuilder.Append("\\n");
                        quote = true;
                        break;
                    case '\t':
                        stringBuilder.Append("\\t");
                        quote = true;
                        break;
                    case '\u001E':
                        stringBuilder.Append("\\u001E");
                        quote = true;
                        break;
                    case ' ':
                    case ',':
                    case '{':
                    case '}':
                        stringBuilder.Append(c);
                        quote = true;
                        break;
                    default:
                        stringBuilder.Append(c);
                        break;
                }

            if (quote)
            {
                stringBuilder.Insert(0, '\'');
                stringBuilder.Append('\'');
            }

            return stringBuilder.ToString();
        }

    }
}
