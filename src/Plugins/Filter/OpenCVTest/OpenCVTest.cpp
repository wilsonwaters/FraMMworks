// This is the main DLL file.

#include "stdafx.h"

#include "OpenCVTest.h"



namespace OpenCVTest
{


OpenCVTest::OpenCVTest()
{
}

OpenCVTest::~OpenCVTest() // implements/overrides the IDisposable::Dispose method
{
  // free managed and unmanaged resources
}

OpenCVTest::!OpenCVTest() // implements/overrides the Object::Finalize method
{
  // free unmanaged resources only
}

	static System::Drawing::Bitmap ^IplImage2Bitmap(IplImage* _iplimage){
      System::Drawing::Bitmap ^_bitmap= gcnew System::Drawing::Bitmap(_iplimage->width,_iplimage->height, System::Drawing::Imaging::PixelFormat::Format24bppRgb);
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


void OpenCVTest::processFrame(IFrame ^inputFrame)
{

   FraMMWorksImageFrame ^frame = dynamic_cast<FraMMWorksImageFrame^>(inputFrame);
   IplImage* temp_image=cvCreateImage(cvSize(frame->Width,frame->Height),8,3);
		cvCircle(temp_image,cvPoint(100,75),70,cvScalar(0,0,255));
		cvCircle(temp_image,cvPoint(75,45),10,cvScalar(0,0,255));
		cvCircle(temp_image,cvPoint(125,45),10,cvScalar(0,0,255));
		cvCircle(temp_image,cvPoint(100,75),5,cvScalar(0,0,255));
		cvLine(temp_image,cvPoint(80,110),cvPoint(120,110),cvScalar(0,0,255));	
		cvLine(temp_image,cvPoint(80,110),cvPoint(65,95),cvScalar(0,0,255));	
		cvLine(temp_image,cvPoint(135,95),cvPoint(120,110),cvScalar(0,0,255));	
		cvSaveImage("ok.bmp",temp_image);
		System::Drawing::Bitmap ^return_value=IplImage2Bitmap(temp_image);
		cvReleaseImage(&temp_image);

      // just replace the whole image for now
      frame->Image = return_value;
}




}