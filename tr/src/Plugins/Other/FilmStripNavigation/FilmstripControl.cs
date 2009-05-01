using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FilmstripNavigation
{
   /// <remarks>
   /// Displays a filmstrip style display of images.
   /// </remarks>
   public partial class FilmstripControl : ScrollableControl
   {

      //------------------------- consts ----------------------------------------
      /// <summary>
      /// Height of th actual image within the filmstrip. Determines all other
      /// scaling around the filmstrip.
      /// </summary>
      private int DEFAULT_IMAGE_HEIGHT = 100;

      //------------------------- public structs --------------------------------
      /// <summary>
      /// Contains information about each scene, such as start and end frames.
      /// </summary>
      public class FrameInfo
      {
         /// <summary>
         /// The first frame of this scene
         /// </summary>
         public uint startFrame;

         /// <summary>
         /// The last frame on this scene
         /// </summary>
         public uint endFrame;

         /// <summary>
         /// An image representing this scene.
         /// </summary>
         public Image image;

         /// <summary>
         /// Whether this image is selected.
         /// </summary>
         public bool selected;

      };

      //------------------------- private data --------------------------------
      private int imageHeight;
      private Object imageHeightChangeLock;

      /// <summary>
      /// The SceneInfo items in this control
      /// </summary>
      private FilmstripControlFrameCollection items;

      // details for drawing filmstrip.
      private int edgeHeight;
      private int holesWidth;
      private int holesHeight;
      private int holesSpacing;
      private int holesPixelsFremEdge;
      private int totalHeight;
      private int imageSeparation;
      private int imageBorder;
      private int imageWidth;

      /// <summary>
      /// buffer for drawing the filmstrip before it's drawn to the screen.
      /// </summary>
      private Bitmap drawingSpace;

      /// <summary>
      /// whether items in this control may be selected or not.
      /// </summary>
      private bool allowSelection;

      //------------------------- public properties ---------------------------
      /// <summary>
      /// Height of th actual image within the filmstrip. Determines all other
      /// scaling around the filmstrip.
      /// </summary>
      public int ImageHeight
      {
         get { return imageHeight; }
         set { imageHeight_Changed(value); }
      }

      /// <summary>
      /// Get the collection of items in the filmstrip.
      /// </summary>
      public FilmstripControlFrameCollection Items
      {
         get { return items; }
      }

      /// <summary>
      /// Whether items in this collection may be selected.
      /// </summary>
      public bool AllowSelection
      {
         get { return allowSelection; }
         set { allowSelection = value; }
      }

      /// <summary>
      /// provide implementation of this method for handling a frame clicked
      /// event
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      /// <returns></returns>
      public delegate void FrameClickedHandler(object sender, FrameInfo frame);

      /// <summary>
      /// Attach this event to be provided with information on when a frame
      /// is clicked.
      /// </summary>
      public event FrameClickedHandler FrameClicked;


      //------------------------- constructors ---------------------------
      public FilmstripControl()
      {
         InitializeComponent();

         // make sure this control is always the width of its parent
         //this.Dock = DockStyle.None;
         //this.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
         this.Size = new Size(100, 100);
         this.AutoScroll = true;
         this.HScroll = false;
         imageHeightChangeLock = new Object();
         imageHeight_Changed(DEFAULT_IMAGE_HEIGHT);
         
         // set up the items and get notification of changes
         items = new FilmstripControlFrameCollection();
         items.Changed += new FilmstripControlFrameCollection.ChangedEventHandler(items_Changed);

         allowSelection = true;
      }

      protected override void OnPaint(PaintEventArgs pe)
      {
         if (drawingSpace == null)
            return;

         Graphics g = this.CreateGraphics();

         // handle scroll bar by only drawing the visible part of the drawingSpace
         if (!this.HScroll)
         {
            g.DrawImageUnscaled(drawingSpace, 0, 0);
         }
         else
         {
            g.DrawImage(drawingSpace, 0, 0, new Rectangle(-AutoScrollPosition.X, 0, this.Width, this.Height), GraphicsUnit.Pixel);
         }

         g.Dispose();

         // Calling the base class OnPaint
         base.OnPaint(pe);
      }

      /// <summary>
      /// Re-draws the control to the internal drawingSpace buffer
      /// </summary>
      private void drawControl()
      {
         // set the width and height of this bitmap
         int width = Math.Max(this.Width, items.Count * (imageWidth + imageSeparation) +imageSeparation);
         int height = totalHeight;

         // create bitmap, leaving enough space for the scrol bar.
         drawingSpace = new Bitmap(width, height);
         Graphics g = Graphics.FromImage(drawingSpace);

         // some brushes
         Brush backgroundBrush = Brushes.Black;
         Brush filmHolesBrush = Brushes.WhiteSmoke;
         Brush selectedBrush = Brushes.Red;

         // background
         g.FillRectangle(backgroundBrush, 0, 0, width, height);

         // film holes
         for (int x = 5; x < width; x += holesSpacing)
         {
            g.FillRectangle(filmHolesBrush, x, holesPixelsFremEdge, holesWidth, holesHeight);
            g.FillRectangle(filmHolesBrush, x, totalHeight-holesPixelsFremEdge-holesHeight, holesWidth, holesHeight);
         }

         // images
         for (int i = 0; i < items.Count; i++)
         {
            // if it's selected, draw a red border around it (only if enabled)
            if (allowSelection && items[i].selected)
               g.FillRectangle(selectedBrush, i * (imageWidth + imageSeparation) - imageBorder, edgeHeight - imageBorder, imageWidth + imageBorder*2, imageHeight + imageBorder*2);

            //If the image is null, draw a blank frame, otherwise the image.
            if (items[i].image == null)
               g.FillRectangle(Brushes.WhiteSmoke, i * (imageWidth + imageSeparation), edgeHeight, imageWidth, imageHeight);
            else
               g.DrawImage(items[i].image, i * (imageWidth + imageSeparation), edgeHeight, imageWidth, imageHeight);
         }

         g.Dispose();

         this.AutoScrollMinSize = new Size(width, height);
         this.Invalidate();
      }

      private int imageHeight_Changed(int newHeight)
      {
         lock (imageHeightChangeLock)
         {
            imageHeight = newHeight;
            edgeHeight = (int)(imageHeight * 0.25);
            holesWidth = (int)(imageHeight * 0.09);
            holesHeight = (int)(imageHeight * 0.12);
            holesSpacing = (int)(imageHeight * 0.21);
            holesPixelsFremEdge = (int)(imageHeight * 0.075);
            totalHeight = imageHeight + edgeHeight * 2;
            imageSeparation = 5;
            imageBorder = imageSeparation / 2;
            imageWidth = (int)(((double)imageHeight) * (4.0 / 3.0));
         }

         return imageHeight;
      }

      // Detemine which frame was cicked and send an event to anyone who cares
      private void FilmstripControl_Click(object sender, EventArgs e)
      {
         // Work out the position in the whole filmstrip which was clicked
         int drawingX = (e as MouseEventArgs).X + (-AutoScrollPosition.X);

         // translate drawingX to an index in the items array
         int i = (int) (drawingX / (imageWidth + imageSeparation - imageBorder));

         // mark this image as selected
         items[i].selected = !items[i].selected;

         // redraw the items
         drawControl();

         // notify any listeners
         if (FrameClicked != null)
            FrameClicked.Invoke(this, items[i]);
      }

      // make sure the control is always as wide as it can be.
      void Parent_SizeChanged(object sender, System.EventArgs e)
      {
         Control parent = sender as Control;
         if (parent != null)
         {
            this.Size = new Size(parent.ClientSize.Width - 7, totalHeight + SystemInformation.HorizontalScrollBarHeight);

            // and re-draw the control.
            drawControl();
         }
      }

      private void FilmstripControl_ParentChanged(object sender, EventArgs e)
      {
         if (this.Parent != null)
         {
            this.Parent.SizeChanged += new System.EventHandler(Parent_SizeChanged);

            // set the initial width (and remember to leave enough room for the scrollbar)
            this.Size = new Size(this.Parent.ClientSize.Width - 7, totalHeight + SystemInformation.VerticalScrollBarWidth);

            // and re-draw the control.
            drawControl();
         }
      }

      void items_Changed(object sender, EventArgs e)
      {
         drawControl();
      }

      /// <summary>
      /// Internal class to hold a collection of items and notify the
      /// parent class when the list changes.
      /// </summary>
      public class FilmstripControlFrameCollection : List<FrameInfo>
      {
         // A delegate type for hooking up change notifications.
         public delegate void ChangedEventHandler(object sender, EventArgs e);

         // An event that clients can use to be notified whenever the
         // elements of the list change.
         public event ChangedEventHandler Changed;

         // Invoke the Changed event; called whenever list changes
         protected virtual void OnChanged(EventArgs e)
         {
            if (Changed != null)
               Changed(this, e);
         }

         // Override some of the methods that can change the list;
         // invoke event after each
         public new void Add(FrameInfo item)
         {
            base.Add(item);
            OnChanged(EventArgs.Empty);
         }

         public new void Clear()
         {
            base.Clear();
            OnChanged(EventArgs.Empty);
         }

         public new FrameInfo this[int index]
         {
            set
            {
               base[index] = value;
               OnChanged(EventArgs.Empty);
            }

            get
            {
               return base[index];
            }
         }
      }

   }


}
