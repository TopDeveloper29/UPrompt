using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UPrompt.Class;

namespace UPrompt.UDesigner
{

    internal class Action
    {
        internal class Argument
        {
            public string Description2 { get; set; }
            public string Mandatory { get; set; }
        }
        public string Description { get; set; }
        public Dictionary<string,Argument> Arguments { get; set; }
    }
    internal class View
    {
        public string Description { get; set; }
        public string InnerTextPurpose { get; set; }
        public string[]  SupportedProperties { get; set; }
    }
    internal class Settings
    {
        public string Description { get; set; }
        public string[] Values { get; set; }
    }
    public static class Designer
    {
        private static string MainNodeName = "xml";
        private static Dictionary<string, string> ListOfJson = new Dictionary<string, string>();
        private static string xml = "";
        private static string ExtraXml = "";
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
            }
            catch (Exception ex) { UCommon.Error(ex.Message); }
        }
        public static void CallEditor(string @new = "true")
        {
            try
            {
                string path = "";
                if (bool.Parse(@new)) { path = UCommon.GetVariable("NewPath"); File.WriteAllText(path, ""); }
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
        public static void SelectSetting()
        {
            try
            {
                string name = UCommon.GetVariable("SubType");
                string value = UCommon.GetVariable("SettingValue");

                if (name != null && value != null)
                {
                    UCommon.SetVariable("GeneratedCode", "<Setting Name='" + name + "' Value='" + value + "'/>");
                }
                else
                {
                    Settings Settings = JsonConvert.DeserializeObject<Settings>(ListOfJson[ListOfJson.Keys.First()].ToString());
                    UCommon.SetVariable("SettingValue", Settings.Values[0]);
                    SelectSetting();
                }
            }
            catch (Exception ex) { UCommon.Error(ex.Message); }
        }
        public static void SelectView()
        {
            try
            {
                string name = UCommon.GetVariable("SubType");
                string value = UCommon.GetVariable("ViewValue");

                if (name != null && value != null)
                {
                    OpenXmlGroup();
                    Dictionary<string, View> data = JsonConvert.DeserializeObject<Dictionary<string, View>>(ListOfJson[name]);
                    UCommon.SetVariable("ViewValue", value);
                    xml += GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                    xml += GenrerateDropBox(data.Keys.ToArray(), "ViewValue", "210129,SelectView");

                    foreach (string key in data.Keys)
                    {
                        if (key.ToLower() == value.ToLower())
                        {
                            string description = data[key].Description;
                            string InnerPurpose = data[key].InnerTextPurpose;
      
                            ExtraXml += $"<ViewItem Type=\"Label\">Description: {description}</ViewItem>\n<ViewItem Type=\"Label\">Inner text behaviors:{InnerPurpose}</ViewItem>\n<ViewItem Type=\"Box\">\n";
                            ExtraXml += $"<ViewItem Type=\"Label\">Valid Property: </ViewItem>\n<ViewItem Type=\"Row\">";
                            foreach (string element in data[key].SupportedProperties)
                            {
                                ExtraXml += $"<ViewItem Type=\"Label\"> {element} </ViewItem>\n";
                            }
                            ExtraXml += "</ViewItem>\n</ViewItem>\n";
                            UCommon.SetVariable("ViewValue", value);
                            UCommon.SetVariable("GeneratedCode", $"<{name} Type='{value}'></{name}>");
                        }
                    }

                    CloseXmlGroup();
                    RefreshDesignerCode();
                }
                else
                {
                    Dictionary<string, Dictionary<string, object>> data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(ListOfJson.Keys.First().ToString());
                    UCommon.SetVariable("ViewValue", data.Keys.First());
                    SelectView();
                }
            }
            catch (Exception ex) { UCommon.Error(ex.Message); }
        }

        private static void RefreshDesignerCode()
        {
            try
            {
                if (xml.Length > 5)
                {
                    UParser.ClearHtml();
                    if (ExtraXml.Length > 10)
                    {
                        if (ExtraXml.Contains("</ViewItem>"))
                        {
                            UCommon.SetVariable("DesignerExtraHtml", UParser.GenerateHtmlFromXML(ExtraXml));
                            UParser.ClearHtml();
                        }
                    }
                    string html = UParser.GenerateHtmlFromXML(xml);
                    UCommon.SetVariable("DesignerHtml", html);
                    UParser.ReloadView();
                }
            }
            catch (Exception ex) { UCommon.Error(ex.Message); }
        }
        public static void SelectMainNode(string NodeName)
        {
            try
            {
                string json = File.ReadAllText($@"{UCommon.Application_Path_Windows}\Resources\Code\UDesigner.json");
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
                xml = "";
                ExtraXml = "";
                ListOfJson.Clear();
                MainNodeName = NodeName.ToLower();

                foreach (var property in jsonObject.Properties())
                {
                    string pname = property.Name.ToLower();
                    if (pname.Contains(MainNodeName))
                    {
                        var element = property.Value;
                        JObject jObject = JObject.Parse(element.ToString());
                        foreach (var pair in jObject)
                        {
                            string key = pair.Key;
                            string value = pair.Value.ToString();
                            if (key.Length > 1 && value.Length > 1)
                            {
                                ListOfJson.Add(key, value);
                            }
                        }
                    }
                }
                UCommon.SetVariable("SubType", ListOfJson.Keys.First());
                xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
   
                UParser.ClearHtml();
                UCommon.SetVariable("GeneratedCode", "");
                UCommon.SetVariable("DesignerHtml", "");
                if (xml.Length > 1)
                {
                    UCommon.SetVariable("DesignerHtml", UParser.GenerateHtmlFromXML(xml));
                }

                SelectSubNode();
            }
            catch (Exception ex) { UCommon.Error(ex.Message); }
        }
        public static void SelectSubNode()
        {
            xml = "";
            ExtraXml = "";
              
            try
            {
                string Node = UCommon.GetVariable("SubType");
                if (Node != null)
                {
                    try
                    {
                        if (ListOfJson.Keys.Contains(Node))
                        {
                            OpenXmlGroup();
                        }
                        foreach (string key in ListOfJson.Keys)
                        {
                            if (key.ToLower() == (Node.ToLower()))
                            {
                                UCommon.SetVariable("SubType", key);
                                xml += GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                                switch (MainNodeName)
                                {
                                    case "xml":
                                        UCommon.SetVariable("GeneratedCode", ListOfJson[key].ToString());
                                        CloseXmlGroup();
                                        break;
                                    case "settings":
                                        Settings Settings = JsonConvert.DeserializeObject<Settings>(ListOfJson[Node].ToString());
                                        if (UCommon.GetVariable("SettingValue") != null)
                                        {
                                            if (Settings.Values.Contains(UCommon.GetVariable("SettingValue")))
                                            {
                                                UCommon.SetVariable("SettingValue", UCommon.GetVariable("SettingValue"));
                                            }
                                            else
                                            {
                                                UCommon.SetVariable("SettingValue", Settings.Values[0]);
                                            }
                                        }
                                        else
                                        {
                                            UCommon.SetVariable("SettingValue", Settings.Values[0]);
                                        }
                                        xml += $"<ViewItem Type=\"Label\">Description: {Settings.Description}</ViewItem>";
                                        
                                        ExtraXml += "<ViewItem Type=\"Label\">Examples Value:</ViewItem>";
                                        ExtraXml += GenrerateDropBox(Settings.Values, "SettingValue", "210129,SelectSetting");
                                        
                                        CloseXmlGroup();
                                        SelectSetting();
                                        break;
                                    case "view":
                                        Dictionary<string, Dictionary<string, object>> data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(ListOfJson[Node].ToString());
                                        if (UCommon.GetVariable("ViewValue") != null)
                                        {
                                            if (data.Keys.Contains(UCommon.GetVariable("ViewValue") ?? ""))
                                            {
                                                UCommon.SetVariable("ViewValue", UCommon.GetVariable("ViewValue"));
                                            }
                                            else
                                            {
                                                UCommon.SetVariable("ViewValue", data.Keys.First());
                                            }
                                        }
                                        else
                                        {
                                            UCommon.SetVariable("ViewValue", data.Keys.First());
                                        }
                                        xml += GenrerateDropBox(data.Keys.ToArray(), "ViewValue", "210129,SelectView");
                                        SelectView();
                                        
                                        break;
                                    case "action":
                                        Console.WriteLine(ListOfJson.Values.First().ToString());
                                        //Action acdata = JsonConvert.DeserializeObject<Action>();
                                        CloseXmlGroup();
                                        break;

                                    default:
                                        CloseXmlGroup();
                                        break;
                                }
                            }
                        }
                    }
                    catch (Exception ex) { UCommon.Error(ex.Message); }
                }
                else
                {
                    xml = "";
                    ExtraXml = "";
                    OpenXmlGroup();
                    CloseXmlGroup();
                    UParser.ClearHtml();
                    UCommon.SetVariable("SubType", ListOfJson.Keys.First());
                    SelectSubNode();
                }
            }
            catch (Exception ex) { UCommon.Error(ex.Message); }
            RefreshDesignerCode();
        }
        private static void OpenXmlGroup()
        {
            xml = "<ViewItem Type=\"Row\">";
            ExtraXml = "<ViewItem Type=\"Row\">";
        }
        private static void CloseXmlGroup()
        {
            xml += "</ViewItem>";
            ExtraXml += "</ViewItem>";
        }
        private static string GenrerateDropBox(string[] items, string id, string method)
        {
            try
            {
                string xml = "";
                string list = "";
                int count = items.Length - 1;
                int index = 0;

                foreach (string item in items)
                {
                    if (item.Length > 0)
                    {
                        if (index == count)
                        {
                            list += item;
                        }
                        else
                        {
                            list += item + ",";

                        }
                    }
                    index++;
                }

                xml = $"<ViewInput Type=\"DropDown\" Style=\"Margin:5px;\" Id=\"{id}\" Action=\"RunMethod\" Argument=\"{method}\" DropList=\"{list}\">[{id}]</ViewInput>";
                return xml;
            }
            catch { return null; }
        }
    }
}

