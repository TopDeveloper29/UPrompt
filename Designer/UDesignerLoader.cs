using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Class;

namespace UPrompt.UDesigner
{
    public static class UDesignerLoader
    {
        public static void OpenNewUprompt()
        {
            string path = UCommon.GetVariable("SavedPath");
            if (File.Exists(path))
            {
                Process upromt = new Process();
                upromt.StartInfo.FileName = UCommon.UPrompt_exe;
                upromt.StartInfo.WorkingDirectory = UCommon.Application_Path_Windows;
                upromt.StartInfo.Arguments = $"/Path \"{path}\"";
                upromt.Start();
            }
        }
        public static void PreviewXML()
        {
            try
            {
                string path = UCommon.GetVariable("SavedPath");
                UCommon.SetVariable("PreviewHtml", "");
                UParser.ClearHtml();
                if (File.Exists(path))
                {
                    
                    string rawxml = File.ReadAllText(path);
                    if (rawxml.ToLower().Contains("<Application>".ToLower()) && rawxml.ToLower().Contains("<View>".ToLower()) && rawxml.ToLower().Contains("</Application>".ToLower()) && rawxml.ToLower().Contains("</View>".ToLower()))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(path);
                        string html = "";
                        foreach (XmlNode childNode in xmlDoc.SelectSingleNode("/Application/View").ChildNodes)
                        {
                            html = UParser.GenerateHtmlFromXML(childNode.OuterXml);
                        }

                        UCommon.SetVariable("PreviewHtml", html);
                    }
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
                        if (File.Exists(@"C:\Program Files\Notepad++\notepad++.exe"))
                        {
                            Process.Start(@"C:\Program Files\Notepad++\notepad++.exe", $"\"{path}\"");
                        }
                        else
                        {
                            Process.Start(path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UCommon.Error(ex.Message, "Save Xml Error");
            }
        }
    }
}

