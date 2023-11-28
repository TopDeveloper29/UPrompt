using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Internal;

namespace UPrompt.Class
{
    public class USettingList : List<USetting>
    {
        public USettingList() { }
        public USettingList(IEnumerable<USetting> settings) { this.AddRange(settings); }
        public new void Add(USetting setting)
        {
            base.Add(setting);
        }
        public void Add(string Name, string Value, string Id)
        {
            this.Add(new USetting(Name, Value, Id));
        }
    }
    public class USetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Id { get; set; }
        public USetting(string Name,string Value,string Id)
        {
            this.Name = Name;
            this.Value = UParser.ParseSystemText(Value);
            this.Id = Id;
        }
    }
    public class USettings
    {
        // Properties that do not reflect any system or view setting
        public static bool SkipSystemParsing {  get; set; } = false;
        public static int Count { get; private set; } = 0;
        public static string Raw { get; private set; } = string.Empty;
        public static List<string> ElementsParsingSkip { get; set; } = new List<string>();
        internal static int PowershellFallbackId { get; private set; } = 0;
        internal static int ExtentionFallbackId { get; private set; } = 0;
        internal static bool FirstLoadCompleted { get; private set; } = false;
        

        // All settings that reflect system and view setting (can be set using Load)
        public static string Text_Color { get; private set; } = "#fff";
        public static string Back_Color { get; private set; } = "#22252e";
        public static string Main_Color { get; private set; } = "#272c33";
        public static string Text_Main_Color { get; private set; } = "#000000";
        public static string Fade_Back_Color { get; private set; } = "#000";
        public static string Fade_Main_Color { get; private set; } = "#fff";
        public static string WindowsOpenMode { get; private set; } = "Normal";
        public static string WindowsResizeMode { get; private set; } = "All";
        public static bool ShowMinimize { get; private set; } = true;
        public static bool ShowMaximize { get; private set; } = true;
        public static bool ShowClose { get; private set; } = true;
        public static int Width { get; private set; } = 600;
        public static int Height { get; private set; } = 600;
        public static string Item_Margin { get; private set; } = "3%";
        public static string Font_Name { get; private set; } = "Arial";
        public static bool Production { get; private set; } = false;

        public static void ReLoad()
        {
            Load(UCommon.XmlDocument.SelectNodes("//Application/Setting"));
        }
        public static void Load(XmlNodeList settingsList)
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
        public static void Load(USettingList settings)
        {
            foreach (USetting setting in settings)
            {
                ParseSettingsFromXml(setting.Name, setting.Value, setting.Id);
            }
            NewFadeColor();
        }
        public static void LoadXml(string path,bool loadsettings = true)
        {
            UCommon.Xml_Path = path;
            try{ UCommon.XmlDocument.Load(path);}
            catch (Exception ex) { UCommon.Error($"{ex.Message}\n\n{UCommon.XmlDocument.InnerXml}", "Fatal error on XML Parsing"); }
            if (loadsettings)
            {
                Load(UCommon.XmlDocument.SelectNodes("//Application/Setting"));
                string css = UParser.ParseSettingsText(File.ReadAllText($@"{UCommon.Application_Path}\Resources\Code\UTemplate.css"));
                File.WriteAllText($@"{UCommon.Application_Path}\Resources\Code\UTemplate.css", css);
            }
            UParser.ReloadView();
            UCommon.Windows.htmlhandler.Navigate($"file:///{UCommon.Application_Path}Resources/Code/UView.html");
        }
        private static void NewFadeColor()
        {
            Fade_Back_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Back_Color), 0.1f));
            Fade_Main_Color = ColorTranslator.ToHtml(ControlPaint.Light(ColorTranslator.FromHtml(Main_Color), 0.2f));
        }
        private static void ParseSettingsFromXml(string name, string value, string id = null)
        {
            string WarningTitle = "Wrong settings format";
            
            value = UParser.ParseSystemText(value);
            switch (name)
            {
                case "Production":
                    try
                    {
                        Production = bool.Parse(value);
                    }
                    catch { UCommon.Warning($"The setting {name} as invalid value \"{value}\" it must be a boolean", WarningTitle); }
                    break;
                case "OnLoad":
                    try
                    {
                        UHandler.RunAction(value.Split(',')[0], string.Join(",",value.Split(',').Skip(1).ToArray()));
                    }
                    catch { UCommon.Warning($"{name} value must be like Value=\"ActionName,ActionArgument\"", WarningTitle); }
                    break;
                case "SkipElementParsing":
                    if (value != null)
                    {
                        ElementsParsingSkip.Add(value);
                    }
                    else
                    {
                        UCommon.Warning($"{name} provide an string that represent the id of element", WarningTitle);
                    }
                    break;
                case "CSS":
                    if (File.Exists(value) && value.ToLower().Contains(".css"))
                    {
                        NewFadeColor();
                        string css = UParser.ParseSettingsText(File.ReadAllText(value));
                        string copy = $@"{UCommon.Application_Path_Windows}Resources\Code\{UImage.GetImageNameFromLocalPath(value)}";
                        File.WriteAllText(copy,css);
                        UParser.CSSLink += $"<link rel=\"stylesheet\" href=\"file:///{copy}\">";
                    }
                    else
                    {
                        UCommon.Warning($"The setting for {name} file do not exist or is not a css file: \"{value}\"",WarningTitle);
                    }
                    break;
                case "Variable":

                    if (value.Contains(","))
                    {
                        if (value.Split(',')[1].Contains($"[{value.Split(',')[0]}]"))
                        {
                            if (UCommon.GetVariable(value.Split(',')[0]) == null) { UCommon.SetVariable(value.Split(',')[0], $""); break; }
                        }
                        UCommon.SetVariable(value.Split(',')[0], UParser.ParseSystemText(value.Split(',')[1]));
                    }
                    else
                    {
                        UCommon.SetVariable(value, "");
                    }
  
                    break;
 
                case "Extention":
                    string path = "";
                    string namespc = "";
                    try
                    {
                        path = value.Split(',')[0];
                        namespc = value.Split(',')[1];
                    }
                    catch { UCommon.Warning($"The settings {name} as invalid argument ({value}) or id ({id})\n The value must be Argument=\"PATH,NAMESPACE.CLASS\"\nThe id must be an integer", WarningTitle); }

                    if (File.Exists(path))
                    {
                        if (id == null)
                        {
                            UHandler.ExtentionHandlers.Add(new UHandler.ExtentionHandler(ExtentionFallbackId, path, namespc));
                        }
                        else
                        {
                            UHandler.ExtentionHandlers.Add(new UHandler.ExtentionHandler(int.Parse(id), path, namespc));
                        }
                    }
                    else { UCommon.Warning($"The file could not be found {path}"); }
                    break;
                case "Powershell":
                    if (File.Exists(value))
                    {
                        if (id == null)
                        {
                            UHandler.PowershellHandlers.Add(new UHandler.PowershellHandler(PowershellFallbackId, value));
                        }
                        else
                        {
                            try
                            {
                                UHandler.PowershellHandlers.Add(new UHandler.PowershellHandler(int.Parse(id), value));
                            }
                            catch
                            {
                                UCommon.Warning($"This setting {name} as invalid id ({value}) it must be an integer", WarningTitle);
                            }
                        }
                    }
                    else
                    {
                        UCommon.Warning($"This setting {name} as invalid path or file do not exist: \"{value}\"", WarningTitle);
                    }
                    PowershellFallbackId++;
                    break;
                
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
                            UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be a size like ({value}px or {value}%)", WarningTitle);
                        }
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be a size like ({value}px or {value}%)", WarningTitle);
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
                            UCommon.Windows.Left.BackColor = RealColor;
                            UCommon.Windows.Right.BackColor = RealColor;
                            UCommon.Windows.Bottom.BackColor = RealColor;
                            UCommon.Windows.TitleBar.BackColor = RealColor;
                            UCommon.Windows.UpdateTitleBarColor();
                            FirstLoadCompleted = true;
                        }
                        else
                        {
                            UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
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
                            UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
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
                            UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
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
                            UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                        }
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an hexadecimal color", WarningTitle);
                    }
                    break;
                
                case "Width":
                    try
                    {
                        Width = int.Parse(value);
                        UCommon.Windows.Width = Width;
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an integer", WarningTitle);
                    }
                    break;
                case "Height":
                    try
                    {
                        Height = int.Parse(value);
                        UCommon.Windows.Height = Height;
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be an integer", WarningTitle);
                    }
                    break;
                case "WindowsResizeMode":
                    switch (value)
                    {
                        case "All":
                            UCommon.Windows.Top.Enabled = true;
                            UCommon.Windows.Left.Enabled = true;
                            UCommon.Windows.Right.Enabled = true;
                            UCommon.Windows.Bottom.Enabled = true;
                            UCommon.Windows.cornerleft.Enabled = true;
                            UCommon.Windows.cornerright.Enabled = true;
                            WindowsResizeMode = value;
                            break;
                        case "Horizontal":
                            UCommon.Windows.Left.Enabled = true;
                            UCommon.Windows.Right.Enabled = true;
                            UCommon.Windows.Top.Enabled = false;
                            UCommon.Windows.Bottom.Enabled = false;
                            UCommon.Windows.cornerleft.Enabled = false;
                            UCommon.Windows.cornerright.Enabled = false;
                            WindowsResizeMode = value;
                            break;
                        case "Vertical":
                            UCommon.Windows.Left.Enabled = false;
                            UCommon.Windows.Right.Enabled = false;
                            UCommon.Windows.Bottom.Enabled = true;
                            UCommon.Windows.Top.Enabled = true;
                            UCommon.Windows.cornerleft.Enabled = false;
                            UCommon.Windows.cornerright.Enabled = false;
                            WindowsResizeMode = value;
                            break;
                        case "Diagonal":
                            UCommon.Windows.Left.Enabled = false;
                            UCommon.Windows.Right.Enabled = false;
                            UCommon.Windows.Bottom.Enabled = false;
                            UCommon.Windows.Top.Enabled = false;
                            UCommon.Windows.cornerleft.Enabled = true;
                            UCommon.Windows.cornerright.Enabled = true;
                            WindowsResizeMode = value;
                            break;
                        case "None":
                            UCommon.Windows.Top.Enabled = false;
                            UCommon.Windows.Left.Enabled = false;
                            UCommon.Windows.Right.Enabled = false;
                            UCommon.Windows.Bottom.Enabled = false;
                            UCommon.Windows.cornerleft.Enabled = false;
                            UCommon.Windows.cornerright.Enabled = false;
                            WindowsResizeMode = value;
                            break;
                        default:
                            UCommon.Warning($"The settings {name} must be one of this All,Horizontal,Vertical,Diagonal,None", WarningTitle);
                            break;
                    }
                    break;
                case "WindowsOpenMode":
                    string WindowsMode = value;
                    switch (value)
                    {
                        case "Normal":
                            WindowsOpenMode = "Normal";
                            UCommon.Windows.WindowState = FormWindowState.Normal;
                            break;
                        case "Minimized":
                            WindowsOpenMode = "Minimized";
                            UCommon.Windows.WindowState = FormWindowState.Minimized;
                            break;
                        case "Maximized":
                            WindowsOpenMode = "Maximized";
                            UCommon.Windows.WindowState = FormWindowState.Maximized;
                            break;
                        default:
                            UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be Normal, Minimized or Maximized", WarningTitle);
                            break;
                    }
                    break;
                case "Application-Icon":
                    if (File.Exists(value))
                    {
                        try
                        {
                            Icon icon = new Icon(value);
                            UCommon.Windows.Icon = icon;
                        }
                        catch { UCommon.Error("Could not load this file as image"); }
                    }
                    else
                    {
                        UCommon.Warning($"Could not find this file: \"{value}\"!!!");
                    }
                    break;
                case "Application-Title":
                    UCommon.Windows.Text = value;
                    break;

                case "ShowMinimize":
                    try
                    {
                        ShowMinimize = bool.Parse(value);
                        UCommon.Windows.MinimizeBox = ShowMinimize;
                        UCommon.Windows.UpdateTitleBarIconAndFunction();
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be a boolean", WarningTitle);
                    }
                    break;
                case "ShowMaximize":

                    try
                    {
                        ShowMaximize = bool.Parse(value);
                        UCommon.Windows.MaximizeBox = ShowMaximize;
                        UCommon.Windows.UpdateTitleBarIconAndFunction();
                    }
                    catch
                    {
                        UCommon.Warning($"This setting {name} as invalid value \"{value}\" the value must be a boolean", WarningTitle);
                    }
                    break;
                case "ShowClose":
                    bool Frame = bool.Parse(value);
                    UCommon.Windows.ControlBox = Frame;
                    UCommon.Windows.UpdateTitleBarIconAndFunction();
                    break;
                default:
                    UCommon.Warning($"This setting \"{name}\" is unknow please provide valid settings", WarningTitle);
                    break;
            }

        }
    }
}
