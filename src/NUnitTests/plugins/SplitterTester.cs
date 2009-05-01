using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NUnit.Framework;

using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;

using Splitter;

namespace NUnitTests.plugins
{
   [TestFixture]
   public class SplitterTester
   {
      /// <summary>
      /// The file to use as test input.
      /// </summary>
      const String TEST_IMAGE_FILE = @"..\..\testData\testImage.jpg";


      public SplitterTester()
      {
      }

      /// <summary>
      /// Tests settings object
      /// </summary>
      [Ignore, Test]
      public void getSettings()
      {

      }

      /// <summary>
      /// Tests the getFrame/setFrame function. Assume settings are defeult of two way.
      /// </summary>
      [Test]
      public void processFrame()
      {
         // set it up
         Splitter.Splitter plugin = new Splitter.Splitter();

         // create a test frame
         FraMMWorksImageFrame frame = new FraMMWorksImageFrame();
         Bitmap i = new Bitmap(TEST_IMAGE_FILE);
         frame.Image = i;

         // set it up for a 2 way split
         List<Setting> settings = plugin.getSettings();
         settings[0].value = 2;
         settings[0].isDirty = true;

         plugin.sendFrame(frame, 0);

         IFrame retFrame1 = plugin.getFrame(0);
         IFrame retFrame2 = plugin.getFrame(1);

         Assert.IsNotNull((retFrame1 as FraMMWorksImageFrame).Image, "Returned original image was null");
         Assert.IsNotNull((retFrame2 as FraMMWorksImageFrame).Image, "Returned split image was null");

         // Draw on the second image to make sure it is really a copy of the original frame
         FraMMWorksImageFrame f2 = (retFrame2 as FraMMWorksImageFrame);
         Graphics g = Graphics.FromImage(f2.Image);
         g.DrawString("THIS IS THE COPIED IMAGE", new Font(FontFamily.GenericMonospace, 40), Brushes.Red, new PointF(20, 200));
         g.Dispose();


         (retFrame1 as FraMMWorksImageFrame).Image.Save(@"..\..\testData\testImage-SplitterOut-1.jpg");
         f2.Image.Save(@"..\..\testData\testImage-SplitterOut-2.jpg");
      }

   }
}