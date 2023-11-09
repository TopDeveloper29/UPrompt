using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace UPrompt
{
    internal class PowershellRunner
    {
        internal string Id { get; set; }
        internal string Ps1Path { get; set; }
        internal string LastOutput { get; private set; }
        internal string LastInput { get; private set; }
        private Process Powershell = new Process();
        private void StartPowershell(string path)
        {
        }
        internal void StopPowershell()
        {
            Powershell.Dispose();
        }
        internal string ReadOutput()
        {
            string output = null;
            LastOutput = output;
            return output;
        }
        internal void SendInput(string command)
        {
            LastInput = command;
        }

        internal PowershellRunner(string id, string ps1)
        {
            Id = id;
            Ps1Path = ps1;
            this.StartPowershell(Ps1Path);
        }

    }
    internal static class Handler
    {
        public static Dictionary<string, string> InternalVariable = new Dictionary<string, string>();
        public static Dictionary<string, string> InternalLastVariable = new Dictionary<string, string>();
        internal static Prompt Windows { get; set; }
        public static Dictionary<string, string> ShowInternalVariable(string Id = "Show::All::Id", bool ShowDialog = true)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            string PlainVariable = "";
            if (Id == "Show::All::Id")
            {
                foreach (string key in InternalVariable.Keys)
                {
                    variables.Add(key, GetInternalVariable(key));
                    PlainVariable += $"{key} | {GetInternalVariable(key)}\n";
                }
            }
            else
            {
                if (Id.Contains(","))
                {
                    string[] Ids = Id.Split(',');
                    foreach (string IdIn in Ids)
                    {
                        variables.Add(IdIn, GetInternalVariable(IdIn));
                        PlainVariable += $"{IdIn} | {GetInternalVariable(IdIn)}\n";
                    }
                }
                else
                {
                    variables.Add(Id, GetInternalVariable(Id));
                    PlainVariable = $"{Id} | {GetInternalVariable(Id)}";
                }
            }

            if (ShowDialog == true)
            {
                if (MessageBox.Show($"Key Id | Key Value\n-----------------------\n{PlainVariable}", "Here the Variable do you want to copy this to your cliboard?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Clipboard.SetText(PlainVariable);
                }
            }
            return variables;
        }
        public static void SetInternalVariable(string Id, string Value)
        {
            if (!InternalVariable.ContainsKey(Id))
            {
                InternalVariable.Add(Id, Value);
                InternalLastVariable.Add(Id, "|VOID|");
            }
            else
            {
                if (InternalLastVariable.ContainsKey(Id))
                {
                    InternalLastVariable[Id] = InternalVariable[Id];
                }
                else
                {
                    InternalLastVariable.Add(Id, InternalVariable[Id]);
                }
                InternalVariable[Id] = Value;
            }
        }
        public static string GetInternalVariable(string Id)
        {
            InternalVariable.TryGetValue(Id, out var Value);
            return Value;
        }
        public static void RunAction(string ActionName, string ActionValue)
        {
            if (ActionName.Contains("InputHandler_") || ActionName.Contains("INPUT_") || ActionName.Contains("Linker_"))
            {
                if (ActionName.Contains("InputHandler_"))
                {
                    string Runner_Input_Id = ActionName.Split(new string[] { "InputHandler_" }, StringSplitOptions.None)[1].Split(new string[] { "::Action::" }, StringSplitOptions.None)[0];
                    string Runner_Action = ActionName.Split(new string[] { "::Action::" }, StringSplitOptions.None)[1];
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
                    SetInternalVariable(ActionInputId, ActionValue);
                }
                if (ActionName.Contains("Linker_"))
                {
                    string LinkerSource = ActionName.Split(new string[] { "Linker_" }, StringSplitOptions.None)[1];
                    string LinkerDestination = ActionValue;
                    if(GetInternalVariable(LinkerSource) != null) { SetInternalVariable(LinkerDestination, GetInternalVariable(LinkerSource)); }
                }
            }
            else
            {
                string ActionId = ActionName.Split(new string[] { "::ID::" }, StringSplitOptions.None)[0];
                if (ActionId.Contains("::Action::"))
                {
                    ActionId = ActionId.Split(new string[] { "::Action::" }, StringSplitOptions.None)[1];
                }
                else
                {
                    ActionName = ActionName.Replace($"{ActionId}::ID::", null);
                }

                string ActionOutput = "";
                try
                {
                    ActionValue = HtmlXml.SpecialTextParse(ActionValue);
                    switch (ActionName)
                    {
                        case "GetVariable":
                            ShowInternalVariable(ActionValue);
                            break;
                        case "SetVariable":
                            try
                            {
                                SetInternalVariable(ActionValue.Split(':')[0], ActionValue.Split(':')[1]);
                            }catch (Exception ex) { MessageBox.Show("SetInternalVariable must include : after the = like SetInternalVariable=NewVar=Hello {User}!!!", "Set internal variable format error",MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                            break;
                        case "YesNoDialog":
                            if (MessageBox.Show(ActionValue, Windows.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                ActionOutput = "true";
                            }
                            else
                            {
                                ActionOutput = "false";
                            }

                            break;
                        case "OkDialog":

                            if (MessageBox.Show(ActionValue, Windows.Text, MessageBoxButtons.OK, MessageBoxIcon.None) == DialogResult.OK)
                            {
                                ActionOutput = "true";
                            }
                            else
                            {
                                ActionOutput = "false";
                            }
                            break;
                        case "EXIT":
                            ActionOutput = ActionValue;
                            Environment.Exit(int.Parse(ActionValue));
                            break;
                    }
                }
                catch (Exception ex) { ActionOutput = $"UPrompt Internal error:\n {ex.Message}"; }

                if (ActionOutput.Length > 0)
                {
                    SetInternalVariable(ActionId, ActionOutput);
                }
            }
            Windows.GenerateView();
        }
        public static void LoadSettings(XmlNodeList settingsList)
        {
            foreach (XmlNode settingNode in settingsList)
            {
                string name = settingNode.Attributes["Name"].Value;
                string value = settingNode.Attributes["Value"].Value;

                switch (name)
                {
                    case "Powershell-Runner":

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
                                Color RealColor = ColorTranslator.FromHtml(color);
                                Windows.Left.BackColor = RealColor;
                                Windows.Right.BackColor = RealColor;
                                Windows.Bottom.BackColor = RealColor;
                                Windows.TitleBar.BackColor = RealColor;
                                Windows.UpdateTitleBarColor();
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

                    case "AllowClose":
                        bool Frame = bool.Parse(value);
                        Windows.ControlBox = Frame;
                        Windows.UpdateTitleBarIconAndFunction();
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
