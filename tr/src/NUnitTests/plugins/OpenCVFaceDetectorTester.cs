using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using NUnit.Framework;

using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;
using FraMMWorks.Core;

using OpenCVFaceDetector;

namespace NUnitTests.plugins
{
   [TestFixture]
   public class OpenCVFaceDetectorTester
   {
      /// <summary>
      /// The file to use as test input.
      /// </summary>
      const String TEST_IMAGE_FILE = @"..\..\testData\testImage-OpenCVFaceDetectorIn.jpg";

      public OpenCVFaceDetectorTester()
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
      /// Tests the processFrame function.
      /// </summary>
      [Test]
      public void processFrame()
      {
         // set it up
         OpenCVFaceDetector.OpenCVFaceDetector plugin = new OpenCVFaceDetector.OpenCVFaceDetector();

         // create a test frame
         FraMMWorksImageFrame frame = new FraMMWorksImageFrame();
         Bitmap i = new Bitmap(TEST_IMAGE_FILE);
         frame.Image = i;

         plugin.sendFrame(frame, 0);
         IFrame retFrame = plugin.getFrame(0);

         Assert.That(frame == retFrame, "return frame itself was differnet (we only want to modify the internal data)");
         Assert.IsNotNull((retFrame as FraMMWorksImageFrame).Image, "Returned image was null");

         frame.Image.Save(@"..\..\testData\testImage-OpenCVFaceDetectorOut.jpg");
      }



   }
}