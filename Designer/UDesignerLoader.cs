using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Class;

namespace UPrompt.UDesigner
{
    public static class UDesignerLoader
    {
        public static string PreferedEditorPath = @"C:\Program Files\Notepad++\notepad++.exe";
        public static void SetPreferedEditor(string path)
        {
            PreferedEditorPath = path;
        }
        public static void OpenNewUprompt()
        {
            string path = UCommon.GetVariable("SavedPath");
            if (path.Length > 2 && path.ToLower().Contains(":\\"))
            {
                if (File.Exists(path))
                {
                    Process upromt = new Process();
                    upromt.StartInfo.FileName = UCommon.UPrompt_exe;
                    //upromt.StartInfo.WorkingDirectory = UCommon.Application_Path_Windows;
                    upromt.StartInfo.Arguments = $"/Path \"{path}\"";
                    upromt.Start();
                    upromt.WaitForExit();
                    UParser.ReloadView();
                }
            }
        }
        public static void PreviewXML()
        {
            try
            {
                string path = UCommon.GetVariable("SavedPath");
                if (path.Length > 2 && path.ToLower().Contains(":\\"))
                {
                    UCommon.SetVariable("PreviewHtml", "");
                    UParser.ClearHtml();

                    if (File.Exists(path))
                    {
                        string rawxml = File.ReadAllText(path);
                        if (rawxml.ToLower().Contains("<Application>".ToLower()) && rawxml.ToLower().Contains("<View>".ToLower()) && rawxml.ToLower().Contains("</Application>".ToLower()) && rawxml.ToLower().Contains("</View>".ToLower()))
                        {
                            XmlDocument xmlDocument = new XmlDocument();
                            xmlDocument.Load(path);
                            string html = "";
                            foreach (XmlNode childNode in xmlDocument.SelectSingleNode("/Application/View").ChildNodes)
                            {
                                html = UParser.GenerateHtmlFromXML(childNode.OuterXml);
                            }
                            UCommon.SetVariable("PreviewHtml", html);
                        }
                    }
                    UParser.ReloadView();
                }
            } catch(Exception ex) { UCommon.Error(ex.Message); }
        }
        public static void CallEditor(string @new = "true")
        {
            try
            {
                string path = "";
                if (bool.Parse(@new)){ path = UCommon.GetVariable("NewPath"); File.WriteAllText(path, ""); }
                else { path = UCommon.GetVariable("LoadPath"); }

                if (Directory.Exists(Path.GetDirectoryName(path)))
                {
                    if (File.Exists(path))
                    {
                        UCommon.SetVariable("SavedPath", path);
                        if (File.Exists(PreferedEditorPath))
                        {
                            Process.Start(PreferedEditorPath, $"\"{path}\"");
                        }
                        else
                        {
                            Process.Start(path);
                        }
                        PreviewXML();
                    }
                }
            }
            catch (Exception ex)
            {
                UCommon.Error(ex.Message);
            }
        }
    }
}

