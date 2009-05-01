using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FraMMWorksGUI
{
   public partial class DebugConsole : Form
   {

      /// <summary>
      /// deligate the updateMessages call to the GUI thread.
      /// </summary>
      /// <param name="image"></param>
      private delegate void OnUpdateMessages(ICollection<String> debugMessages, ICollection<String> errorMessages);

      public DebugConsole()
      {
         InitializeComponent();
      }

      public void updateMessages(ICollection<String> debugMessages, ICollection<String> errorMessages)
      {
         // need to marshal this call back to the UI thread, as it may be called from a worker thread
         if (this.listView_debugMessages.InvokeRequired)
         {
            this.BeginInvoke(new OnUpdateMessages(updateMessages), new object[] { debugMessages, errorMessages });
         }
         else
         {
            this.listView_debugMessages.Items.Clear();
            foreach (String msg in debugMessages)
            {
               this.listView_debugMessages.Items.Add(msg);
            }

            this.listView_errorMessages.Items.Clear();
            foreach (String msg in errorMessages)
            {
               this.listView_errorMessages.Items.Add(msg);
            }
         }
      }
   }
}