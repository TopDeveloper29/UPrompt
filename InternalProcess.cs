using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UPrompt
{
    internal class InternalProcess
    {

        public static Dictionary<string, string> InternalVariable = new Dictionary<string, string>();
        public static Dictionary<string, string> InternalLastVariable = new Dictionary<string, string>();
        internal static Prompt Windows = new Prompt();
        public static void RunAction(string ActionName, string ActionValue)
        {
            if (ActionName.Contains("InputHandler_"))
            {

                string Runner_Input_Id = ActionName.Split(new string[] { "InputHandler_" }, StringSplitOptions.None)[1].Split(new string[] { ":Action:" }, StringSplitOptions.None)[0];
                string Runner_Action = ActionName.Split(new string[] { ":Action:" }, StringSplitOptions.None)[1];
                if (InternalLastVariable.ContainsKey(Runner_Input_Id))
                {
                    if (InternalVariable[Runner_Input_Id] != InternalLastVariable[Runner_Input_Id])
                    {
                        RunAction(Runner_Action, ActionValue);
                    }
                }
            }
            if (ActionName.Contains("INPUT_"))
            {
                string ActionInputId = ActionName.Split(new string[] { "INPUT_" }, StringSplitOptions.None)[1];
                if (!InternalVariable.ContainsKey(ActionInputId))
                {
                    InternalVariable.Add(ActionInputId, ActionValue);
                    InternalLastVariable.Add(ActionInputId, "|VOID|");
                }
                else
                {
                    if (InternalLastVariable.ContainsKey(ActionInputId))
                    {
                        InternalLastVariable[ActionInputId] = InternalVariable[ActionInputId];
                    }
                    else
                    {
                        InternalLastVariable.Add(ActionInputId, InternalVariable[ActionInputId]);
                    }
                    InternalVariable[ActionInputId] = ActionValue;
                }
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
        public static void LoadSettings(XmlNodeList settingsList)
        {
            foreach (XmlNode settingNode in settingsList)
            {
                string name = settingNode.Attributes["Name"].Value;
                string value = settingNode.Attributes["Value"].Value;

                switch (name)
                {
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
                                HtmlXml.Text_Color = color;
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
                                HtmlXml.Back_Color = color;
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
                                HtmlXml.Main_Color = color;
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
                                HtmlXml.Text_Main_Color = color;
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
                            Windows.Width = width;
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
                            Windows.Height = height;
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
                                Windows.WindowState = FormWindowState.Normal;
                                break;
                            case "Minimized":
                                Windows.WindowState = FormWindowState.Minimized;
                                break;
                            case "Maximized":
                                Windows.WindowState = FormWindowState.Maximized;
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
                            Windows.MinimizeBox = AllowMinimize;
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
                            Windows.MaximizeBox = AllowMaximize;
                        }
                        catch
                        {
                            MessageBox.Show($"This setting {name} as invalid value {value} the value must be a boolean", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case "Frame":
                        bool Frame = bool.Parse(value);
                        Windows.ControlBox = Frame;
                        break;

                    default:
                        MessageBox.Show($"This setting name is not reconize as a valid one: {name});", "Bad XML", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            HtmlXml.GetNewFadeColor();
        }
    }
}
