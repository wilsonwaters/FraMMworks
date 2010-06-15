using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;

using FraMMWorks.Core;

namespace FraMMWorksGUI
{
   /// <summary>
   /// Display a FraMMWorks topology and allow editing.
   /// </summary>
   public class TopologyControl : ScrollableControl
   {
      //-----------------consts----------------------------------------------
      /// <summary>
      /// Minimum width for this component
      /// </summary>
      private const int MIN_WIDTH = 400;

      /// <summary>
      /// Minimum height for this component
      /// </summary>
      private const int MIN_HEIGHT = 400;

      //-----------------private data----------------------------------------
      /// <summary>
      /// The double buffer of the current screen
      /// </summary>
      private System.Drawing.Bitmap db;

      /// <summary>
      /// Size of the internal bitmap of this control.
      /// Is the max of AutoScrollMinimum and control size
      /// </summary>
      private Size currentSize;

      /// <summary>
      /// Whether the mouse is currently being dragged
      /// </summary>
      private bool dragging = false;

      /// <summary>
      /// The current zoom factor of the control
      /// </summary>
      private float zoom = 1.0f;

      /// <summary>
      /// All plugin windows currently being drawn by the control
      /// </summary>
      List<PluginWindow> pluginWindows;

      /// <summary>
      /// whether to draw shadows under the plugin windows
      /// </summary>
      private bool shadows;

      /// <summary>
      /// The currently selected item
      /// </summary>
      private ITopologyControlItem selectedItem;

      /// <summary>
      /// the last registered mouse point
      /// </summary>
      Point lastPoint;

      //-----------------public data-----------------------------------------
      /// <summary>
      /// Get or set the topology this control operates on
      /// </summary>
      public ITopology Topology
      {
         get { return this.topology; }
         set 
         { 
            this.topology = value;
            
            //ww testing
            if (topology != null && topology.GetActivePlugins().Count > 0)
               pluginWindows.Add(new PluginWindow(topology.GetActivePlugins()[0]));

            this.Invalidate();
         }
      }
      private ITopology topology;

      //-----------------constructors----------------------------------------

      /// <summary>
      /// default constructor
      /// </summary>
      public TopologyControl()
      {
         pluginWindows = new List<PluginWindow>();
         lastPoint = new Point(0, 0);
         shadows = true;

         ResizeRedraw = true;
         this.AutoScroll = true;
         this.AutoScrollMinSize = new Size(MIN_WIDTH, MIN_HEIGHT);
         setCurrentSize();
         db = new Bitmap(this.currentSize.Width, this.currentSize.Height);
         this.DoubleBuffered = true;

      }

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing)
         {
            // get rid of stuff by calling their dispose methods
            db.Dispose();
         }
         base.Dispose(disposing);
      }

      //--------------graphics methods---------------------------------------

      private void paintDB()
      {
         Graphics graphics = Graphics.FromImage(db);
         graphics.Clear(this.BackColor);

         foreach (PluginWindow w in this.pluginWindows)
         {
            if (shadows)
            {
               int shadowDistance = 4;
               SolidBrush brush = new SolidBrush(Color.FromArgb(190,190,190));
               graphics.FillRectangle(brush, w.Position.X + shadowDistance + 1, w.Position.Y + shadowDistance + 1, w.Size.Width, w.Size.Height);
               graphics.FillRectangle(brush, w.Position.X + shadowDistance - 1, w.Position.Y + shadowDistance - 1, w.Size.Width, w.Size.Height);
               brush = new SolidBrush(Color.FromArgb(150, 150, 150));
               graphics.FillRectangle(brush, w.Position.X + shadowDistance, w.Position.Y + shadowDistance, w.Size.Width, w.Size.Height);
               brush = new SolidBrush(Color.FromArgb(190, 190, 190));
               graphics.FillRectangle(brush, w.Position.X + shadowDistance + w.Size.Width - 1, w.Position.Y + shadowDistance + w.Size.Height - 1, 1, 1);
               graphics.FillRectangle(brush, w.Position.X + shadowDistance + w.Size.Width - 1, w.Position.Y + shadowDistance, 1, 1);
               graphics.FillRectangle(brush, w.Position.X + shadowDistance, w.Position.Y + shadowDistance + w.Size.Height - 1, 1, 1);
               brush = new SolidBrush(this.BackColor);
               graphics.FillRectangle(brush, w.Position.X + shadowDistance + w.Size.Width, w.Position.Y + shadowDistance + w.Size.Height, 1, 1);
            }
            w.Paint(graphics);
         }

      }

      protected override void OnPaint(PaintEventArgs e)
      {
         // paint the bitmap first
         paintDB();

         //now paint it to the screen
         Graphics graphics = e.Graphics;

         Console.WriteLine("painting autoscroll={0}, zoom={1}", AutoScrollPosition, zoom);

         // handle scrolling
         graphics.ScaleTransform(zoom, zoom);
         graphics.TranslateTransform(AutoScrollPosition.X * (1.0F / zoom), AutoScrollPosition.Y * (1.0F / zoom));


         graphics.DrawImage(db, 0, 0);

      }

      /// <summary>
      /// Determines if something is under the given point
      /// </summary>
      /// <returns>The item which is under the current point</returns>
      private ITopologyControlItem HitItem(Point p)
      {
         foreach (PluginWindow w in this.pluginWindows)
         {
            if (p.X >= w.Position.X && p.X <= w.Position.X + w.Size.Width &&
                p.Y >= w.Position.Y && p.Y <= w.Position.Y + w.Size.Height)
            {
               return w;
            }
         }
         return null;
      }

      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);
         setCurrentSize();

         // recreate the offscreen buffer
         if (db != null)
         {
            db.Dispose();
         }
         db = new Bitmap(currentSize.Width, currentSize.Height);
      }

      private void UpdateMinimumSize(int width, int height)
      {
         this.AutoScrollMinSize = new Size(width, height);
         setCurrentSize();

         // recreate the offscreen buffer
         if (db != null)
         {
            db.Dispose();
         }
         db = new Bitmap(currentSize.Width, currentSize.Height);
         this.Invalidate();
      }

      private void setCurrentSize()
      {
         if (this.currentSize == null)
            this.currentSize = new Size();
         this.currentSize.Width = Math.Max(this.Width, this.AutoScrollMinSize.Width);
         this.currentSize.Height = Math.Max(this.Height, this.AutoScrollMinSize.Height);
      }

      protected override void OnScroll(ScrollEventArgs e)
      {
         this.Invalidate();
      }

      /// <summary>
      /// Gets the mouse location according to the virtual position in the control.
      /// </summary>
      /// <returns></returns>
      protected Point GetVirtualMouseLocation(MouseEventArgs e)
      {
         // http://www.bobpowell.net/backtrack.htm

         //Creates the drawing matrix with the right zoom;
         Matrix mx = new Matrix(zoom, 0, 0, zoom, 0, 0);
         //pans it according to the scroll bars
         mx.Translate(this.AutoScrollPosition.X * (1.0f / zoom), this.AutoScrollPosition.Y * (1.0f / zoom));
         //inverts it
         mx.Invert();

         //uses it to transform the current mouse position
         Point[] pa = new Point[] { new Point(e.X, e.Y) };
         mx.TransformPoints(pa);

         return pa[0];
      }

      protected override void OnMouseDown(MouseEventArgs e)
      {
         base.OnMouseDown(e);


         // get the virtual mouse location using the transforms
         Point mouseLocation = GetVirtualMouseLocation(e);

         // check if it's hit an object
         selectedItem = HitItem(mouseLocation);
         lastPoint.X = mouseLocation.X;
         lastPoint.Y = mouseLocation.Y;
         dragging = true;
      }

      protected override void OnMouseUp(MouseEventArgs e)
      {
         base.OnMouseUp(e);

         // get the virtual mouse location using the transforms
         Point mouseLocation = GetVirtualMouseLocation(e);

         //set that we're not currently dragging
         selectedItem = null;
         dragging = false;
      }

      protected override void OnMouseMove(MouseEventArgs e)
      {
         base.OnMouseMove(e);

         // get the virtual mouse location using the transforms
         Point mouseLocation = GetVirtualMouseLocation(e);
         int deltaX = lastPoint.X - mouseLocation.X;
         int deltaY = lastPoint.Y - mouseLocation.Y;

         if (selectedItem != null)
         {
            //ww assume for now
            PluginWindow w = (PluginWindow)selectedItem;
            Point point = w.Position;
            point.X -= deltaX;
            point.Y -= deltaY;
            w.Position = point;
            this.Invalidate();

            // scroll the window if we move things outside it
            if (w.Position.X + w.Size.Width > AutoScrollMinSize.Width)
            {
               UpdateMinimumSize(w.Position.X + w.Size.Width, currentSize.Height);
            }
            if (w.Position.Y + w.Size.Height > AutoScrollMinSize.Height)
            {
               UpdateMinimumSize(currentSize.Width, w.Position.Y + w.Size.Height);
            }
         }
         else if (dragging)
         {
            // somehow drag the whole control
         }
         lastPoint.X = mouseLocation.X;
         lastPoint.Y = mouseLocation.Y;
      }

      protected override void OnMouseWheel(MouseEventArgs e)
      {
         //base.OnMouseWheel(e);
         this.zoom += ((float)e.Delta/120.0f)*0.1f;
         if (this.zoom < 0.1)
            this.zoom = 0.1F;
         if (this.zoom > 10)
            this.zoom = 10.0F;
         this.Invalidate();
      }

   }
}
