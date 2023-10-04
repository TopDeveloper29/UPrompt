using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using UPrompt.Properties;

namespace UPrompt
{
    public partial class Prompt : MaterialForm   
    {
        string app_path = (AppDomain.CurrentDomain.BaseDirectory).Replace(@"\","/");
        internal XmlDocument xmlDoc = new XmlDocument();
        private string HtmlFromXml = "";
        private string Text_Color = "#000000";
        private string Back_Color = "#ffffff";
        private string Main_Color = "#2d89e5";
        private string Text_Main_Color = "#ffffff";
        private string Fade_Back_Color = "#ffffff";
        private string Fade_Main_Color = "#ffffff";
        private int Current_Width = 0;
        private string Item_Margin = "10px";
        public Prompt()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Grey900, Primary.Grey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                xmlDoc.Load("C:\\Users\\beah\\source\\repos\\UPrompt\\DEMO.xml");
                ParseSettings();
                ParseView();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Fatal error on XML Parsing", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            GenerateView();
            string html_path = $@"file:///{app_path}View.html";
            htmlhandler.Navigate($"{(string)html_path}");
        }
        private void GetNewFadeColor()
        {
            Fade_Back_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Back_Color)));
            Fade_Main_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Main_Color)));
        }
        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().Contains("="))
            {
                if (e.Url.ToString().Contains("&"))
                {
                    string action_code = e.Url.ToString().Split('?')[1].Replace("%3F", "");
                    string[] actions = action_code.Split('&');
                    foreach(string action in actions)
                    {
                        string action_name = action.Split('=')[0];
                        string action_value = SpecialKeyParse(action.Split('=')[1]);
                        RunAction(action_name, action_value);
                    }
                }
                else
                {
                    string action_code = e.Url.ToString().Split('?')[1].Replace("%3F", "");
                    string action_name = action_code.Split('=')[0];
                    string action_value = SpecialKeyParse(action_code.Split('=')[1]);
                    RunAction(action_name, action_value);
                }
            }
        }
        public void RunAction(string ActionName, string ActionValue)
        {
            if (ActionName.Contains("INPUT_"))
            {
                MessageBox.Show(ActionValue,ActionName);
            }
            else
            {
                switch (ActionName)
                {
                    case "MSG":
                        MessageBox.Show(ActionValue);
                        break;
                    case "EXIT":
                        Environment.Exit(int.Parse(ActionValue));
                        break;
                }
            }
        }
        private string SpecialKeyParse(string Text)
        {
            string ParsedText = Text
                .Replace("[USER]", Environment.UserName)
                .Replace("[DEVICE]", Environment.MachineName)
                .Replace("#TEXT_COLOR#", Text_Color)
                .Replace("#MAIN_COLOR#", Main_Color)
                .Replace("#BACKGROUND_COLOR#", Back_Color)
                .Replace("#FADE_BACKGROUND_COLOR#", Fade_Back_Color)
                .Replace("#FADE_MAIN_COLOR#", Fade_Main_Color)
                .Replace("#ITEM_MARGIN#", Item_Margin)
                .Replace("#MAIN_TEXT_COLOR#", Text_Main_Color)
                ;
            return ParsedText;
        }
        public void GenerateView()
        {
            string Template = File.ReadAllText($"{app_path}Index.html");
            string html = Template.Replace("=== XML CODE WILL GENERATE THIS VIEW ===", HtmlFromXml);

            File.WriteAllText($"{app_path}View.html", SpecialKeyParse(html));
        }
        public void ParseView()
        {

            XmlNode viewNode = xmlDoc.SelectSingleNode("/Application/View");
            foreach (XmlNode childNode in viewNode.ChildNodes)
            {
                GenrateHtmlFromXML(childNode.OuterXml);
            }
            if (HtmlFromXml.Length < 5) { HtmlFromXml = "=== THE VIEW IS EMPTY PLEASE FILL IT IN XML ==="; }
        }
        public void GenrateHtmlFromXML(string XML)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(XML);

            XmlNode childNode = doc.DocumentElement;


                // Vérifier le type de l'élément enfant
                switch (childNode.Name)
                {
                    case "ViewItem":
                        // Récupérer les attributs Type et Action
                        string type = childNode.Attributes["Type"].Value;
                        string action = childNode.Attributes["Action"]?.Value;
                        string action_name = "";
                        string action_value = "";
                    if (action != null)
                    {
                        try
                        {
                            action_name = action.Split('=')[0];
                            action_value = action.Split('=')[1];
                        }
                        catch (Exception ex) { MessageBox.Show("An action must be include = so in this format ACTION=VALUE", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                    }
                        // Traiter l'élément en fonction de son type et de son action
                    switch (type)
                        {
                            case "Title":
                                    HtmlFromXml += $"<div class=\"Title\">{childNode.InnerText}</div>\n";
                                    break;
                            case "Label":
                                HtmlFromXml += $"<div class=\"Label\">{childNode.InnerText}</div>\n";
                                break;
                            case "LabelBox":
                                HtmlFromXml += $"<div class=\"Box\">{childNode.InnerText}</div>\n";
                                break;
                            case "Button":
   
                                HtmlFromXml += $"<button class=\"Button\" type=\"submit\" name=\"?{action_name}\" value=\"{action_value}\">{childNode.InnerText}</button>\n";
                                break;
                            case "Row":
                                HtmlFromXml += $"<div class=\"Row\">\n";
                                
                                foreach (XmlNode rowChildNode in childNode.ChildNodes)
                                {
                                    GenrateHtmlFromXML(rowChildNode.OuterXml);
                                }

                                HtmlFromXml += "</div>\n";

                            break;
                        }
                        break;
                        default:
                        HtmlFromXml += $"{childNode.OuterXml}\n";
                        break;
                }
            

        }
        public void ParseSettings()
        {
            XmlNodeList settingsList = xmlDoc.SelectNodes("//Application/Settings");

            foreach (XmlNode settingNode in settingsList)
            {
                string name = settingNode.Attributes["Name"].Value;
                string value = settingNode.Attributes["Value"].Value;

                switch (name)
                {
                    case "Item-Margin":
                        try
                        {
                            string margin = value;
                            if (margin.Contains("px") || margin.Contains("%"))
                            {
                                Item_Margin = margin;
                            }
                            else
                            {
                                MessageBox.Show($"This setting {name} as invalid value {value} the value must be an CSS margin size (px or %)", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be an CSS margin size (px or %)", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case "Text-Color":
                        try
                        {
                            string color = value;
                            if (color.Contains("#") && color.Length > 3 && color.Length < 8)
                            {
                                Text_Color = color;
                            }
                            else
                            {
                                MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case "Application-Color":
                        try
                        {
                            string color = value;
                            if (color.Contains("#") && color.Length > 3 && color.Length < 8)
                            {
                                Back_Color = color;
                            }
                            else
                            {
                                MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case "Accent-Color":
                        try
                        {
                            string color = value;
                            if (color.Contains("#") && color.Length > 3 && color.Length < 8)
                            {
                                Main_Color = color;
                            }
                            else
                            {
                                MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case "Accent-Text-Color":
                        try
                        {
                            string color = value;
                            if (color.Contains("#") && color.Length > 3 && color.Length < 8)
                            {
                                Text_Main_Color = color;
                            }
                            else
                            {
                                MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case "Width":
                        try
                        {
                            int width = int.Parse(value);
                            this.Width = width;
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case "Height":
                        try
                        {
                            int height = int.Parse(value);
                            this.Height = height;
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be an Hexadecimal", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case "WindowsOpenMode":
                        string WindowsMode = value;
                        switch (value)
                        {
                            case "Normal":
                                this.WindowState = FormWindowState.Normal;
                                break;
                            case "Minimized":
                                this.WindowState = FormWindowState.Minimized;
                                break;
                            case "Maximized":
                                this.WindowState = FormWindowState.Maximized;
                                break;
                            default:
                                MessageBox.Show($"This setting {name} as invalid value {value} the value must be Normal, Minimized or Maximized", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                        }



                        break;

                    case "AllowMinimize":
                        try
                        {
                            bool AllowMinimize = bool.Parse(value);
                            this.MinimizeBox = AllowMinimize;
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be a boolean", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case "AllowMaximize":
                        try
                        {
                            bool AllowMaximize = bool.Parse(value);
                            this.MaximizeBox = AllowMaximize;
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be a boolean", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;
                    case "Frame":
                        bool Frame = bool.Parse(value);
                        this.ControlBox = Frame;
                        break;
                    default:
                        MessageBox.Show($"This setting name is not reconize as a valid one: {name});", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            GetNewFadeColor();
        }

    }
}
