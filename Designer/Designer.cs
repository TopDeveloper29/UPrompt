using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Core;


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
        public Dictionary<string, Argument> Arguments { get; set; }
    }
    internal class View
    {
        public string Description { get; set; }
        public string InnerTextPurpose { get; set; }
        public string[] SupportedProperties { get; set; }
    }
    internal class Settings
    {
        public string Description { get; set; }
        public string[] Values { get; set; }
    }
    public static class Designer
    {
        private static string MainNodeName { get; set; } = "Xml";
        private static Dictionary<string, string> ListOfJson { get; set; } = new Dictionary<string, string>();
        private static string Xml { get; set; } = "";
        private static string ExtraXml { get; set; } = "";
        private static string PreferedEditorPath { get; set; } = @"C:\Program Files\Notepad++\notepad++.exe";
        private static string ErrorTitle = "UDesigner Internal Error";

        // Handle editor and file load
        public static void SetPreferedEditor(string path)
        {
            PreferedEditorPath = path;
        }
        public static void CallEditor(string @new = "true")
        {
            try
            {
                string path = "";
                if (bool.Parse(@new)) { path = UCommon.GetVariable("Result_NewPath"); File.WriteAllText(path, ""); }
                else { path = UCommon.GetVariable("Result_LoadPath"); }

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
                    }
                }
            }
            catch (Exception ex) { UCommon.Error(ex.Message, ErrorTitle); }
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
                    upromt.StartInfo.Arguments = $"/Path \"{path}\"";
                    upromt.Start();
                }
            }
        }

        // General node generation
        public static void SelectMainNode(string NodeName)
        {
            try
            {
                UCommon.DeleteVariable("ViewValue");
                UCommon.DeleteVariable("SettingValue");

                string json = File.ReadAllText($@"{UCommon.Application_Path_Windows}\Resources\Code\UDesigner.json");
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
                Xml = "";
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
                SelectSubNode();
            }
            catch (Exception ex) { UCommon.Error(ex.Message, ErrorTitle); }
        }
        public static void SelectSubNode()
        {
            try
            {
                UCommon.DeleteVariable("ViewValue");
                UCommon.DeleteVariable("SettingValue");

                Xml = ""; ExtraXml = "";
                string key = UCommon.GetVariable("SubType") ?? ListOfJson.Keys.First();
                if (!ListOfJson.Keys.Contains(key)) { key = ListOfJson.Keys.First(); UCommon.SetVariable("SubType", ListOfJson.Keys.First()); }

                switch (MainNodeName.ToLower() ?? MainNodeName)
                {
                    case "xml":
                        UCommon.SetVariable("XmlCode", ListOfJson[key].ToString());
                        Xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                        break;
                    case "settings":
                        Settings Settings = JsonConvert.DeserializeObject<Settings>(ListOfJson[key].ToString());
                        if (UCommon.GetVariable("SettingValue") == null) { UCommon.SetVariable("SettingValue", Settings.Values[0]); }
                        Xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                        Xml += $"<ViewItem Type=\"Label\">Description: {Settings.Description}</ViewItem>";
                        SelectSetting();
                        break;
                    case "view":
                        Dictionary<string, Dictionary<string, object>> View = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(ListOfJson[key].ToString());
                        if (UCommon.GetVariable("ViewValue") == null) { UCommon.SetVariable("ViewValue", View.Keys.First()); }
                        SelectView();

                        break;
                    case "action":
                        Action action = JsonConvert.DeserializeObject<Action>(ListOfJson[key]);
                        Xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                        Xml += $"\n<ViewItem Type=\"Label\">Description: {action.Description}</ViewItem>\n";
                        ExtraXml =
                            "<table>\r\n" +
                            "   <caption style=\"font-size: 1.5em; font-weight: bold; text-align: left;\">Arguments: </caption>\n" +
                            "   <tr>\n" +
                            "       <th>Name</th>\r\n" +
                            "       <th>Description</th>\r\n" +
                            "       <th>Mandatory</th>\r\n" +
                            "   </tr>\n";

                        foreach (string akey in action.Arguments.Keys)
                        {
                            ExtraXml += 
                            $"  <tr>\n" +
                            $"      <td>{akey}</td>\r\n" +
                            $"      <td>{action.Arguments[akey].Description2}</td>\r\n" +
                            $"      <td>{action.Arguments[akey].Mandatory}</td>\r\n" +
                            $"  </tr>\n";
                        }
                        ExtraXml += "</table>";
                        UCommon.SetVariable("XmlCode", $"<ViewAction Id='Test{key}' Type='Button' Action='{key}' Argument='Test of {key}'>Test of {key}</ViewAction>");

                        break;
                    case "variable":
                        
                        Xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                        Dictionary<string, string> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(ListOfJson[key].ToString());
                        
                        ExtraXml = 
                        "   <table>\r\n" +
                        "       <tr>\n" +
                        "           <th>Name</th>\r\n" +
                        "           <th>Description</th>\r\n" +
                        "       </tr>\n";

                        foreach (string vkey in jsonDict.Keys)
                        {
                            ExtraXml += 
                            "       <tr>\n" +
                            $"          <td>{vkey.Replace("{", "").Replace("}", "").Replace("#", "")}</td>\r\n" +
                            $"          <td>{jsonDict[vkey]}</td>\r\n" +
                            $"      </tr>\n";
                        }
                        ExtraXml += "   </table>";
                        
                        UCommon.SetVariable("XmlCode", "N/A");
                        break;
                    default:
                        Xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                        break;
                }
                RefreshDesignerCode();
            }
            catch (Exception ex) { UCommon.Error($"Select Node Error: {ex.Message}", ErrorTitle); }
        }

        // Special node generation
        public static void SelectSetting(string Refresh = "false")
        {
            try
            {
                string name = UCommon.GetVariable("SubType");
                Settings Settings = JsonConvert.DeserializeObject<Settings>(ListOfJson[name].ToString());
                string value = UCommon.GetVariable("SettingValue") ?? Settings.Values[0];
                if (!Settings.Values.Contains(value)) { value = Settings.Values[0]; UCommon.SetVariable("SettingValue", Settings.Values[0]); }

                Xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                ExtraXml = "<ViewItem Type=\"SubTitle\">Examples Value:</ViewItem>";
                ExtraXml += GenrerateDropBox(Settings.Values, "SettingValue", "210129,SelectSetting(true)");
                UCommon.SetVariable("XmlCode", "<Setting Name='" + name + "' Value='" + value + "'/>");
                if (Refresh.Contains("true")) { RefreshDesignerCode(); }
            }
            catch (Exception ex) { UCommon.Error(ex.Message, ErrorTitle); }
        }
        public static void SelectView(string Refresh = "false")
        {
            try
            {
                string name = UCommon.GetVariable("SubType");
                Dictionary<string, View> View = JsonConvert.DeserializeObject<Dictionary<string, View>>(ListOfJson[name]);
                string value = UCommon.GetVariable("ViewValue") ?? View.Keys.First();
                if (!View.Keys.Contains(value)) { value = View.Keys.First(); UCommon.SetVariable("SettingValue", View.Keys.First()); }

                Xml = GenrerateDropBox(ListOfJson.Keys.ToArray(), "SubType", "210129,SelectSubNode");
                Xml += "<ViewItem Type=\"Label\"> Type: </ViewItem>";
                Xml += GenrerateDropBox(View.Keys.ToArray(), "ViewValue", "210129,SelectView(true)");

                string description = View[value].Description;
                string InnerPurpose = View[value].InnerTextPurpose;

                ExtraXml = 
                    "<table>\r\n" +
                    "  <tr>\n" +
                    "      <td>Description</td>\n" +
                    $"      <td>{description}</td>\n" +
                    "  </tr>" +
                    "  <tr>" +
                    "       <td>Inner text behaviors</td>\n" +
                    $"      <td>{InnerPurpose}</td>\n" +
                    "   </tr>" +
                    "   <tr>" +
                    "       <td>Valid Property</td>" +
                    "       <td>";
                int index = 0;
                int end = View[value].SupportedProperties.Length - 1;
                foreach (string element in View[value].SupportedProperties)
                {
                    string Virgule = ",";if (index == end) { Virgule = ""; }
                    ExtraXml += $"{element} {Virgule}\n";
                    index++;
                }
                ExtraXml += "</td></tr>\n</table>\n";

                string innertext;
                
                switch(value.ToLower() ?? value)
                {
                    case "row":
                    case "box":
                    case "spacer":
                        innertext = string.Empty;
                        break;
                    default:
                        innertext = "Test";
                        break;
                }
                UCommon.SetVariable("XmlCode", $"<{name} Type='{value}'>{innertext}</{name}>");
                if (Refresh.Contains("true")) { RefreshDesignerCode(); }
            }
            catch (Exception ex) { UCommon.Error(ex.Message, ErrorTitle); }
        }

        // Generation of html
        public static void PreviewXML(string ReadBox = "false")
        {
            string htmlcode;
            if (ReadBox.Contains("false"))
            { htmlcode = UParser.GenerateHtmlFromXML(UCommon.GetVariable("XmlCode")); }
            else
            { htmlcode = UParser.GenerateHtmlFromXML(UCommon.GetVariable("TestIt")); }
            UCommon.SetVariable("PreviewHtml", htmlcode);
        }
        private static void RefreshDesignerCode()
        {
            // Add Row and close it
            Xml = $"<ViewItem Type=\"Row\">\n{Xml}\n</ViewItem>";
            ExtraXml = $"<ViewItem Type=\"Row\">\n{ExtraXml}\n</ViewItem>";
            try
            {
                UCommon.SetVariable("DesignerHtml", UParser.GenerateHtmlFromXML(Xml));
                UCommon.SetVariable("DesignerExtraHtml", UParser.GenerateHtmlFromXML(ExtraXml));
            }
            catch (Exception ex) { UCommon.Error("Refresh Error: " + ex.Message, ErrorTitle); }

            PreviewXML();
            UParser.ReloadView();
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

