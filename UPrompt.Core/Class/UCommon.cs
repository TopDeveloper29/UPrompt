using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace UPrompt.Core
{
    internal class ActionStorage
    {
        internal string Action {  get; set; }
        internal string Arguments { get; set; }
        internal string LastValue { get; set; }
        internal string Id { get; set; }
        internal ActionStorage(string _Action,string _Arguments,string _LastValue,string _Id)
        {
            Action = _Action;
            Id = _Id;
            Arguments = _Arguments;
            LastValue = _LastValue;
        }
    }
    public static class UCommon
    {
        public class VariableDictionary : Dictionary<string, string>
        {
            internal new string this[string key]
            {
                get { return base[key]; }
                set
                {
                    base[key] = value;
                    TrackChangeInVariable(key);
                }
            }
        }
        private static void TrackChangeInVariable(string Key)
        {
            try
            {
                if (GetVariable(Key) != null && TrackedVariable.TryGetValue(Key, out ActionStorage acs) == true)
                {
                    if (Variable[Key] != TrackedVariable[Key].LastValue)
                    {
                        TrackedVariable[Key].LastValue = Variable[Key];
                        UHandler.RunAction(TrackedVariable[Key].Action, TrackedVariable[Key].Arguments, TrackedVariable[Key].Id);
                    }
                }
            } catch { }
        }
        internal static VariableDictionary Variable { get; private set; } = new VariableDictionary();

        internal static Dictionary<string,ActionStorage> TrackedVariable = new Dictionary<string, ActionStorage>();
        internal static XmlDocument XmlDocument { get; set; } = new XmlDocument();
        public static bool ForceOpenDesinger { get; set; } = false;
       
        public static string LastWarning { get; internal set; } = "";
        public static List<string> WarningHistory { get; internal set; } = new List<string>();
        public static string LastError { get; internal set; } = "";
        public static List<string> ErrorHistory { get; internal set; } = new List<string>();
        public static string LastOutput { get; internal set; }  = "";
        public static List<string> OutputHistory { get; internal set; } = new List<string>();
        public static Prompt Windows { get; set; } = new Prompt();
        public static string Application_Path { get; set; } = (AppDomain.CurrentDomain.BaseDirectory).Replace(@"\", "/");
        public static string Application_Path_Windows { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public static string UPrompt_exe = Assembly.GetEntryAssembly().Location;
        public static string Xml_Path { get; set; } = $@"{Application_Path}\MainPage.xml";

        public static DialogResult Output(string Text, string Title = "Output", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            LastOutput = Text;
            OutputHistory.Add(Text);
            return MessageBox.Show(Text, Title, buttons, icon);

        }
        public static DialogResult Warning(string Text, string Title = "Warning", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            LastWarning = Text;
            WarningHistory.Add(Text);
            if (USettings.Production == false)
            {
                return MessageBox.Show(Text, Title, buttons, MessageBoxIcon.Warning);
            }
            return DialogResult.Ignore;
        }
        public static DialogResult Error(string Text, string Title = "Error", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            LastError = Text;
            ErrorHistory.Add(Text);
            if (USettings.Production == false)
            {
                return MessageBox.Show(Text, Title, buttons, MessageBoxIcon.Error);
            }
            return DialogResult.Ignore;
        }
        public static Dictionary<string, string> ShowVariable(string Id = "Show::All::Id", bool ShowDialog = true)
        {
            Dictionary<string, string> variables = new Dictionary<string, string>();
            string PlainVariable = "";
            if (Id == "Show::All::Id")
            {
                foreach (string key in Variable.Keys)
                {
                    variables.Add(key, GetVariable(key));
                    PlainVariable += $"{key} | {GetVariable(key)}\n";
                }
            }
            else
            {
                if (Id.Contains(","))
                {
                    string[] Ids = Id.Split(',');
                    foreach (string IdIn in Ids)
                    {
                        variables.Add(IdIn, GetVariable(IdIn));
                        PlainVariable += $"{IdIn} | {GetVariable(IdIn)}\n";
                    }
                }
                else
                {
                    variables.Add(Id, GetVariable(Id));
                    PlainVariable = $"{Id} | {GetVariable(Id)}";
                }
            }

            if (ShowDialog == true)
            {
                if (MessageBox.Show($"Key Id | Key Value\n-----------------------\n{PlainVariable}", "Here the Variable do you want to copy this to your cliboard?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Windows.Invoke((Action)(() =>
                    {
                        Clipboard.SetText(PlainVariable);
                    }));
                }
            }
            return variables;
        }
        public static void SetVariable(string Id, string Value)
        {
            if (!Variable.ContainsKey(Id))
            {
                Variable.Add(Id, Value);
            }
            else
            {
                Variable[Id] = Value;
            }
            File.WriteAllText($@"{Application_Path}\Resources\Code\Variables.ps1","");
            foreach (string Key in Variable.Keys)
            {
                File.AppendAllText($@"{Application_Path}\Resources\Code\Variables.ps1",$"[string]$Global:{Key} = \"{Variable[Key]}\";\n");
            }
        }
        public static string GetVariable(string Id)
        {
            if (Variable.TryGetValue(Id, out var Value) == false)
            {
                return null;
            }
            else
            { return Value; }
        }
        public static void DeleteVariable(string Id)
        {
            if (Variable.ContainsKey(Id))
            {
                Variable.Remove(Id);
            }
        }
    }
}
