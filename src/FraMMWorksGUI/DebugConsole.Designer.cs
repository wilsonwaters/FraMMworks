namespace FraMMWorksGUI
{
   partial class DebugConsole
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
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.listView_debugMessages = new System.Windows.Forms.ListView();
         this.columnDebugMessages = new System.Windows.Forms.ColumnHeader();
         this.listView_errorMessages = new System.Windows.Forms.ListView();
         this.columnErrorMessages = new System.Windows.Forms.ColumnHeader();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.SuspendLayout();
         // 
         // splitContainer1
         // 
         this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainer1.Location = new System.Drawing.Point(0, 0);
         this.splitContainer1.Name = "splitContainer1";
         this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.listView_debugMessages);
         this.splitContainer1.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.listView_errorMessages);
         this.splitContainer1.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.splitContainer1.RightToLeft = System.Windows.Forms.RightToLeft.No;
         this.splitContainer1.Size = new System.Drawing.Size(1036, 458);
         this.splitContainer1.SplitterDistance = 345;
         this.splitContainer1.TabIndex = 0;
         // 
         // listView_debugMessages
         // 
         this.listView_debugMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
         this.listView_debugMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnDebugMessages});
         this.listView_debugMessages.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listView_debugMessages.GridLines = true;
         this.listView_debugMessages.Location = new System.Drawing.Point(0, 0);
         this.listView_debugMessages.Name = "listView_debugMessages";
         this.listView_debugMessages.Size = new System.Drawing.Size(1036, 345);
         this.listView_debugMessages.TabIndex = 0;
         this.listView_debugMessages.UseCompatibleStateImageBehavior = false;
         this.listView_debugMessages.View = System.Windows.Forms.View.Details;
         // 
         // columnDebugMessages
         // 
         this.columnDebugMessages.Text = "Debug Messages";
         this.columnDebugMessages.Width = 4096;
         // 
         // listView_errorMessages
         // 
         this.listView_errorMessages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
         this.listView_errorMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnErrorMessages});
         this.listView_errorMessages.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listView_errorMessages.GridLines = true;
         this.listView_errorMessages.Location = new System.Drawing.Point(0, 0);
         this.listView_errorMessages.Name = "listView_errorMessages";
         this.listView_errorMessages.Size = new System.Drawing.Size(1036, 109);
         this.listView_errorMessages.TabIndex = 0;
         this.listView_errorMessages.UseCompatibleStateImageBehavior = false;
         this.listView_errorMessages.View = System.Windows.Forms.View.Details;
         // 
         // columnErrorMessages
         // 
         this.columnErrorMessages.Text = "Error Messages";
         this.columnErrorMessages.Width = 4096;
         // 
         // DebugConsole
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1036, 458);
         this.Controls.Add(this.splitContainer1);
         this.Name = "DebugConsole";
         this.Text = "DebugConsole";
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         this.splitContainer1.ResumeLayout(false);
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.ListView listView_debugMessages;
      private System.Windows.Forms.ListView listView_errorMessages;
      private System.Windows.Forms.ColumnHeader columnDebugMessages;
      private System.Windows.Forms.ColumnHeader columnErrorMessages;
   }
}