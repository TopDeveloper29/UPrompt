using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;


namespace UPrompt
{
    public partial class Prompt : MaterialForm   
    {
        public string ViewHtml = "";
        public string Xml_Path = @"C:\Users\beah\source\repos\TopDeveloper29\UPrompt\DEMO.xml";

        internal XmlDocument xmlDoc = new XmlDocument();
        private string app_path = (AppDomain.CurrentDomain.BaseDirectory).Replace(@"\", "/");
        public Prompt()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void Prompt_Load(object sender, EventArgs e){ LoadXml(Xml_Path); InternalProcess.Windows = this; }
        public void LoadXml(string path)
        {
            try
            {
                xmlDoc.Load(path);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fatal error on XML Parsing", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            XmlNodeList settingsList = xmlDoc.SelectNodes("//Application/Settings");
            InternalProcess.LoadSettings(settingsList);
            GenerateView();
        }
        public void GenerateView()
        {
            ViewHtml = "";
            XmlNode viewNode = xmlDoc.SelectSingleNode("/Application/View");
            foreach (XmlNode childNode in viewNode.ChildNodes)
            {
                ViewHtml = HtmlXml.GenrateHtmlFromXML(childNode.OuterXml);
            }
            if (ViewHtml.Length < 5) { ViewHtml = "=== THE VIEW IS EMPTY PLEASE FILL IT IN XML ==="; }

            string Template = File.ReadAllText($"{app_path}Index.html");
            string html = Template.Replace("=== XML CODE WILL GENERATE THIS VIEW ===", ViewHtml);

            File.WriteAllText($"{app_path}View.html",HtmlXml.SettingsTextParse(html));

            string html_path = $@"file:///{app_path}View.html";
            htmlhandler.Navigate($"{(string)html_path}");
        }

        private void htmlhandler_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().Contains("="))
            {
                if (e.Url.ToString().Contains("&"))
                {
                    string action_code = System.Net.WebUtility.UrlDecode(e.Url.ToString()).Split('?')[1];
                    string[] actions = action_code.Split('&');
                    foreach(string action in actions)
                    {
                        string action_name = action.Split('=')[0];
                        string action_value = HtmlXml.SpecialTextParse(action.Split('=')[1]);
                        if (action_name.Contains("INPUT_"))
                        {
                           InternalProcess.RunAction(action_name, action_value);
                        }
                    }
                    foreach (string action in actions)
                    {
                        string action_name = action.Split('=')[0];
                        string action_value = HtmlXml.SpecialTextParse(action.Split('=')[1]);
                        if (!action_name.Contains("INPUT_"))
                        {
                            InternalProcess.RunAction(action_name, action_value);
                        }
                    }
                }
                else
                {
                    string action_code = System.Net.WebUtility.UrlDecode(e.Url.ToString()).Split('?')[1];
                    string action_name = action_code.Split('=')[0];
                    string action_value = HtmlXml.SpecialTextParse(action_code.Split('=')[1]);
                    InternalProcess.RunAction(action_name, action_value);
                }
            }
        }
        
        

    }
}
