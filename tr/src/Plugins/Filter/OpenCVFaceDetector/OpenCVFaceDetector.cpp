// This is the main DLL file.

#include "stdafx.h"

#include "OpenCVFaceDetector.h"


namespace OpenCVFaceDetector
{


OpenCVFaceDetector::OpenCVFaceDetector()
{
   initSettings();
   m_pszCurrentClassifierFile = (char *)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi((String^)settings[0]->value).ToPointer();
   m_pCascade = (CvHaarClassifierCascade*)cvLoad( m_pszCurrentClassifierFile, 0, 0, 0 );
   m_pStorage = cvCreateMemStorage(0);
}

OpenCVFaceDetector::~OpenCVFaceDetector() // implements/overrides the IDisposable::Dispose method
{
  // free managed and unmanaged resources
}

OpenCVFaceDetector::!OpenCVFaceDetector() // implements/overrides the Object::Finalize method
{
     //cvReleaseHaarClassifierCascade( &m_pCascade );
     //cvReleaseMemStorage( &m_pStorage );
}

	static System::Drawing::Bitmap ^IplImage2Bitmap(IplImage* _iplimage)
   {
		System::Drawing::Bitmap ^_bitmap= gcnew System::Drawing::Bitmap(_iplimage->width,_iplimage->height);
		System::Drawing::Imaging::BitmapData ^bdata;
		bdata =_bitmap->LockBits(
			System::Drawing::Rectangle(0,0,_iplimage->width,_iplimage->height),
			System::Drawing::Imaging::ImageLockMode::WriteOnly,
			System::Drawing::Imaging::PixelFormat::Format24bppRgb
		);
		uchar* pSourcePixel = (uchar*)bdata->Scan0.ToPointer();
		uchar* pixel = (uchar*)(_iplimage->imageData);
		memcpy(pSourcePixel,pixel,_iplimage->width*_iplimage->height*3);
		_bitmap->UnlockBits(bdata);
		return _bitmap;
	}


void OpenCVFaceDetector::processFrame(IFrame ^inputFrame)
{
   // first, make sure the inpu classifier file setting haven't changed
   if((bool)settings[0]->isDirty)
   {
      // reload the new harr classifier file
      m_pszCurrentClassifierFile = (char *)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi((String^)settings[0]->value).ToPointer();
      settings[0]->isDirty = false;
      m_pCascade = (CvHaarClassifierCascade*)cvLoad( m_pszCurrentClassifierFile, 0, 0, 0 );
   }

   FraMMWorksImageFrame ^frame = dynamic_cast<FraMMWorksImageFrame^>(inputFrame);
   IplImage* temp_image = cvCreateImage(cvSize(frame->Width,frame->Height),8,3);

   // don't copy the image data, just let openCV know where it is. It can
   // work directly on 8bit RGB images.
   try
   {
      IntPtr ptr = frame->LockBitmap();
      temp_image->imageData = (char*)ptr.ToPointer();

      // pass it to the face detection algorithm
      detect_and_draw(temp_image);

      // unlock the image data. Any faces should have been drawn on the image data
      frame->UnlockBitmap();
   }
   catch (System::Exception^ e)
   {
      // we want to make sure the frame is unlocked before
      // the exception is passed up
      throw e;
   }
   finally
   {
      try
      {
         frame->UnlockBitmap();
      }
      catch(...) {}
      cvReleaseImage(&temp_image);
   }
}

// Function to detect and draw any faces that is present in an image
// taken from http://opencvlibrary.sourceforge.net/FaceDetection
void OpenCVFaceDetector::detect_and_draw( IplImage* img )
{
    int scale = 1;

    // Create two points to represent the face locations
    CvPoint pt1, pt2;

    // Clear the memory storage which was used before
    cvClearMemStorage( m_pStorage );

    // Find whether the cascade is loaded, to find the faces. If yes, then:
    if( m_pCascade )
    {

        // There can be more than one face in an image. So create a growable sequence of faces.
        // Detect the objects and store them in the sequence
        CvSeq* faces = cvHaarDetectObjects( img, m_pCascade, m_pStorage,
                                            1.1, 2, CV_HAAR_DO_CANNY_PRUNING,
                                            cvSize(40, 40) );

        // Loop the number of faces found.
        for(int i = 0; i < (faces ? faces->total : 0); i++ )
        {
           // Create a new rectangle for drawing the face
            CvRect* r = (CvRect*)cvGetSeqElem( faces, i );

            // Find the dimensions of the face,and scale it if necessary
            pt1.x = r->x*scale;
            pt2.x = (r->x+r->width)*scale;
            pt1.y = r->y*scale;
            pt2.y = (r->y+r->height)*scale;

            // Draw the rectangle in the input image
            cvRectangle( img, pt1, pt2, CV_RGB(255,0,0), 3, 8, 0 );
        }
    }
}


void OpenCVFaceDetector::initSettings()
{
   settings = gcnew List<Setting^>();

   // setting 0: filename of the HARR classifier XML file
   Setting^ setting = gcnew Setting();
   setting->name = "HARR classifier";   // The display name of this setting
   setting->minValue = 0;                  // Min length of field
   setting->maxValue = 1024;               // Max length of field 
   setting->type = String::typeid;         // It's a string setting
   setting->value = DEFAULT_HARR_CASCADE_FILE;  // default value
   settings->Add(setting);

}




}