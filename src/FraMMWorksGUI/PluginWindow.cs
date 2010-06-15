using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using FraMMWorks.Interfaces;

namespace FraMMWorksGUI
{
   /// <summary>
   /// Handles rendering of a plugin window on the topology editor.
   /// The window consists of a frame which may be resized and a title bar allowing
   /// removal and minimising of the plugin window.
   /// The body of the window (which may be hidden in minimised mode) contains either
   /// the default rendering of the plugin settings OR the custom control painted by the
   /// plugin.
   /// This class handles the resizing function when the border of the window is dragged
   /// </summary>
   class PluginWindow : ITopologyControlItem
   {
      #region consts
      /// <summary>
      /// color pallet
      /// </summary>
      private Color COLOUR_1 = Color.FromArgb(22, 22, 25);

      /// <summary>
      /// color pallet
      /// </summary>
      private Color COLOUR_2 = Color.FromArgb(30, 28, 38);

      /// <summary>
      /// color pallet
      /// </summary>
      private Color COLOUR_3 = Color.FromArgb(52, 65, 89);
      
      /// <summary>
      /// color pallet
      /// </summary>
      private Color COLOUR_4 = Color.FromArgb(92, 109, 115);
      
      /// <summary>
      /// color pallet
      /// </summary>
      private Color COLOUR_5 = Color.FromArgb(134, 151, 153);

      /// <summary>
      /// Height of the titlebar on the window
      /// </summary>
      private const int TITLE_BAR_HEGHT = 25;

      #endregion

      #region data members
      private Size size;
      /// <summary>
      /// Get the current size of this window
      /// </summary>
      public Size Size
      {
         get{return this.size;}
      }

      private Point position;
      /// <summary>
      /// Gets or sets the position of this window on the TopologyControl window.
      /// Point is top left of the window
      /// </summary>
      public Point Position
      {
         get { return this.position; }
         set { this.position = value; }
      }
      #endregion

      #region constructors
      /// <summary>
      /// Create a new PluginWindow to display the given plugin
      /// </summary>
      /// <param name="plugin">the plugin to render</param>
      public PluginWindow(IPlugin plugin)
      {
         this.size = new Size(200,100);
         this.position = new Point(40, 40);
      }

      #endregion

      #region  GUI Drawing members

      /// <summary>
      /// Paint the control on the Graphics object with top left point starting at p.
      /// This assumes the area where the control is to be placed has already been cleared
      /// or is suitable to be painted over.
      /// </summary>
      /// <param name="g">graphics to draw on</param>
      /// <param name="p">top left point to start drawing</param>
      public void Paint(Graphics g)
      {

         // draw frame
         int frameWidth = 2;
         Pen pen = new Pen(COLOUR_1, frameWidth);
         g.DrawRectangle(pen, position.X, position.Y, this.size.Width, this.size.Height);

         // draw title bar
         SolidBrush brush = new SolidBrush(COLOUR_3);
         int frameWidthOffset = frameWidth / 2;
         g.FillRectangle(brush, position.X + frameWidthOffset, position.Y + frameWidthOffset, this.size.Width - frameWidth, TITLE_BAR_HEGHT);
         pen = new Pen(COLOUR_1, 1);
         g.DrawLine(pen, position.X, position.Y + TITLE_BAR_HEGHT, position.X + this.size.Width, position.Y + TITLE_BAR_HEGHT);

         //draw background
         brush = new SolidBrush(COLOUR_5);
         g.FillRectangle(brush, position.X + frameWidthOffset, position.Y + TITLE_BAR_HEGHT + 1, this.size.Width - frameWidth, this.size.Height - TITLE_BAR_HEGHT - frameWidth);

      }

      #endregion
   }
}
