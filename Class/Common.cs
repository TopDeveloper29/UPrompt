using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPrompt.Class
{
    public static class Common
    {
        public static Dictionary<string, string> Variable = new Dictionary<string, string>();
        public static Dictionary<string, string> PreviousVariable = new Dictionary<string, string>();
        public static string LastWarning { get; internal set; } = "";
        public static List<string> WarningHistory { get; internal set; } = new List<string>();
        public static string LastError { get; internal set; } = "";
        public static List<string> ErrorHistory { get; internal set; } = new List<string>();
        public static string LastOutput { get; internal set; }  = "";
        public static List<string> OutputHistory { get; internal set; } = new List<string>();
        public static Prompt Windows { get; set; } = new Prompt();
        public static string Application_Path { get; set; } = (AppDomain.CurrentDomain.BaseDirectory).Replace(@"\", "/");
        public static string Xml_Path { get; set; } = $@"{Application_Path}\MainPage.xml";
        internal static int DebugXmlLineNumber { get; set; } = 0;


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
            if (Settings.Production == false)
            {
                return MessageBox.Show(Text, Title, buttons, MessageBoxIcon.Warning);
            }
            return DialogResult.Ignore;
        }
        public static DialogResult Error(string Text, string Title = "Error", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            LastError = Text;
            ErrorHistory.Add(Text);
            if (Settings.Production == false)
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
                    Clipboard.SetText(PlainVariable);
                }
            }
            return variables;
        }
        public static void SetVariable(string Id, string Value)
        {
            if (!Variable.ContainsKey(Id))
            {
                Variable.Add(Id, Value);
                PreviousVariable.Add(Id, "|VOID|");
            }
            else
            {
                if (PreviousVariable.ContainsKey(Id))
                {
                    PreviousVariable[Id] = Variable[Id];
                }
                else
                {
                    PreviousVariable.Add(Id, Variable[Id]);
                }
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
            Variable.TryGetValue(Id, out var Value);
            return Value;
        }

    }
}
