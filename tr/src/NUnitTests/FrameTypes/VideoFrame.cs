using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using FraMMWorks.FrameTypes;

using NUnit.Framework;

namespace NUnitTests.FrameTypes
{
   [TestFixture]
   public class VideoFrame
   {
      const String TEST_IMAGE = @"..\..\testData\testImage.jpg";
      const int TEST_IMAGE_WIDTH = 1222;
      const int TEST_IMAGE_HEIGHT = 878;
      public VideoFrame()
      {
      }

      [Ignore, Test]
      public void getRawData()
      {
         FraMMWorks.FrameTypes.FraMMWorksImageFrame frame = new FraMMWorks.FrameTypes.FraMMWorksImageFrame();
         Bitmap testImage = new Bitmap(TEST_IMAGE);
         frame.Image = testImage;

         byte [] rawData = frame.getRawData();
         Assert.IsNotNull(rawData, "Raw image data was null");

         // check if every pixel is the same
         for (int x = 0; x < TEST_IMAGE_WIDTH; x++)
         {
            for (int y = 0; y < TEST_IMAGE_HEIGHT; y++)
            {
               Color origPix = testImage.GetPixel(x, y);
               Color rawPix = Color.FromArgb(rawData[(x * y) + (x * 3)], rawData[(x * y) + (x * 3) + 1], rawData[(x * y) + (x * 3) + 2]);
               Console.Out.WriteLine("format="+testImage.PixelFormat+"OrigPix=" + origPix + " rawPix=" + rawPix);
               Assert.That(origPix.Equals(rawPix), "Found a different pixel when comparing original image with raw data "+x+" " +y);
            }
         }

      }

      [Test]
      public void Height()
      {
         FraMMWorks.FrameTypes.FraMMWorksImageFrame frame = new FraMMWorks.FrameTypes.FraMMWorksImageFrame();
         Bitmap testImage = new Bitmap(TEST_IMAGE);
         frame.Image = testImage;

         Assert.That(frame.Height == TEST_IMAGE_HEIGHT, "VideoFrame height was not as expected");
         Assert.That(frame.Width == TEST_IMAGE_WIDTH, "VideoFrame width was not as expected");
      }

      [Test]
      public void Width()
      {
         FraMMWorks.FrameTypes.FraMMWorksImageFrame frame = new FraMMWorks.FrameTypes.FraMMWorksImageFrame();
         Bitmap testImage = new Bitmap(TEST_IMAGE);
         frame.Image = testImage;

         Assert.That(frame.Width == TEST_IMAGE_WIDTH, "VideoFrame width was not as expected");
      }
   }
}


