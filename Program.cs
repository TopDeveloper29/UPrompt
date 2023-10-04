using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UPrompt
{
    internal static class Program
    {

        [STAThread]
        static void Main()
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
            Application.Run(new Prompt());
        }
    }
}
