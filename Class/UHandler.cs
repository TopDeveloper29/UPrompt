﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UPrompt.Class
{
    public static class UHandler
    {
        public class ExtentionHandler
        {
            internal int Id;
            private Assembly extensionAssembly;
            private Type extensionType;

            internal ExtentionHandler(int _id, string _path, string _namespace)
            {
                this.Id = _id;
                this.LoadExtension(_path, _namespace);
            }
            public void LoadExtension(string pathToDll, string extensionNamespace)
            {
                try
                {
                    extensionAssembly = Assembly.LoadFrom(pathToDll);
                    extensionType = extensionAssembly.GetType(extensionNamespace);
                }
                catch (Exception ex)
                {
                    UCommon.Error($"Could not load extension: {ex.Message}");
                }
            }
            public object CallExtensionMethod(string methodName, params object[] arguments)
            {
                try
                {
                    MethodInfo methodInfo = extensionType.GetMethod(methodName);
                    return methodInfo.Invoke(methodInfo, arguments);
                }
                catch (Exception ex)
                {
                    UCommon.Error($"Could not run extension method: {ex.Message}");
                    return "false";
                }
            }

        }
        public class PowershellHandler
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

                    Powershell.StartInfo.Arguments = $"-File \"{UCommon.Application_Path}\\Resources\\Code\\PsHandler.ps1\" -Path \"{Path}\"";
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
                catch (Exception ex) { UCommon.Error(ex.Message); }
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
                catch (Exception ex) { UCommon.Error(ex.Message); }
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
                catch { }
            }
            internal void SendInput(string Command)
            {
                try
                {
                    LastInput = Command;
                    InputHistory.Append(LastInput);
                    Powershell.StandardInput.WriteLine(Command);
                }
                catch (Exception ex) { UCommon.Error(ex.Message); }
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

        public static string LastActionOutput { get; private set; } = "";
        public static Collection<PowershellHandler> PowershellHandlers { get; private set; } = new Collection<PowershellHandler>();
        public static Collection<ExtentionHandler> ExtentionHandlers { get; private set; } = new Collection<ExtentionHandler>();
        private static string Decrypt(string encryptedText, string encryptionKey)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

            byte[] keyBytes = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 }, 1000).GetBytes(32);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                int ivLength = BitConverter.ToInt32(cipherTextBytes, 0);
                byte[] iv = new byte[ivLength];
                Array.Copy(cipherTextBytes, sizeof(int), iv, 0, ivLength);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(aes.Key, iv), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cipherTextBytes, sizeof(int) + ivLength, cipherTextBytes.Length - sizeof(int) - ivLength);
                    }

                    byte[] decryptedBytes = memoryStream.ToArray();

                    // Remove padding from decrypted data
                    int paddingSize = decryptedBytes[decryptedBytes.Length - 1];
                    byte[] unpaddedBytes = new byte[decryptedBytes.Length - paddingSize];
                    Array.Copy(decryptedBytes, unpaddedBytes, unpaddedBytes.Length);

                    return Encoding.UTF8.GetString(unpaddedBytes);
                }
            }
        }

        public static void HandlePost(string json)
        {
            json = json.Substring(0, json.Length - 1).Substring(1).Replace(@"\", null);
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);

            // Accessing properties
            foreach (var property in jsonObject.Properties())
            {
                string Name = property.Name;
                JToken Value = property.Value;
                try
                {
                    string Itemjson = Decrypt(Value.ToString(), "UPromptKey2023");
                    if (Itemjson != null && Itemjson.Length > 5 && Itemjson.Contains("{") && Itemjson.Contains("}") && Itemjson.Contains(",") && Itemjson.Contains(":"))
                    {
                        int lastIndex = Itemjson.LastIndexOf('}');
                        string removestring = Itemjson.Substring(lastIndex + 1);
                        string parsedjson = Itemjson.Replace(removestring, null);
                        JObject EndItemJson = JObject.Parse(parsedjson);
                        string type = (string)EndItemJson["type"];
                        string name = (string)EndItemJson["name"];
                        string id = (string)EndItemJson["id"];
                        string value = (string)EndItemJson["value"];
                        switch (type)
                        {
                            default:
                            case "ui":
                                RunAction(name, value, id);
                                break;
                            case "Linker":
                                string LinkerSource = value.Split(new string[] { "," }, StringSplitOptions.None)[0];
                                string LinkerDestination = value.Split(new string[] { "," }, StringSplitOptions.None)[1];
                                if (UCommon.GetVariable(LinkerSource) != null)
                                { UCommon.SetVariable(LinkerDestination, UCommon.GetVariable(LinkerSource)); }
                                break;
                            case "InputHandler":
                                if (UCommon.PreviousVariable.ContainsKey(id))
                                {
                                    if (UCommon.Variable[id] != UCommon.PreviousVariable[id])
                                    {
                                        RunAction(name, value, id);
                                    }
                                }
                                break;
                            case "ViewLoad":
                                RunAction(name, value, id);
                                break;

                        }
                    }
                }
                catch { }
            }
        }
  
        public static void RunAction(string ActionName, string ActionValue, string ActionId)
        {
                SwitchAction(ActionName, ActionValue);

                if (LastActionOutput.Length > 0)
                {
                    UCommon.SetVariable(ActionId, LastActionOutput);
                }
                UParser.GenerateView(UCommon.XmlDocument.SelectSingleNode("/Application/View"));
        }
        internal static void SwitchAction(string ActionName, string ActionValue)
        {
            string LineTitle = $"Xml line: {UCommon.DebugXmlLineNumber}";
            LastActionOutput = "";
            try
            {
                ActionValue = UParser.ParseSystemText(ActionValue);
                switch (ActionName)
                {
                    case "RunMethod":
                        try
                        {
                            foreach (ExtentionHandler EH in UHandler.ExtentionHandlers)
                            {
                                if (EH.Id == int.Parse(ActionValue.Split(',')[0]))
                                {
                                    if (ActionValue.Contains("(") && ActionValue.Contains(")"))
                                    {
                                        string[] splitString = ActionValue.Split(',');
                                        string methodtext = string.Join(",", splitString.Skip(1).ToArray());
                                        string pattern = @"\((.*?)\)";
                                        MatchCollection matches = Regex.Matches(methodtext, pattern);

                                        if (matches.Count > 0)
                                        {
                                            string _value = "";
                                            foreach (Match match in matches) { _value = match.Groups[1].Value; }
                                            object Result = EH.CallExtensionMethod(methodtext.Split('(')[0], _value.Split(','));
                                            if (Result != null) { LastActionOutput = Result.ToString(); }

                                            UCommon.SetVariable($"CS_{EH.Id}", LastActionOutput);
                                        }
                                        else
                                        { EH.CallExtensionMethod(ActionValue.Split(',')[1]); }
                                    }
                                    else
                                    { EH.CallExtensionMethod(ActionValue.Split(',')[1]); }
                                    return;
                                }
                            }

                        }
                        catch { UCommon.Warning($"The argument of {ActionName} must be in format \"Id,MethodName(argument)\"", LineTitle); }
                        break;
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
                                        LastActionOutput = PSH.ReadOutput();
                                        if (LastActionOutput != null && !LastActionOutput.Contains("ERROR:"))
                                        {
                                            UCommon.SetVariable($"PS_{psid}", LastActionOutput);
                                        }
                                        else
                                        {
                                            if (LastActionOutput.Contains("ERROR:"))
                                            {
                                                UCommon.Error($"Powershell instance #{psid} throw an error:\n {LastActionOutput}");
                                            }
                                            else
                                            {
                                                UCommon.Warning($"Powershell instance #{psid} run without any output");
                                            }
                                        }
                                    }
                                    PS_FOUND = true;
                                    if (PS_FOUND == false) { UCommon.Error($"The id \"{psid}\" was not found!!!\n If you don't know id, please note it will be auto filled starting from 0 and increment by one each time you create an new powershell instance you can also set the id by specifing in xml configuration", LineTitle); }
                                }).Start();
                            }

                        }
                        catch { UCommon.Warning($"RunPowershell action value must be formated as this way PowershellInstamceId,FunctionToRunOrCode\n If you don't know id, please note it will be auto filled starting from 0 and increment by one each time you create an new powershell instance you can also set the id by specifing in xml configuration", LineTitle); }
                        break;
                    case "RunExe":
                        try
                        {
                            if (ActionValue.Split(',').Length > 1)
                            { Process.Start(ActionValue.Split(',')[0], ActionValue.Split(',')[1]); }
                            else
                            { Process.Start(ActionValue.Split(',')[0]); }
                            
                        }
                        catch { UCommon.Warning($"RunExe action argument must specify exe path and exe argument like: Argument=\"cmd.exe,/C echo Helllo User\"", LineTitle); }
                        break;
                    case "RunPs1":
                        try
                        {
                            if (File.Exists(ActionValue) && ActionValue.ToLower().Contains(".ps1"))
                            {
                                Process powershell = new Process();
                                if (File.Exists(@"C:\Program Files\PowerShell\7\pwsh.exe"))
                                { powershell.StartInfo.FileName = @"C:\Program Files\PowerShell\7\pwsh.exe"; }
                                else
                                { powershell.StartInfo.FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe"; }
                                powershell.StartInfo.Arguments = $"-ExecutionPolicy Bypass -File {ActionValue}";
                                powershell.StartInfo.UseShellExecute = false;
                                powershell.StartInfo.RedirectStandardOutput = false;
                                powershell.StartInfo.CreateNoWindow = true;
                                powershell.BeginOutputReadLine();
                                powershell.Start();
                                powershell.WaitForExit();
                                LastActionOutput = powershell.StandardOutput.ReadToEnd();
                            }
                            else
                            {
                                UCommon.Warning($"The ps1 file do not exist: {ActionValue}", LineTitle);
                                LastActionOutput = "false";
                            }
                        }
                        catch { UCommon.Warning($"RunPs1 action argument must specify exe path that is a ps1 file (xml line: {UCommon.DebugXmlLineNumber})", LineTitle); LastActionOutput = "false"; }
                        break;
                    case "GetVariable":
                        try
                        {
                            LastActionOutput = UCommon.GetVariable(ActionValue);
                        }
                        catch { UCommon.Warning($"Could not found {ActionValue} variable.", LineTitle); LastActionOutput = "false"; }
                        break;
                    case "SetVariable":
                        try
                        {
                            UCommon.SetVariable(ActionValue.Split(',')[0], ActionValue.Split(',')[1]);
                        }
                        catch { UCommon.Warning($"SetInternalVariable arguments must specify name and value as argument like: Argument=\"NewVar,Hello User\"", LineTitle); }
                        break;
                    case "ShowVariable":
                        try
                        {
                            if (ActionValue.Length < 1)
                            {
                                UCommon.ShowVariable();
                            }
                            else
                            {
                                if (ActionValue.Contains(","))
                                {
                                    foreach (string item in UCommon.ShowVariable(ActionValue.Split(',')[0], bool.Parse(ActionValue.Split(',')[1])).Keys)
                                    {
                                        LastActionOutput += $"Name: {item} Value: {UCommon.ShowVariable(ActionValue, false)[item]}";
                                    }
                                }
                                else
                                {
                                    foreach (string item in UCommon.ShowVariable(ActionValue, false).Keys)
                                    {
                                        LastActionOutput += $"Name: {item} Value: {UCommon.ShowVariable(ActionValue, false)[item]}";
                                    }
                                }
                            }
                        }
                        catch (Exception ex) { UCommon.Error(ex.Message, LineTitle); LastActionOutput = "false"; }
                        break;
                    case "YesNoDialog":
                        if (UCommon.Output(ActionValue, UCommon.Windows.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            LastActionOutput = "true";
                        }
                        else
                        {
                            LastActionOutput = "false";
                        }

                        break;
                    case "OkDialog":
                        if (UCommon.Output(ActionValue, UCommon.Windows.Text) == DialogResult.OK)
                        {
                            LastActionOutput = "true";
                        }
                        else
                        {
                            LastActionOutput = "false";
                        }
                        break;
                    case "Browse":
                        try
                        {
                            string Type = ActionValue.Split(',')[0];
                            string Mode = ActionValue.Split(',')[1];
                            string Filter = " ";
                            if (ActionValue.Split(',').Length > 2) { Filter = ActionValue.Split(',')[2]; }
                            if (Type.Contains("File"))
                            {
                                if (Mode.Contains("Save"))
                                {
                                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                                    saveFileDialog.Filter = Filter;
                                    saveFileDialog.Title = "Save a File";

                                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        LastActionOutput = saveFileDialog.FileName;
                                    }
                                    else { LastActionOutput = "false"; }
                                }
                                else
                                {
                                    OpenFileDialog openFileDialog = new OpenFileDialog();
                                    openFileDialog.Filter = Filter;
                                    openFileDialog.Title = "Select a File";

                                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        LastActionOutput = openFileDialog.FileName;
                                    }
                                    else { LastActionOutput = "false"; }
                                }
                            }
                            else
                            {
                                FolderBrowserDialog Folderdialog = new FolderBrowserDialog();
                                Folderdialog.Description = "Select a Folder";

                                if (Folderdialog.ShowDialog() == DialogResult.OK)
                                {
                                    LastActionOutput = Folderdialog.SelectedPath;
                                }
                                else { LastActionOutput = "false"; }
                            }
                        }
                        catch { UCommon.Warning($"Browse action argument must be like: Argument=\"Type(File/Folder),Mode(Save/Load), Filter|*.*\" the filter is optional", LineTitle); }

                        break;
                    case "LoadPage":
                        if (File.Exists(ActionValue) && ActionValue.ToLower().Contains(".xml"))
                        {
                            try
                            {
                                USettings.LoadXml(ActionValue, false);
                                LastActionOutput = "true";
                            }
                            catch (Exception ex) { UCommon.Error(ex.Message, LineTitle); }
                        }
                        else
                        {
                            UCommon.Warning($"The xml file do not exist: {ActionValue}", LineTitle);
                            LastActionOutput = "false";
                        }
                        break;
                    case "ReloadView":
                        try
                        {
                            UParser.ReloadView();
                            LastActionOutput = "true";
                        }
                        catch(Exception ex) { UCommon.Warning(ex.Message,LineTitle); LastActionOutput = "false"; }
                        break;
                    case "ReloadSettings":
                        try
                        {
                            USettings.ReLoad();
                            LastActionOutput = "true";
                        }
                        catch (Exception ex) { UCommon.Warning(ex.Message, LineTitle); LastActionOutput = "false"; }
                        break;
                    case "LoadSetting":
                        try
                        {
                            USettings.Load(ActionValue.Split(',')[0], ActionValue.Split(',')[1], ActionValue.Split(',')[2]);
                            UParser.ReloadView();
                        }
                        catch { UCommon.Warning($"The argument of {ActionName} should be formated as:\n Argument=\"Name,Value,Id\" The Id is optional", LineTitle); }
                        break;
                    case "GetClipboard":
                        try
                        {
                            LastActionOutput = Clipboard.GetText();
                        }
                        catch (Exception ex) { UCommon.Error(ex.Message, LineTitle); LastActionOutput = "false"; }
                        break;
                    case "SetClipboard":
                        try
                        {
                            Clipboard.SetText(ActionValue);
                            LastActionOutput = "true";
                        }
                        catch (Exception ex) { UCommon.Error(ex.Message, LineTitle); LastActionOutput = "false"; }
                        break;
                    case "Exit":
                        int value = 0;
                        int.TryParse(ActionValue, out value);
                        Environment.Exit(value);
                        break;
                    default: UCommon.Warning($"{ActionName} do not exist or have ivalid uper and lower case", LineTitle); break;
                }
            }
            catch (Exception ex) { UCommon.Error(ex.Message, "Unknow Error"); }
        }
    }
}