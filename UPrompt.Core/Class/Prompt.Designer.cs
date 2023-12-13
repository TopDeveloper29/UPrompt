using System;
using System.Drawing;

namespace UPrompt
{
    partial class Prompt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.htmlhandler = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.TitleBar = new System.Windows.Forms.Panel();
            this.Top = new System.Windows.Forms.Panel();
            this.ButtonSplitPanel = new System.Windows.Forms.TableLayoutPanel();
            this.closepan = new System.Windows.Forms.Panel();
            this.closeButton = new System.Windows.Forms.PictureBox();
            this.maxpan = new System.Windows.Forms.Panel();
            this.maximizeButton = new System.Windows.Forms.PictureBox();
            this.minpan = new System.Windows.Forms.Panel();
            this.minimizeButton = new System.Windows.Forms.PictureBox();
            this.Left = new System.Windows.Forms.Panel();
            this.cornerleft = new System.Windows.Forms.Panel();
            this.Bottom = new System.Windows.Forms.Panel();
            this.cornerright = new System.Windows.Forms.Panel();
            this.Right = new System.Windows.Forms.Panel();
            this.body = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.htmlhandler)).BeginInit();
            this.TitleBar.SuspendLayout();
            this.ButtonSplitPanel.SuspendLayout();
            this.closepan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).BeginInit();
            this.maxpan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maximizeButton)).BeginInit();
            this.minpan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeButton)).BeginInit();
            this.Left.SuspendLayout();
            this.Bottom.SuspendLayout();
            this.body.SuspendLayout();
            this.SuspendLayout();
            // 
            // htmlhandler
            // 
            this.htmlhandler.AllowExternalDrop = true;
            this.htmlhandler.CreationProperties = null;
            this.htmlhandler.DefaultBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.htmlhandler.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlhandler.Location = new System.Drawing.Point(0, 0);
            this.htmlhandler.Margin = new System.Windows.Forms.Padding(0);
            this.htmlhandler.MinimumSize = new System.Drawing.Size(20, 20);
            this.htmlhandler.Name = "htmlhandler";
            this.htmlhandler.Size = new System.Drawing.Size(781, 413);
            this.htmlhandler.TabIndex = 0;
            this.htmlhandler.ZoomFactor = 1D;
            this.htmlhandler.NavigationCompleted += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs>(this.Htmlhandler_NavigationCompleted);
            this.htmlhandler.WebMessageReceived += new System.EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs>(this.Htmlhandler_WebMessageReceived);
            // 
            // TitleBar
            // 
            this.TitleBar.Controls.Add(this.Top);
            this.TitleBar.Controls.Add(this.ButtonSplitPanel);
            this.TitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitleBar.Location = new System.Drawing.Point(0, 0);
            this.TitleBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TitleBar.Name = "TitleBar";
            this.TitleBar.Size = new System.Drawing.Size(800, 25);
            this.TitleBar.TabIndex = 1;
            this.TitleBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseDown);
            this.TitleBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseMove);
            this.TitleBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TitleBar_MouseUp);
            // 
            // Top
            // 
            this.Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.Top.Location = new System.Drawing.Point(0, 0);
            this.Top.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Top.Name = "Top";
            this.Top.Size = new System.Drawing.Size(695, 9);
            this.Top.TabIndex = 1;
            this.Top.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Border_MouseDown);
            this.Top.MouseLeave += new System.EventHandler(this.Border_MouseLeave);
            this.Top.MouseHover += new System.EventHandler(this.Vertical_MouseHover);
            this.Top.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TopBorder_MouseMove);
            this.Top.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Border_MouseUp);
            // 
            // ButtonSplitPanel
            // 
            this.ButtonSplitPanel.ColumnCount = 3;
            this.ButtonSplitPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ButtonSplitPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ButtonSplitPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.ButtonSplitPanel.Controls.Add(this.closepan, 2, 0);
            this.ButtonSplitPanel.Controls.Add(this.maxpan, 1, 0);
            this.ButtonSplitPanel.Controls.Add(this.minpan, 0, 0);
            this.ButtonSplitPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.ButtonSplitPanel.Location = new System.Drawing.Point(695, 0);
            this.ButtonSplitPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ButtonSplitPanel.Name = "ButtonSplitPanel";
            this.ButtonSplitPanel.RowCount = 1;
            this.ButtonSplitPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.ButtonSplitPanel.Size = new System.Drawing.Size(105, 25);
            this.ButtonSplitPanel.TabIndex = 0;
            // 
            // closepan
            // 
            this.closepan.Controls.Add(this.closeButton);
            this.closepan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.closepan.Location = new System.Drawing.Point(70, 0);
            this.closepan.Margin = new System.Windows.Forms.Padding(0);
            this.closepan.Name = "closepan";
            this.closepan.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.closepan.Size = new System.Drawing.Size(35, 25);
            this.closepan.TabIndex = 2;
            this.closepan.Click += new System.EventHandler(this.CloseButton_Click);
            this.closepan.MouseEnter += new System.EventHandler(this.CloseButton_MouseEnter);
            this.closepan.MouseLeave += new System.EventHandler(this.CloseButton_MouseLeave);
            // 
            // closeButton
            // 
            this.closeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.closeButton.Image = global::UPrompt.Core.Properties.Resources.CloseIcon;
            this.closeButton.Location = new System.Drawing.Point(3, 5);
            this.closeButton.Margin = new System.Windows.Forms.Padding(0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(29, 15);
            this.closeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.closeButton.TabIndex = 2;
            this.closeButton.TabStop = false;
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.CloseButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.CloseButton_MouseLeave);
            // 
            // maxpan
            // 
            this.maxpan.Controls.Add(this.maximizeButton);
            this.maxpan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maxpan.Location = new System.Drawing.Point(35, 0);
            this.maxpan.Margin = new System.Windows.Forms.Padding(0);
            this.maxpan.Name = "maxpan";
            this.maxpan.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.maxpan.Size = new System.Drawing.Size(35, 25);
            this.maxpan.TabIndex = 1;
            this.maxpan.Click += new System.EventHandler(this.MaximizeButton_Click);
            this.maxpan.MouseEnter += new System.EventHandler(this.MaximizeButton_MouseEnter);
            this.maxpan.MouseLeave += new System.EventHandler(this.MaximizeButton_MouseLeave);
            // 
            // maximizeButton
            // 
            this.maximizeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maximizeButton.Image = global::UPrompt.Core.Properties.Resources.MaximizeIcon;
            this.maximizeButton.Location = new System.Drawing.Point(3, 5);
            this.maximizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.maximizeButton.Name = "maximizeButton";
            this.maximizeButton.Size = new System.Drawing.Size(29, 15);
            this.maximizeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.maximizeButton.TabIndex = 1;
            this.maximizeButton.TabStop = false;
            this.maximizeButton.Click += new System.EventHandler(this.MaximizeButton_Click);
            this.maximizeButton.MouseEnter += new System.EventHandler(this.MaximizeButton_MouseEnter);
            this.maximizeButton.MouseLeave += new System.EventHandler(this.MaximizeButton_MouseLeave);
            // 
            // minpan
            // 
            this.minpan.Controls.Add(this.minimizeButton);
            this.minpan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minpan.Location = new System.Drawing.Point(0, 0);
            this.minpan.Margin = new System.Windows.Forms.Padding(0);
            this.minpan.Name = "minpan";
            this.minpan.Padding = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.minpan.Size = new System.Drawing.Size(35, 25);
            this.minpan.TabIndex = 0;
            this.minpan.Click += new System.EventHandler(this.MinimizeButton_Click);
            this.minpan.MouseEnter += new System.EventHandler(this.MinimizeButton_MouseEnter);
            this.minpan.MouseLeave += new System.EventHandler(this.MinimizeButton_MouseLeave);
            // 
            // minimizeButton
            // 
            this.minimizeButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minimizeButton.Image = global::UPrompt.Core.Properties.Resources.MinimizeIcon;
            this.minimizeButton.Location = new System.Drawing.Point(3, 5);
            this.minimizeButton.Margin = new System.Windows.Forms.Padding(0);
            this.minimizeButton.Name = "minimizeButton";
            this.minimizeButton.Size = new System.Drawing.Size(29, 15);
            this.minimizeButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.minimizeButton.TabIndex = 0;
            this.minimizeButton.TabStop = false;
            this.minimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            this.minimizeButton.MouseEnter += new System.EventHandler(this.MinimizeButton_MouseEnter);
            this.minimizeButton.MouseLeave += new System.EventHandler(this.MinimizeButton_MouseLeave);
            // 
            // Left
            // 
            this.Left.Controls.Add(this.cornerleft);
            this.Left.Dock = System.Windows.Forms.DockStyle.Left;
            this.Left.Location = new System.Drawing.Point(0, 25);
            this.Left.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Left.Name = "Left";
            this.Left.Size = new System.Drawing.Size(9, 425);
            this.Left.TabIndex = 2;
            this.Left.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Border_MouseDown);
            this.Left.MouseLeave += new System.EventHandler(this.Border_MouseLeave);
            this.Left.MouseHover += new System.EventHandler(this.SideBorder_MouseHover);
            this.Left.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SideBorderLeft_MouseMove);
            this.Left.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Border_MouseUp);
            // 
            // cornerleft
            // 
            this.cornerleft.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cornerleft.Location = new System.Drawing.Point(0, 409);
            this.cornerleft.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cornerleft.Name = "cornerleft";
            this.cornerleft.Size = new System.Drawing.Size(9, 16);
            this.cornerleft.TabIndex = 1;
            this.cornerleft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Border_MouseDown);
            this.cornerleft.MouseLeave += new System.EventHandler(this.Border_MouseLeave);
            this.cornerleft.MouseHover += new System.EventHandler(this.CornerLeft_MouseHover);
            this.cornerleft.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CornerLeft_MouseMove);
            this.cornerleft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Border_MouseUp);
            // 
            // Bottom
            // 
            this.Bottom.Controls.Add(this.cornerright);
            this.Bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Bottom.Location = new System.Drawing.Point(9, 438);
            this.Bottom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Bottom.Name = "Bottom";
            this.Bottom.Size = new System.Drawing.Size(791, 12);
            this.Bottom.TabIndex = 3;
            this.Bottom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Border_MouseDown);
            this.Bottom.MouseLeave += new System.EventHandler(this.Border_MouseLeave);
            this.Bottom.MouseHover += new System.EventHandler(this.Vertical_MouseHover);
            this.Bottom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BottomBorder_MouseMove);
            this.Bottom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Border_MouseUp);
            // 
            // cornerright
            // 
            this.cornerright.Dock = System.Windows.Forms.DockStyle.Right;
            this.cornerright.Location = new System.Drawing.Point(781, 0);
            this.cornerright.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cornerright.Name = "cornerright";
            this.cornerright.Size = new System.Drawing.Size(10, 12);
            this.cornerright.TabIndex = 0;
            this.cornerright.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Border_MouseDown);
            this.cornerright.MouseLeave += new System.EventHandler(this.Border_MouseLeave);
            this.cornerright.MouseHover += new System.EventHandler(this.CornerRight_MouseHover);
            this.cornerright.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CornerRight_MouseMove);
            this.cornerright.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Border_MouseUp);
            // 
            // Right
            // 
            this.Right.Dock = System.Windows.Forms.DockStyle.Right;
            this.Right.Location = new System.Drawing.Point(790, 25);
            this.Right.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Right.Name = "Right";
            this.Right.Size = new System.Drawing.Size(10, 413);
            this.Right.TabIndex = 4;
            this.Right.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Border_MouseDown);
            this.Right.MouseLeave += new System.EventHandler(this.Border_MouseLeave);
            this.Right.MouseHover += new System.EventHandler(this.SideBorder_MouseHover);
            this.Right.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RightBorder_MouseMove);
            this.Right.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Border_MouseUp);
            // 
            // body
            // 
            this.body.Controls.Add(this.htmlhandler);
            this.body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.body.Location = new System.Drawing.Point(9, 25);
            this.body.Name = "body";
            this.body.Size = new System.Drawing.Size(781, 413);
            this.body.TabIndex = 5;
            // 
            // Prompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.body);
            this.Controls.Add(this.Right);
            this.Controls.Add(this.Bottom);
            this.Controls.Add(this.Left);
            this.Controls.Add(this.TitleBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = global::UPrompt.Core.Properties.Resources.uicon;
            this.Name = "Prompt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UPrompt DEMO";
            this.Load += new System.EventHandler(this.Prompt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.htmlhandler)).EndInit();
            this.TitleBar.ResumeLayout(false);
            this.ButtonSplitPanel.ResumeLayout(false);
            this.closepan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.closeButton)).EndInit();
            this.maxpan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.maximizeButton)).EndInit();
            this.minpan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.minimizeButton)).EndInit();
            this.Left.ResumeLayout(false);
            this.Bottom.ResumeLayout(false);
            this.body.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel ButtonSplitPanel;
        private System.Windows.Forms.PictureBox minimizeButton;
        private System.Windows.Forms.PictureBox maximizeButton;
        private System.Windows.Forms.PictureBox closeButton;
        new internal System.Windows.Forms.Panel Left;
        new internal System.Windows.Forms.Panel Bottom;
        new internal System.Windows.Forms.Panel Right;
        private System.Windows.Forms.Panel closepan;
        private System.Windows.Forms.Panel maxpan;
        private System.Windows.Forms.Panel minpan;
        internal System.Windows.Forms.Panel TitleBar;
        private System.Windows.Forms.Panel body;
        internal System.Windows.Forms.Panel cornerleft;
        internal System.Windows.Forms.Panel cornerright;
        public Microsoft.Web.WebView2.WinForms.WebView2 htmlhandler;
        internal new System.Windows.Forms.Panel Top;
    }
}

