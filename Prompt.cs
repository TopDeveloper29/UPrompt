﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using UPrompt.Class;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;


namespace UPrompt
{
    public partial class Prompt : Form
    {
        internal XmlDocument xmlDoc = new XmlDocument();
        internal bool IconLigthMode = false;

        protected private bool isDragging = false;
        protected private Point dragOffset;
        protected private Point initialMousePos;
        protected private Size initialFormSize;

        protected private void titleBar_MouseMove(object sender, MouseEventArgs e)
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
        protected private void titleBar_MouseDown(object sender, MouseEventArgs e) { isDragging = true; dragOffset = e.Location; }
        protected private void titleBar_MouseUp(object sender, MouseEventArgs e) { isDragging = false; }
        protected private void minimizeButton_Click(object sender, EventArgs e) { this.WindowState = FormWindowState.Minimized; }
        protected private void maximizeButton_Click(object sender, EventArgs e) { if (this.WindowState == FormWindowState.Maximized) { this.WindowState = FormWindowState.Normal; } else { this.WindowState = FormWindowState.Maximized; } }
        protected private void closeButton_Click(object sender, EventArgs e) { this.Close(); }
        protected private void closeButton_MouseEnter(object sender, EventArgs e) { closepan.BackColor = Color.IndianRed; }
        protected private void closeButton_MouseLeave(object sender, EventArgs e) { closepan.BackColor = Color.Transparent; }
        protected private void minimizeButton_MouseEnter(object sender, EventArgs e) { minpan.BackColor = Color.Gray; }
        protected private void minimizeButton_MouseLeave(object sender, EventArgs e) { minpan.BackColor = Color.Transparent; }
        protected private void maximizeButton_MouseEnter(object sender, EventArgs e) { maxpan.BackColor = Color.Gray; }
        protected private void maximizeButton_MouseLeave(object sender, EventArgs e) { maxpan.BackColor = Color.Transparent; }
        protected private void SideBorder_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeWE; }
        protected private void CornerRight_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeNWSE; }
        protected private void CornerLeft_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeNESW; }
        protected private void Bottom_MouseHover(object sender, EventArgs e) { this.Cursor = Cursors.SizeNS; }
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
            Common.Windows = this;
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void Prompt_Load(object sender, EventArgs e) { Settings.LoadXml(Common.Xml_Path); }
        private void htmlhandler_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.ToString().ToLower().Contains("http"))
            {
                Common.Warning("UPrompt do not support browse any internet url for security reason please stay local on machine !!!");
                htmlhandler.Navigate($@"file:///{Common.Application_Path}View.html");
            }
            if (e.Url.ToString().Contains("="))
            {
                if (e.Url.ToString().Contains("&"))
                {
                    string action_code = System.Net.WebUtility.UrlDecode(e.Url.ToString()).Split('?')[1];
                    string[] actions = action_code.Split('&');
                    foreach (string action in actions)
                    {
                        string action_name = action.Split('=')[0];
                        string action_value = ViewParser.ParseSystemText(action.Split('=')[1]);
                        if (action_name.Contains("INPUT_"))
                        {
                            Handler.RunAction(action_name, action_value);
                        }
                    }
                    foreach (string action in actions)
                    {
                        string action_name = action.Split('=')[0];
                        string action_value = ViewParser.ParseSystemText(action.Split('=')[1]);
                        if (!action_name.Contains("INPUT_"))
                        {
                            Handler.RunAction(action_name, action_value);
                        }
                    }
                }
                else
                {
                    string action_code = System.Net.WebUtility.UrlDecode(e.Url.ToString()).Split('?')[1];
                    string action_name = action_code.Split('=')[0];
                    string action_value = ViewParser.ParseSystemText(action_code.Split('=')[1]);
                    Handler.RunAction(action_name, action_value);
                }
            }
        }
        internal static bool IsDark(Color color)
        {
            double perceivedBrightness = (color.R * 0.299 + color.G * 0.587 + color.B * 0.114) / 255;
            return perceivedBrightness <= 0.5;
        }
        internal void ReverseImageColors(PictureBox[] pictureBoxs)
        {
            foreach (PictureBox pictureBox in pictureBoxs)
            {
                Image image = pictureBox.Image;
                if (image != null)
                {
                    Bitmap originalBitmap = new Bitmap(image.Width, image.Height);
                    using (Graphics graphics = Graphics.FromImage(originalBitmap))
                    {
                        using (ImageAttributes attributes = new ImageAttributes())
                        {
                            ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                            {
                                new float[] {-1, 0, 0, 0, 0},
                                new float[] {0, -1, 0, 0, 0},
                                new float[] {0, 0, -1, 0, 0},
                                new float[] {0, 0, 0, 1, 0},
                                new float[] {1, 1, 1, 0, 1}
                            });

                            attributes.SetColorMatrix(colorMatrix);
                            graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                        }
                    }
                    pictureBox.Image = originalBitmap;
                }
            }
        }
        internal void UpdateTitleBarColor()
        {
            Color currentColor = TitleBar.BackColor;
            if (IsDark(currentColor))
            {
                ReverseImageColors(new PictureBox[] { closeButton, minimizeButton, maximizeButton });
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
                    ReverseImageColors(new PictureBox[] { closeButton, minimizeButton, maximizeButton });
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
    }
}
