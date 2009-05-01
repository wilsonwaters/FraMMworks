/*
    FraMMWorks - Multimedia Processing and Diagnostic Framework.
    Copyright (C) 2008  Wilson Waters

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#pragma once

#include "cv.h"
#include "cxcore.h"
#include "highgui.h"

#define DEFAULT_HARR_CASCADE_FILE "S:\\FraMMWorks\\bin\\Debug\\haarcascade_frontalface_alt.xml"
//#define DEFAULT_HARR_CASCADE_FILE ".\\haarcascade_frontalface_alt.xml"
//#define DEFAULT_HARR_CASCADE_FILE "S:\\FraMMWorks\\bin\\Debug\\RightEyeHARRCascade.xml"

#using <FraMMWorks.dll>

using namespace System;
using namespace System::Drawing;
using namespace System::Collections::Generic;

using namespace FraMMWorks::PluginBase;
using namespace FraMMWorks::Interfaces;
using namespace FraMMWorks::FrameTypes;
using namespace FraMMWorks::Core;


namespace OpenCVFaceDetector
{
   /// <remarks>
   /// Detect faces in a video stream and place a rectangle around them.
   /// Uses the OpenCV library to perform the actual face detection.
   ///
   /// </remarks>
   public ref class OpenCVFaceDetector : SimpleProcessingFilter
	{
      //------------------------- public properties ---------------------------
      public:
      /// <summary>
      /// What type of output does this class handle
      /// </summary>
      property Type^ Capability 
      {
         virtual Type^ get() override { return FraMMWorksImageFrame::typeid ; }
      }

      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      property String^ Name
      {
         virtual String^ get() override { return name; }
      }

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      property String^ Description
      {
         virtual String^ get() override { return description; }
      }

      //------------------------- private data members--------------------------
      private:
      /// <summary>
      /// A short descripive name for this plugin
      /// </summary>
      static String^ name = "OpenCV Face Detector";

      /// <summary>
      /// A verbose description for this plugin.
      /// </summary>
      static String^ description = "Detect faces in an image and place a rectangle around them. This is "+
                            "actually a generic HARR classifier and can be usef for rectangular objects "+
                            "other than faces. Specify a different classifier file to detect other objects. "+
                            "See the OpenCV project for more info - http://opencvlibrary.sourceforge.net/";

      /// <summary>
      /// The settings list for this plugin.
      /// </summary>
      List<Setting^>^ settings;

      // The Haar classifier for face detection
      CvHaarClassifierCascade *m_pCascade;

      // openCV memory storage
      CvMemStorage *m_pStorage;

      // The current HARR classifier file
      char *m_pszCurrentClassifierFile;

      //------------------------- Constructors ---------------------------------
      public:
      /// <summary>
      /// Default constructor
      /// </summary>
      OpenCVFaceDetector();

      ~OpenCVFaceDetector();

      !OpenCVFaceDetector();

      //------------------------- public access memebers------------------------
      public:
      /// <summary>
      /// Process this frame.
      /// </summary>
      /// <param name="inputFrame">The frame to operate on</param>
      virtual void processFrame(IFrame ^inputFrame) override;

      /// <summary>
      /// The setings for this plugin
      /// setting 0: String   - filename of the HARR classifier file
      /// </summary>
      /// <returns>The <see cref="Setting"/>s for this plugin
      /// </returns>
      virtual List<Setting^>^ getSettings() override
      {
         return settings;
      }

      //virtual System::Windows::Forms::Control ^getDisplayControl() override;


      //------------------------- private access memebers------------------------
      private:
      // Performs the actual object detection on a openCV image
      void detect_and_draw( IplImage* img );

      // Set up the settings object
      void initSettings();



	};
}
