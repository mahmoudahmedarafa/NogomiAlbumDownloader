using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nogomi_Crawler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "Nogomi Album Downloader";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text;
            WebClient webClient = new WebClient();

            string html = webClient.DownloadString(url);
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var links_list = doc.DocumentNode
           .Descendants("tr")
           .Where(tr => tr.GetAttributeValue("class", "").Contains("IndDivmainbox_m_left_Top_Albums_thumb_2_inner_tblRow2"))
           .SelectMany(tr => tr.Descendants("a"))
           .ToList();

            for (int i = 0; i < links_list.Count; i += 4)
            {
                string tmp = links_list[i].OuterHtml;
                var regex = new Regex("<a [^>]*href=(?:'(?<href>.*?)')|(?:\"(?<href>.*?)\")", RegexOptions.IgnoreCase);
                string song_url = regex.Matches(tmp).OfType<Match>().Select(m => m.Groups["href"].Value).SingleOrDefault();

                html = webClient.DownloadString(song_url);
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var links = doc.DocumentNode.Descendants("a").ToList();

                foreach (HtmlNode item in links)
                {
                    if (item.OuterHtml.Contains("c=mp3"))
                    {
                        regex = new Regex("<a [^>]*href=(?:'(?<href>.*?)')|(?:\"(?<href>.*?)\")", RegexOptions.IgnoreCase);
                        string download_url = regex.Matches(item.OuterHtml).OfType<Match>().Select(m => m.Groups["href"].Value).FirstOrDefault();
                        System.Diagnostics.Process.Start(download_url);
                    }
                }
            }

        }
           
    }
}
