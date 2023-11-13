using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Linq;
using UPrompt.Class;

namespace UPrompt.Class
{
    public class SettingList : List<Setting>
    {
        public SettingList() { }
        public SettingList(IEnumerable<Setting> settings) { this.AddRange(settings); }
        public new void Add(Setting setting)
        {
            base.Add(setting);
        }
        public void Add(string Name, string Value, string Id)
        {
            this.Add(new Setting(Name, Value, Id));
        }
    }
    public class Setting
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Id { get; set; }
        public Setting(string Name,string Value,string Id)
        {
            this.Name = Name;
            this.Value = Value;
            this.Id = Id;
        }
    }
    public class Settings
    {
        public static string Text_Color { get; private set; } = "#fff";
        public static string Back_Color { get; private set; } = "#22252e";
        public static string Main_Color { get; private set; } = "#272c33";
        public static string Text_Main_Color { get; private set; } = "#000000";
        public static string Fade_Back_Color { get; private set; } = "#000";
        public static string Fade_Main_Color { get; private set; } = "#fff";
        public static string WindowsOpenMode { get; private set; } = "Normal";
        public static bool AllowMinimize { get; private set; } = true;
        public static bool AllowMaximize { get; private set; } = true;
        public static bool AllowClose { get; private set; } = true;
        public static int Width { get; private set; } = 600;
        public static int Height { get; private set; } = 600;
        public static string Item_Margin { get; private set; } = "3%";
        public static string Font_Name { get; private set; } = "Arial";
        public static bool Production { get; private set; } = false;
        public static string Raw { get; private set; } = string.Empty;
        public static int Count { get; private set; } = 0;
        internal static int PowershellFallbackId { get; private set; } = 0 ;
        internal static int ExtentionFallbackId { get; private set; } = 0;
        internal static void Load(XmlNodeList settingsList)
        {
            Raw = settingsList.ToString();

            foreach (XmlNode settingNode in settingsList)
            {
                Count++;
                string name = settingNode.Attributes["Name"]?.Value; if (name == null) { name = settingNode.Attributes["name"]?.Value; }
                string value = settingNode.Attributes["Value"]?.Value; if (value == null) { value = settingNode.Attributes["value"]?.Value; }
                string id = settingNode.Attributes["Id"]?.Value;
                ParseSettingsFromXml(name, value, id);
            }
            NewFadeColor();
        }
        public static void Load(string Name, string Value, string Id = null)
        {
            ParseSettingsFromXml(Name, Value, Id);
            NewFadeColor();
        }
        public static void Load(SettingList settings)
        {
            foreach (Setting setting in settings)
            {
                ParseSettingsFromXml(setting.Name, setting.Value, setting.Id);
            }
            NewFadeColor();
        }
        public static void LoadXml(string path,bool loadsettings = true)
        {
            Common.Xml_Path = path;
            try{ Common.Windows.xmlDoc.Load(path);}
            catch (Exception ex) { Common.Error($"{ex.Message}\n\n{Common.Windows.xmlDoc.InnerXml}", "Fatal error on XML Parsing"); }
            if (loadsettings)
            {
                Load(Common.Windows.xmlDoc.SelectNodes("//Application/Setting"));
            }
            ViewParser.GenerateView(Common.Windows.xmlDoc.SelectSingleNode("/Application/View"));
            Common.Windows.htmlhandler.Navigate($"file:///{Common.Application_Path}Resources/Code/View.html");
        }
        private static void NewFadeColor()
        {
            Fade_Back_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Back_Color), 0.1f));
            Fade_Main_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Main_Color), 0.2f));
        }
        private static void ParseSettingsFromXml(string name, string value, string id = null)
        {
            string WarningTitle = "Wrong settings format";
            
            value = ViewParser.ParseSystemText(value);
            switch (name)
            {
                case "Variable":
                case "Var":
                case "var":
                    if (value.Contains(","))
                    {
                        Common.SetVariable(value.Split(',')[0], value.Split(',')[1]);
                    }
                    else
                    {
                        Common.SetVariable(value, "");
                    }
                    break;
               
                case "Extention":
                case "C#":
                case "CSharp":
                case "DLL":
                    string path = "";
                    string namespc = "";
                    try
                    {
                        path = value.Split(',')[0];
                        namespc = value.Split(',')[1];
                    }
                    catch { Common.Warning($"The settings {name} as invalid argument ({value}) or id ({id})\n The value must be Argument=\"PATH,NAMESPACE.CLASS\"\nThe id must be an integer", WarningTitle); }

                    if (File.Exists(path))
                    {
                        if (id == null)
                        {
                            Handler.ExtentionHandlers.Add(new ExtentionHandler(ExtentionFallbackId, path, namespc));
                        }
                        else
                        {
                            Handler.ExtentionHandlers.Add(new ExtentionHandler(int.Parse(id), path, namespc));
                        }
                    }
                    else { Common.Warning($"The file could not be found {path}"); }
                    break;
                case "Powershell":
                case "PS":
                    if (File.Exists(value))
                    {
                        if (id == null)
                        {
                            Handler.PowershellHandlers.Add(new PowershellHandler(PowershellFallbackId, value));
                        }
                        else
                        {
                            try
                            {
                                Handler.PowershellHandlers.Add(new PowershellHandler(int.Parse(id), value));
                            }
                            catch
                            {
                                Common.Warning($"This setting {name} as invalid id ({value}) it must be an integer", WarningTitle);
                            }
                        }
                    }
                    else
                    {
                        Common.Warning($"This setting {name} as invalid path or file do not exist: \"{value}\"", WarningTitle);
                    }
                    PowershellFallbackId++;
                    break;

                case "Font-Name":
                case "Font":
                    Font_Name = value;
                    break;
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
                            Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be a size like ({value}px or {value}%)", WarningTitle);
                        }
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be a size like ({value}px or {value}%)", WarningTitle);
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
                            Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                    }
                    break;
                case "Application-Color":
                    try
                    {
                        string color = value;
                        if (color.Contains("#") && color.Length > 3 && color.Length < 8)
                        {
                            Back_Color = color;
                            Color RealColor = ColorTranslator.FromHtml(color);
                            Common.Windows.Left.BackColor = RealColor;
                            Common.Windows.Right.BackColor = RealColor;
                            Common.Windows.Bottom.BackColor = RealColor;
                            Common.Windows.TitleBar.BackColor = RealColor;
                            Common.Windows.UpdateTitleBarColor();
                        }
                        else
                        {
                            Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
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
                            Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
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
                            Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                    }
                    break;

                case "Width":
                    try
                    {
                        Width = int.Parse(value);
                        Common.Windows.Width = Width;
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an integer", WarningTitle);
                    }
                    break;
                case "Height":
                    try
                    {
                        Height = int.Parse(value);
                        Common.Windows.Height = Height;
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an integer", WarningTitle);
                    }
                    break;

                case "WindowsOpenMode":
                case "OpenMode":
                    string WindowsMode = value;
                    switch (value)
                    {
                        case "Normal":
                            WindowsOpenMode = "Normal";
                            Common.Windows.WindowState = FormWindowState.Normal;
                            break;
                        case "Minimized":
                            WindowsOpenMode = "Minimized";
                            Common.Windows.WindowState = FormWindowState.Minimized;
                            break;
                        case "Maximized":
                            WindowsOpenMode = "Maximized";
                            Common.Windows.WindowState = FormWindowState.Maximized;
                            break;
                        default:
                            Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be Normal, Minimized or Maximized", WarningTitle);
                            break;
                    }
                    break;

                case "AllowMinimize":
                case "Minimize":
                case "Min":
                    try
                    {
                        AllowMinimize = bool.Parse(value);
                        Common.Windows.MinimizeBox = AllowMinimize;
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be a boolean", WarningTitle);
                    }
                    break;

                case "AllowMaximize":
                case "Maximize":
                case "Max":
                    try
                    {
                        AllowMaximize = bool.Parse(value);
                        Common.Windows.MaximizeBox = AllowMaximize;
                    }
                    catch
                    {
                        Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be a boolean", WarningTitle);
                    }
                    break;

                case "AllowClose":
                case "Close":
                    bool Frame = bool.Parse(value);
                    Common.Windows.ControlBox = Frame;

                    Common.Windows.UpdateTitleBarIconAndFunction();
                    break;

                case "Production":
                case "Release":
                case "Prod":
                    try
                    {
                        Production = bool.Parse(value);
                    }
                    catch { Common.Warning($"The setting {name} as invalid value \"{value}\" it must be a boolean", WarningTitle); }
                    break;
                case "Application-Icon":
                case "Icon":
                    if (File.Exists(value))
                    {
                        try
                        {
                            Icon icon = new Icon(value);
                            Common.Windows.Icon = icon;
                        }
                        catch { Common.Error("Could not load this file as image"); }
                    }
                    else
                    {
                        Common.Warning($"Could not find this file: \"{value}\"!!!");
                    }
                    break;
                default:
                    Common.Warning($"This setting \"{name}\" is unknow please provide valid settings", WarningTitle);
                    break;
            }

        }
    }
}
