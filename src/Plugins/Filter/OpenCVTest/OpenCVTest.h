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

   /// NOTE: because of some bug in openCV you have to add MSVCR80.dll to the manifest
   /// for this library.
   /// Add the following line
   /// "type='win32' name='Microsoft.VC80.CRT' version='8.0.50608.0' processorArchitecture='x86' publicKeyToken='1fc8b3b9a1e18e3b'"
   /// to roject -> Properties -> [Debug Configuration] -> Configuration Properties -> Linker -> Manifest File -> Additional Manifest Dependencies:
   ///
   /// SEE: http://sourceforge.net/tracker/index.php?func=detail&aid=1709435&group_id=22870&atid=376677
//#pragma comment(linker,"\"/manifestdependency:type='win32' name='Microsoft.VC80.CRT' version='8.0.50608.0' processorArchitecture='x86' publicKeyToken='1fc8b3b9a1e18e3b'\"")

#include "cv.h"
#include "cxcore.h"
#include "highgui.h"

#using <FraMMWorks.dll>

using namespace System;

using namespace FraMMWorks::PluginBase;
using namespace FraMMWorks::Interfaces;
using namespace FraMMWorks::FrameTypes;


namespace OpenCVTest
{
   /// <remarks>
   /// Simple demonstration plugin showing how to write plugins in managed C++
   /// and interfacing with unmanaged C code (openCV).
   ///
   /// NOTE: There is a very annoying bug within OpenCV which prevnts us from
   /// using their debug libraries. For now I've set both debug and release builds
   /// to use the openCV release libraries.
   ///
   /// The issue is related to manifest files and MSVCR80.dll.
   /// See: http://sourceforge.net/tracker/index.php?func=detail&aid=1709435&group_id=22870&atid=376677
   ///
   /// </remarks>
   public ref class OpenCVTest : SimpleProcessingFilter
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

      //------------------------- private data members--------------------------
      private:

      //------------------------- Constructors ---------------------------------
      public:
      /// <summary>
      /// Default constructor
      /// </summary>
      OpenCVTest();

      ~OpenCVTest();

      !OpenCVTest();

      //------------------------- public access memebers------------------------
      public:
      /// <summary>
      /// Process this frame.
      /// </summary>
      /// <param name="inputFrame">The frame to operate on</param>
      virtual void processFrame(IFrame ^inputFrame) override;

	};
}
