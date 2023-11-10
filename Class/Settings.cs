using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Class;

namespace UPrompt.Class
{
    internal class Settings
    {
        internal static bool Release = false;
        internal static string RawSettings { get; set; } = string.Empty;
        internal static int Count { get; set; } = 0;
        public static void Load(XmlNodeList settingsList)
        {
            RawSettings = settingsList.ToString();
            string WarningTitle = "Wrong settings format";
            int PowershellId = 0;
            foreach (XmlNode settingNode in settingsList)
            {
                Count++;
                string name = settingNode.Attributes["Name"].Value;
                string value = settingNode.Attributes["Value"].Value;
                string id = settingNode.Attributes["Id"]?.Value;

                switch (name)
                {
                    case "Powershell":
                    case "PS":
                        if (File.Exists(value))
                        {
                            if (id == null)
                            {
                                Handler.PowershellHandlers.Add(new PowershellHandler(PowershellId, value));
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
                        PowershellId++;
                        break;

                    case "Font-Name":
                        HtmlXml.Font_Name = value;
                        break;
                    case "Item-Margin":
                        try
                        {
                            string margin = value;
                            if (margin.Contains("px") || margin.Contains("%"))
                            {
                                HtmlXml.Item_Margin = margin;
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
                                HtmlXml.Text_Color = color;
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
                                HtmlXml.Back_Color = color;
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
                                HtmlXml.Main_Color = color;
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
                                HtmlXml.Text_Main_Color = color;
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
                            int width = int.Parse(value);
                            Common.Windows.Width = width;
                        }
                        catch
                        {
                            Common.Warning($"This setting {name} as invalid value \"{value}\" the value must be an integer", WarningTitle);
                        }
                        break;
                    case "Height":
                        try
                        {
                            int height = int.Parse(value);
                            Common.Windows.Height = height;
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
                                Common.Windows.WindowState = FormWindowState.Normal;
                                break;
                            case "Minimized":
                                Common.Windows.WindowState = FormWindowState.Minimized;
                                break;
                            case "Maximized":
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
                            bool AllowMinimize = bool.Parse(value);
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
                            bool AllowMaximize = bool.Parse(value);
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
                            Release = bool.Parse(value);
                        }
                        catch { Common.Warning($"The setting {name} as invalid value \"{value}\" it must be a boolean", WarningTitle); }
                        break;

                    default:
                        Common.Warning($"This setting \"{name}\" is unknow please provide valid settings", WarningTitle);
                        break;
                }
            }
            HtmlXml.GetNewFadeColor();
        }

    }
}
