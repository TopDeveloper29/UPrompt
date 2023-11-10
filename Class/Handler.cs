using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;


namespace UPrompt.Class
{
    internal class PowershellHandler
    {
        internal int Id { get; private set; }
        internal bool NewOutput { get; private set; }
        internal bool NewError { get; private set; }
        internal string LastOutput { get; private set; }
        internal string[] OutputHistory { get; private set; }
        internal string LastInput { get; private set; }
        internal string[] InputHistory { get; private set; }
        internal string LastError { get; private set; }
        internal string[] ErrorHistory { get; private set; }
        internal Process Powershell { get; private set; }
        public PowershellHandler(int Id, string Path)
        {
            this.Id = Id;
            LastInput = string.Empty;
            LastOutput = string.Empty;
            Powershell = new Process();
            OutputHistory = new string[] { };
            InputHistory = new string[] { };
            NewOutput = false;
            StartPowershell(Path);
        }
        private void StartPowershell(string Path)
        {
            try
            {
                if (File.Exists(@"C:\Program Files\PowerShell\7\pwsh.exe"))
                {
                    Powershell.StartInfo.FileName = @"C:\Program Files\PowerShell\7\pwsh.exe";
                }
                else
                {
                    Powershell.StartInfo.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";
                }

                Powershell.StartInfo.Arguments = $"-File \"{Prompt.Application_Path}\\Resources\\Code\\PsHandler.ps1\" -Path \"{Path}\"";
                Powershell.StartInfo.CreateNoWindow = true;
                Powershell.StartInfo.RedirectStandardOutput = true;
                Powershell.StartInfo.RedirectStandardError = true;
                Powershell.StartInfo.RedirectStandardInput = true;
                Powershell.StartInfo.UseShellExecute = false;
                Powershell.OutputDataReceived += Powershell_OutputDataReceived;
                Powershell.ErrorDataReceived += Powershell_ErrorDataReceived;
                
                Powershell.Start();
                Powershell.BeginOutputReadLine();
                Powershell.BeginErrorReadLine();
            }
            catch(Exception ex) { Common.Error(ex.Message); }
        }
        internal void StopPowershell() { Powershell.Dispose(); }
        private void Powershell_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    NewOutput = true;
                    LastOutput = e.Data;
                    OutputHistory.Append(LastOutput);
                }
            }
            catch (Exception ex) { Common.Error(ex.Message); }
        }
        private void Powershell_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    NewError = true;
                    LastError = e.Data;
                    ErrorHistory.Append(LastError);
                }
            }
            catch (Exception ex) { /*Handler.Error(ex.Message,"Read Error Fail");*/ }
        }
        internal void SendInput(string Command)
        {
            try
            {
                LastInput = Command;
                InputHistory.Append(LastInput);
                Powershell.StandardInput.WriteLine(Command);
            }
            catch (Exception ex) { Common.Error(ex.Message); }
        }
        internal string ReadOutput()
        {
            if (NewError == true)
            {
                NewError = false;
                NewOutput = false;
                return "ERROR:" + LastError;
            }
            NewOutput = false;
            return LastOutput;
        }
    }
    internal static class Handler
    {
        internal static string ActionOutput { get; private set; } = "";
        public static Collection<PowershellHandler> PowershellHandlers { get; private set; } = new Collection<PowershellHandler>();
        
        public static void RunAction(string ActionName, string ActionValue)
        {
            if (ActionName.Contains("InputHandler_") || ActionName.Contains("INPUT_") || ActionName.Contains("Linker_"))
            {
                if (ActionName.Contains("InputHandler_"))
                {
                    string Runner_Input_Id = ActionName.Split(new string[] { "InputHandler_" }, StringSplitOptions.None)[1].Split(new string[] { "::Action::" }, StringSplitOptions.None)[0];
                    string Runner_Action = ActionName.Split(new string[] { "::Action::" }, StringSplitOptions.None)[1];
                    if (Common.InternalLastVariable.ContainsKey(Runner_Input_Id))
                    {
                        if (Common.InternalVariable[Runner_Input_Id] != Common.InternalLastVariable[Runner_Input_Id])
                        {
                            RunAction(Runner_Action, ActionValue);
                        }
                    }
                }
                if (ActionName.Contains("INPUT_"))
                {
                    string ActionInputId = ActionName.Split(new string[] { "INPUT_" }, StringSplitOptions.None)[1];
                    Common.SetInternalVariable(ActionInputId, ActionValue);
                }
                if (ActionName.Contains("Linker_"))
                {
                    string LinkerSource = ActionName.Split(new string[] { "Linker_" }, StringSplitOptions.None)[1];
                    string LinkerDestination = ActionValue;
                    if(Common.GetInternalVariable(LinkerSource) != null) { Common.SetInternalVariable(LinkerDestination, Common.GetInternalVariable(LinkerSource)); }
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

                ActionOutput = "";
                try
                {
                    ActionValue = HtmlXml.SpecialTextParse(ActionValue);
                    switch (ActionName)
                    {
                        case "RunPowershell":
                            try
                            {
                                int psid = int.Parse(ActionValue.Split(',')[0]);
                                string command = ActionValue.Split(',')[1];
                                bool PS_FOUND = false;
                                foreach (PowershellHandler PSH in PowershellHandlers)
                                {
                                    new Thread(() =>
                                    {
                                        if (PSH.Id == psid)
                                        {
                                            PSH.SendInput(command);
                                            do { Thread.Sleep(1000); } while (PSH.NewOutput != true);
                                            string Output = PSH.ReadOutput();
                                            if (Output != null && !Output.Contains("ERROR:"))
                                            {
                                                Common.SetInternalVariable($"PS_{psid}", Output);
                                            }
                                            else
                                            {
                                                if (Output.Contains("ERROR:"))
                                                {
                                                    Regex regex = new Regex("[^a-zA-Z0-9\\s\\p{P}]");
                                                    string stderror = regex.Replace(Output.Split(new string[] { "ERROR:" }, StringSplitOptions.None)[1], "")
                                                    .Replace("[31;1m",null).Split(new string[] { "Write-Error:" },StringSplitOptions.None)[1].Replace("[0m",null);
                                                    Common.Error($"Powershell instance #{psid} throw an error:\n {stderror}");
                                                }
                                                else
                                                {
                                                    Common.Warning($"Powershell instance #{psid} run without any output");
                                                }
                                            }
                                        }
                                        PS_FOUND = true;
                                        if (PS_FOUND == false) { Common.Error($"The id \"{psid}\" was not found!!!\n If you don't know id, please note it will be auto filled starting from 0 and increment by one each time you create an new powershell instance you can also set the id by specifing in xml configuration"); }
                                    }).Start();
                                }
                            }
                            catch (Exception ex) { Common.Warning($"RunPowershell action value must be formated as this way PowershellInstamceId,FunctionToRunOrCode\n If you don't know id, please note it will be auto filled starting from 0 and increment by one each time you create an new powershell instance you can also set the id by specifing in xml configuration\n\nXml line: {Common.DebugXmlLineNumber}", "Wrong Format"); }
                            break;
                        case "GetVariable":
                            Common.ShowInternalVariable(ActionValue);
                            break;
                        case "SetVariable":
                            try
                            {
                                Common.SetInternalVariable(ActionValue.Split(',')[0], ActionValue.Split(',')[1]);
                            }catch { Common.Warning($"SetInternalVariable action value must specify name and value as argument like: Argument=\"NewVar,Hello User\" (xml line: {Common.DebugXmlLineNumber})", "Wrong Format"); }
                            break;
                        case "YesNoDialog":
                            if (MessageBox.Show(ActionValue, Common.Windows.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                ActionOutput = "true";
                            }
                            else
                            {
                                ActionOutput = "false";
                            }

                            break;
                        case "OkDialog":

                            if (MessageBox.Show(ActionValue, Common.Windows.Text, MessageBoxButtons.OK, MessageBoxIcon.None) == DialogResult.OK)
                            {
                                ActionOutput = "true";
                            }
                            else
                            {
                                ActionOutput = "false";
                            }
                            break;
                        case "EXIT":
                            int value = 0;
                            int.TryParse(ActionValue, out value);
                            Environment.Exit(value);
                            break;
                    }
                }
                catch (Exception ex) { ActionOutput = $"UPrompt Internal error:\n {ex.Message}"; }

                if (ActionOutput.Length > 0)
                {
                    Common.SetInternalVariable(ActionId, ActionOutput);
                }
            }
            Common.Windows.GenerateView();
        }
        }
}
