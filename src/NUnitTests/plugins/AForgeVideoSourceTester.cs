using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using FraMMWorks.Interfaces;
using FraMMWorks.PluginBase;
using FraMMWorks.FrameTypes;

using AForgeVideoSource;

namespace NUnitTests.plugins
{
   [TestFixture]
   public class AForgeVideoSourceTester
   {
      /// <summary>
      /// The file to use as test input.
      /// </summary>
      const String TEST_AVI_FILE = @"..\..\testData\testVideo-MJPEG.avi";


      public AForgeVideoSourceTester()
      {
      }

      /// <summary>
      /// Tests settings object
      /// </summary>
      [Test]
      public void SettingsTest()
      {
         // set it up
         AForgeVideoSource.AForgeVideoSource plugin = new AForgeVideoSource.AForgeVideoSource();

         List<Setting> settings = plugin.getSettings();
         Assert.IsNotNull(settings);

         // Check types are as expected
         Assert.IsTrue(settings[0].type == typeof(String), "setting 0 wasn't a String as expected");
         Assert.IsTrue(settings[1].type == typeof(AForgeVideoSource.SourceSelectionSetting), "Setting 1 wasn't a SourceSelectionSetting as expected");

         Assert.IsTrue(settings[0].value.GetType() == typeof(String), "The type of the value of setting 0 was differnet to the type field");
         Assert.IsTrue(settings[1].value.GetType() == typeof(String), "The type of the value of setting 1 was differnet to the type field");

         // Try setting each setting to something and make sure it returns what we expect
         settings[0].value = TEST_AVI_FILE;
         Console.Out.WriteLine("setting 0 before " + (settings[0].value));

         settings[1].value = AForgeVideoSource.SourceSelectionSetting.DefaultItems[0];
         Console.Out.WriteLine("setting 1 before " + (settings[1].value));

         List<Setting> returnedSettings = plugin.getSettings();
         Console.Out.WriteLine("setting 0 after " + ((String)returnedSettings[0].value));
         Assert.IsTrue(((String)returnedSettings[0].value).Equals(TEST_AVI_FILE), "Test file was different than what we entered");

         Console.Out.WriteLine("setting 1 after " + ((String)returnedSettings[1].value));
         Assert.IsTrue(((String)returnedSettings[1].value).Equals(AForgeVideoSource.SourceSelectionSetting.DefaultItems[0]), "input type selected item was different than what we entered");

      }

      /// <summary>
      /// Tests the getFrame function.
      /// </summary>
      [Test]
      public void GetFrameTest()
      {
         // set it up
         AForgeVideoSource.AForgeVideoSource plugin = new AForgeVideoSource.AForgeVideoSource();

         List<Setting> settings = plugin.getSettings();

         settings[0].value = TEST_AVI_FILE;
         settings[1].value = AForgeVideoSource.SourceSelectionSetting.DefaultItems[0];

         plugin.start();
         IFrame retFrame = plugin.getFrame(0);

         Assert.IsNotNull(retFrame, "returned frame was null");
         (retFrame as FraMMWorksImageFrame).Image.Save(@"..\..\testData\testImage-AForgeVideoSourceOut.jpg");
      }

      /// <summary>
      /// Tests the getFrame function.
      /// </summary>
      [Test]
      public void StartStopTest()
      {
         // set it up
      }
   }
}
