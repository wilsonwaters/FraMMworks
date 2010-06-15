namespace FraMMWorks.Core
{
   /// <remarks>
   /// Topology window
   /// </remarks>
   partial class TopologyEditor
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
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
         this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.statusStrip_bottom = new System.Windows.Forms.StatusStrip();
         this.toolStripStatusLabel_statusMessages = new System.Windows.Forms.ToolStripStatusLabel();
         this.panel1 = new System.Windows.Forms.Panel();
         this.topologyControl1 = new FraMMWorksGUI.TopologyControl();
         this.menuStrip1.SuspendLayout();
         this.statusStrip_bottom.SuspendLayout();
         this.panel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(981, 24);
         this.menuStrip1.TabIndex = 0;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // fileToolStripMenuItem
         // 
         this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
         this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
         this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
         this.fileToolStripMenuItem.Text = "File";
         // 
         // newToolStripMenuItem
         // 
         this.newToolStripMenuItem.Name = "newToolStripMenuItem";
         this.newToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
         this.newToolStripMenuItem.Text = "New";
         // 
         // openToolStripMenuItem
         // 
         this.openToolStripMenuItem.Name = "openToolStripMenuItem";
         this.openToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
         this.openToolStripMenuItem.Text = "Open";
         this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
         // 
         // saveToolStripMenuItem
         // 
         this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
         this.saveToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
         this.saveToolStripMenuItem.Text = "Save";
         // 
         // saveAsToolStripMenuItem
         // 
         this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
         this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
         this.saveAsToolStripMenuItem.Text = "Save As";
         // 
         // toolStripMenuItem1
         // 
         this.toolStripMenuItem1.Name = "toolStripMenuItem1";
         this.toolStripMenuItem1.Size = new System.Drawing.Size(121, 6);
         // 
         // closeToolStripMenuItem
         // 
         this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
         this.closeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
         this.closeToolStripMenuItem.Text = "Close";
         this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
         // 
         // helpToolStripMenuItem
         // 
         this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
         this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
         this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
         this.helpToolStripMenuItem.Text = "Help";
         // 
         // aboutToolStripMenuItem
         // 
         this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
         this.aboutToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
         this.aboutToolStripMenuItem.Text = "About";
         // 
         // statusStrip_bottom
         // 
         this.statusStrip_bottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_statusMessages});
         this.statusStrip_bottom.Location = new System.Drawing.Point(0, 598);
         this.statusStrip_bottom.Name = "statusStrip_bottom";
         this.statusStrip_bottom.Size = new System.Drawing.Size(981, 22);
         this.statusStrip_bottom.TabIndex = 1;
         this.statusStrip_bottom.Text = "statusStrip1";
         // 
         // toolStripStatusLabel_statusMessages
         // 
         this.toolStripStatusLabel_statusMessages.Name = "toolStripStatusLabel_statusMessages";
         this.toolStripStatusLabel_statusMessages.Size = new System.Drawing.Size(161, 17);
         this.toolStripStatusLabel_statusMessages.Text = "FraMMWorks Topology Designer";
         // 
         // panel1
         // 
         this.panel1.AutoScroll = true;
         this.panel1.Controls.Add(this.topologyControl1);
         this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.panel1.Location = new System.Drawing.Point(0, 24);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(981, 574);
         this.panel1.TabIndex = 2;
         // 
         // topologyControl1
         // 
         this.topologyControl1.AutoScroll = true;
         this.topologyControl1.AutoScrollMinSize = new System.Drawing.Size(400, 400);
         this.topologyControl1.BackColor = System.Drawing.SystemColors.Control;
         this.topologyControl1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.topologyControl1.Location = new System.Drawing.Point(0, 0);
         this.topologyControl1.Name = "topologyControl1";
         this.topologyControl1.Size = new System.Drawing.Size(981, 574);
         this.topologyControl1.TabIndex = 0;
         this.topologyControl1.Topology = null;
         // 
         // TopologyEditor
         // 
         this.ClientSize = new System.Drawing.Size(981, 620);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.statusStrip_bottom);
         this.Controls.Add(this.menuStrip1);
         this.MainMenuStrip = this.menuStrip1;
         this.Name = "TopologyEditor";
         this.Text = "FraMMWorks Topology Designer";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Topology_FormClosing);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.statusStrip_bottom.ResumeLayout(false);
         this.statusStrip_bottom.PerformLayout();
         this.panel1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
      private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
      private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
      private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
      private System.Windows.Forms.StatusStrip statusStrip_bottom;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_statusMessages;
      private System.Windows.Forms.Panel panel1;
      private FraMMWorksGUI.TopologyControl topologyControl1;
   }
}