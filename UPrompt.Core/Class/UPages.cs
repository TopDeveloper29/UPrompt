using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace UPrompt.Core
{
    public static class UPages
    {
        public class UPage
        {
            public string Path { get; private set; } = null;
            public string Xml { get; private set; } = null;
            public XmlDocument XmlDocument { get; private set; }
            public string Html { get; private set; } = null;
            public UPage() { }
            public UPage(string _Path, bool LoadPage)
            {
                LoadFile(_Path, LoadPage);
            }
            public void LoadFile(string _Path, bool LoadPage)
            {
                Path = _Path;
                if (File.Exists(Path))
                {
                    Xml = File.ReadAllText(Path);
                    XmlDocument = new XmlDocument();
                    XmlDocument.Load(Path);
                    Load(true, LoadPage, LoadPage);
                }
            }

            public void Load(bool CreateHtml, bool LoadPage, bool LoadSeting)
            {
                if (XmlDocument != null && Xml != null && Path != null)
                {
                    if (LoadSeting)
                    { USettings.Load(XmlDocument.SelectNodes("//Application/Setting")); }

                    if (CreateHtml)
                    {
                        Html = "";

                        foreach (XmlNode ChildNode in XmlDocument.SelectSingleNode("//Application/View").ChildNodes)
                        {
                            // this generate html and include System parsing for inner value and other html element
                            if (ChildNode.OuterXml.Length > 4)
                            {
                                Html += UParser.GenerateHtmlFromXML(ChildNode.OuterXml) ?? "";
                            }
                        }

                        if (Html.Length < 5) { Html = "=== THE VIEW IS EMPTY PLEASE FILL IT IN XML ==="; }
                        Html = $"{UParser.CSSLink}\n{Html}";
                    }

                    if (LoadPage)
                    {
                        string TempHtml = HtmlTemplate.Replace("=== XML CODE WILL GENERATE THIS VIEW ===", Html).Replace("#UTemplateCSS#", UParser.ParseText(CssTemplate, UParser.ParseMode.Setting));
                        File.WriteAllText($@"{UCommon.Application_Path}\Resources\Code\UView.html", UParser.ParseText(TempHtml, UParser.ParseMode.Setting));

                        UCommon.Windows.Invoke((Action)(() =>
                        {
                            UCommon.Windows.htmlhandler.Source = new Uri($"file:///{UCommon.Application_Path}Resources/Code/UView.html");
                        }));
                    }
                }
                else
                {
                    if (Path != null && File.Exists(Path))
                    { LoadFile(Path, LoadPage); }
                }
            }
        }
        public static string HtmlTemplate { get; } = File.ReadAllText($@"{UCommon.Application_Path}\Resources\Code\UTemplate.html");
        public static string CssTemplate { get; } = File.ReadAllText($@"{UCommon.Application_Path_Windows}Resources\Code\UTemplate.css");
        public static UPage CurrentPage { get; private set; } = new UPage($@"{UCommon.Application_Path_Windows}MainPage.xml", true);
        public static List<UPage> Pages { get; } = new List<UPage>();
        public static UPage AddPage(string Path)
        {
            try
            {
                if (Pages.FirstOrDefault(obj => obj.Path == Path) == null)
                {
                    Pages.Add(new UPage(Path, false));
                }
                return Pages.FirstOrDefault(obj => obj.Path == Path);
            }
            catch { return null; }
        }
        public static bool LoadPage(string Path, bool ReloadHtml, bool LoadSettings)
        {
            try
            {
                UPage PageToLoad = Pages.FirstOrDefault(obj => obj.Path == Path);
                if (PageToLoad == null)
                { PageToLoad = new UPage(Path, true); Pages.Add(PageToLoad); }
                else
                { PageToLoad.Load(ReloadHtml, true, LoadSettings); }

                CurrentPage = PageToLoad;
                return true;
            }
            catch { return false; }
        }
        public static void RefreshPage(bool FullRefresh)
        {
            if (FullRefresh)
            {
                CurrentPage.Load(true, true, false);
            }
            UCommon.Windows.Invoke((Action)(() =>
            {
                if (UCommon.Windows.htmlhandler.CoreWebView2 != null && UCommon.Windows.htmlhandler.Source != null)
                {
                    UCommon.Windows.htmlhandler.Reload();
                }
            }));
        }
    }
}