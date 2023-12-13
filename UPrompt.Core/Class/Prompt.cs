using Microsoft.Web.WebView2.Core;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using UPrompt.Core;


namespace UPrompt
{
    public partial class Prompt : Form
    {
        internal bool IconLigthMode = false;

        protected private bool isDragging = false;
        protected private Point dragOffset;
        protected private Point initialMousePos;
        protected private Size initialFormSize;

        protected private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                Point newLocation = this.Location;
                newLocation.X += e.X - dragOffset.X;
                newLocation.Y += e.Y - dragOffset.Y;
                this.Location = newLocation;
            }
        }
        protected private void TitleBar_MouseDown(object sender, MouseEventArgs e) { isDragging = true; dragOffset = e.Location; }
        protected private void TitleBar_MouseUp(object sender, MouseEventArgs e) { isDragging = false; }
        protected private void MinimizeButton_Click(object sender, EventArgs e) { this.WindowState = FormWindowState.Minimized; }
        protected private void MaximizeButton_Click(object sender, EventArgs e) { if (this.WindowState == FormWindowState.Maximized) { this.WindowState = FormWindowState.Normal; } else { this.WindowState = FormWindowState.Maximized; } }
        protected private void CloseButton_Click(object sender, EventArgs e) { this.Close(); }
        protected private void CloseButton_MouseEnter(object sender, EventArgs e) { closepan.BackColor = Color.IndianRed; }
        protected private void CloseButton_MouseLeave(object sender, EventArgs e) { closepan.BackColor = Color.Transparent; }
        protected private void MinimizeButton_MouseEnter(object sender, EventArgs e) { minpan.BackColor = Color.Gray; }
        protected private void MinimizeButton_MouseLeave(object sender, EventArgs e) { minpan.BackColor = Color.Transparent; }
        protected private void MaximizeButton_MouseEnter(object sender, EventArgs e) { maxpan.BackColor = Color.Gray; }
        protected private void MaximizeButton_MouseLeave(object sender, EventArgs e) { maxpan.BackColor = Color.Transparent; }
        protected private void SideBorder_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeWE; }
        protected private void CornerRight_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeNWSE; }
        protected private void CornerLeft_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeNESW; }
        protected private void Vertical_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeNS; }
        protected private void Border_MouseLeave(object sender, EventArgs e) { this.Cursor = Cursors.Default; }
        protected private void Border_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isDragging)
            {
                initialMousePos = e.Location;
                initialFormSize = this.Size;
                this.SuspendLayout();
            }
            isDragging = true;
        }
        protected private void Border_MouseUp(object sender, MouseEventArgs e) { isDragging = false; this.ResumeLayout(); }
        protected private void CornerLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = initialMousePos.X - e.X;
                int newWidth = initialFormSize.Width + deltaX;
                int deltaY = e.Y + initialMousePos.Y;
                int newHeigth = initialFormSize.Height + deltaY;
                this.Size = new Size(newWidth, newHeigth);
            }
        }
        protected private void CornerRight_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X + initialMousePos.X;
                int newWidth = initialFormSize.Width + deltaX;
                int deltaY = e.Y + initialMousePos.Y;
                int newHeigth = initialFormSize.Height + deltaY;
                this.Size = new Size(newWidth, newHeigth);
            }
        }
        protected private void SideBorderLeft_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = initialMousePos.X - e.X;
                int newWidth = initialFormSize.Width + deltaX;
                this.Size = new Size(newWidth, this.Size.Height);
            }
        }
        protected private void BottomBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaY = e.Y + initialMousePos.Y;
                int newHeigth = initialFormSize.Height + deltaY;
                this.Size = new Size(this.Size.Width, newHeigth);
            }
        }
        protected private void TopBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaY = initialMousePos.Y - e.Y;
                int newHeigth = initialFormSize.Height + deltaY;
                this.Size = new Size(this.Size.Width, newHeigth);
            }
        }
        protected private void RightBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - initialMousePos.X;
                int newWidth = initialFormSize.Width + deltaX;
                this.Size = new Size(newWidth, this.Size.Height);
            }
        }

        public Prompt()
        {
            UCommon.Windows = this;
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void Prompt_Load(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                if (File.Exists(UCommon.Xml_Path))
                {
                    USettings.LoadXml(UCommon.Xml_Path);
                }
                else if (File.Exists($@"{UCommon.Application_Path_Windows}\Resources\Code\UDesigner.xml"))
                {
                    UCommon.Windows.Invoke((Action)(() =>
                    {
                        UCommon.Windows.htmlhandler.Source = new Uri($@"{(AppDomain.CurrentDomain.BaseDirectory).Replace(@"\", "/")}Resources/Icon/USplash.html");
                    }));
                    UCommon.Xml_Path = $@"{UCommon.Application_Path_Windows}\Resources\Code\UDesigner.xml";
                    USettings.LoadXml(UCommon.Xml_Path);
                }
            });
            thread.Start();
        }

        internal void UpdateTitleBarColor()
        {
            Color currentColor = TitleBar.BackColor;
            if (UImage.IsDark(currentColor))
            {
                if (USettings.FirstLoadCompleted == false)
                {
                    UImage.ReverseImageColors(new PictureBox[] { closeButton, minimizeButton, maximizeButton });
                }
                IconLigthMode = true;
                float brightness = 0.2f;
                Color newColor = ControlPaint.Light(currentColor, brightness);
                TitleBar.BackColor = newColor;
            }
            else
            {
                if (IconLigthMode == true)
                {
                    IconLigthMode = false;
                    if (USettings.FirstLoadCompleted == false)
                    {
                        UImage.ReverseImageColors(new PictureBox[] { closeButton, minimizeButton, maximizeButton });
                    }
                }
                float brightness = 0.1f;
                Color newColor = ControlPaint.Dark(currentColor, brightness);
                TitleBar.BackColor = newColor;
            }

        }
        internal void UpdateTitleBarIconAndFunction()
        {
            bool Minimize = this.MinimizeBox;
            bool Maxize = this.MaximizeBox;
            bool Close = this.ControlBox;
            Control control1 = ButtonSplitPanel.GetControlFromPosition(0, 0);
            Control control2 = ButtonSplitPanel.GetControlFromPosition(1, 0);
            Control control3 = ButtonSplitPanel.GetControlFromPosition(2, 0);

            if (Close == false)
            {
                
                ButtonSplitPanel.SetCellPosition(control1, new TableLayoutPanelCellPosition(1, 0));
                ButtonSplitPanel.SetCellPosition(control2, new TableLayoutPanelCellPosition(2, 0));
                ButtonSplitPanel.SetCellPosition(control3, new TableLayoutPanelCellPosition(0, 0));
                closepan.Visible = false;
                if (Maxize == false)
                {
                    ButtonSplitPanel.SetCellPosition(control2, new TableLayoutPanelCellPosition(1, 0));
                    ButtonSplitPanel.SetCellPosition(control1, new TableLayoutPanelCellPosition(2, 0));
                    maxpan.Visible = false;
                }

                if (Minimize == false)
                {
                    minpan.Visible = false;
                }
            }
            else
            {
                if (Maxize == false)
                {
                    ButtonSplitPanel.SetCellPosition(control1, new TableLayoutPanelCellPosition(1, 0));
                    ButtonSplitPanel.SetCellPosition(control2, new TableLayoutPanelCellPosition(0, 0));
                    maxpan.Visible = false;
                }
                if (Minimize == false)
                {
                    minpan.Visible = false;
                }
            }
        }

        private void Htmlhandler_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (htmlhandler.CoreWebView2.Source.ToString().ToLower().Split('?')[0].Contains("http"))
            {
                UCommon.Warning("UPrompt do not support browse any internet url for security reason please stay local on machine !!!");
                htmlhandler.Source = new Uri($@"file:///{UCommon.Application_Path}UView.html");
            }
        }

        private void Htmlhandler_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                UCommon.Windows.Invoke((Action)(() =>
                {
                    UHandler.HandlePost(e.WebMessageAsJson);
                }));
            UParser.ReloadView();
            });
            thread.Start();
        }
    }
}
