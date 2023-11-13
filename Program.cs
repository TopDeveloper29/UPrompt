using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPrompt.Class;

namespace UPrompt
{
    internal static class Program
    {

        [STAThread]
        static void Main(string[]Args)
        {
            //SET GOOD MODE FOR BROWSER (ALLOW CSS)
            int browserVer = 11000;
            int regVal = browserVer << 0x10 | 0xFFFF;

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            int currentValue = (int)key.GetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", -1);
            if (currentValue != regVal)
            {
                key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", regVal, RegistryValueKind.DWord);
                key.Close();
                string applicationPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                System.Diagnostics.Process.Start(applicationPath);
                System.Windows.Forms.Application.Exit();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Args.Length > 0)
            {
                bool Path = false;
                if (Args.Length == 1) { Path = true; }
                foreach (string arg in Args)
                {
                    switch (arg)
                    {
                        case "/Path":
                        case "/path":
                        case "/p":
                        case "/P":
                            Path = true;
                            break;
                        default:
                            if (arg.ToLower().Contains(".xml") && Path)
                            {
                                if (File.Exists(arg))
                                {
                                    Common.Application_Path = arg;
                                }
                            }
                            Path = false;
                            break;
                    }
                }
            }
            Application.Run(new Prompt());
        }
    }
}
