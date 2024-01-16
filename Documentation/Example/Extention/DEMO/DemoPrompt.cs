using System.Windows.Forms;
using UPrompt.Core;

namespace DEMO
{
    public static class DemoPrompt
    {
        public static void ShowDemo(string value)
        {
            MessageBox.Show($"Extension method called with this value: {value} here some data that came directly form UPrompt: {USettings.Application_Name}");
        }
    }
}
