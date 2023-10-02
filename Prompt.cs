using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("C:\\Users\\beah\\source\\repos\\UPrompt\\DEMO.xml");

            XmlNodeList settingsList = xmlDoc.SelectNodes("//Application/Settings");

            foreach (XmlNode settingNode in settingsList)
            {
                string name = settingNode.Attributes["Name"].Value;
                string value = settingNode.Attributes["Value"].Value;

                switch (name)
                {
                    case "Width":
                    int width = int.Parse(value);
                    this.Width = width;
                    break;
                
                    case "Height":
                        int height = int.Parse(value);
                        this.Height = height;
                    break;

                    default:
                        MessageBox.Show($"This property is not reconize as a valid one: {name});");
                        break;
                }
                
            }

        //SET GOOD MODE FOR BROWSER (ALLOW CSS)
        int browserVer, regVal;browserVer = 11000;regVal = browserVer << 0x10 | 0xFFFF;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", regVal, RegistryValueKind.DWord);key.Close();

            string html_path = $@"file:///{app_path}index.html";
            htmlhandler.Navigate($"{(string)html_path}");
        }
        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            
            if (e.Url.ToString().Contains("LOCAL:") && e.Url.ToString().Contains("="))
            {
                string action_code = e.Url.ToString().Split(new string[] { "LOCAL:" },StringSplitOptions.None)[1];
                string action_name = action_code.Split('=')[0];
                string action_value = action_code.Split('=')[1];
                action_value = action_value.Replace("[USER]",Environment.UserName).Replace("[DEVICE]",Environment.MachineName);

                switch (action_name)
                {
                    case "MSG":
                        MessageBox.Show(action_value);
                        break;
                    case "EXIT":
                        Environment.Exit(int.Parse(action_value));
                        break;
                }
            }
        }

    }
}
