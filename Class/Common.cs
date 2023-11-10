using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPrompt.Class
{
    internal class Common
    {
        public static Dictionary<string, string> InternalVariable = new Dictionary<string, string>();
        public static Dictionary<string, string> InternalLastVariable = new Dictionary<string, string>();
        internal static Prompt Windows { get; set; }
        internal static int DebugXmlLineNumber = 0;
        public static DialogResult Warning(string Text, string Title = "Warning", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            if (Settings.Release == false)
            {
                return MessageBox.Show(Text, Title, buttons, MessageBoxIcon.Warning);
            }
            return DialogResult.Ignore;
        }
        public static DialogResult Error(string Text, string Title = "Error", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            if (Settings.Release == false)
            {
                return MessageBox.Show(Text, Title, buttons, MessageBoxIcon.Error);
            }
            return DialogResult.Ignore;
        }
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

    }
}
