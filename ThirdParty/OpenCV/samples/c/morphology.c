#include <cv.h>
#include <highgui.h>
#include <stdlib.h>
#include <stdio.h>

IplImage* src = 0;
IplImage* dst = 0;

IplConvKernel* element = 0;
int element_shape = CV_SHAPE_RECT;

//the address of variable which receives trackbar position update 
int max_iters = 10;
int open_close_pos = 0;
int erode_dilate_pos = 0;

// callback function for open/close trackbar
void OpenClose(int pos)   
{
    int n = open_close_pos - max_iters;
    int an = n > 0 ? n : -n;
    element = cvCreateStructuringElementEx( an*2+1, an*2+1, an, an, element_shape, 0 );
    if( n < 0 )
    {
        cvErode(src,dst,element,1);
        cvDilate(dst,dst,element,1);
    }
    else
    {
        cvDilate(src,dst,element,1);
        cvErode(dst,dst,element,1);
    }
    cvReleaseStructuringElement(&element);
    cvShowImage("Open/Close",dst);
}   

// callback function for erode/dilate trackbar
void ErodeDilate(int pos)   
{
    int n = erode_dilate_pos - max_iters;
    int an = n > 0 ? n : -n;
    element = cvCreateStructuringElementEx( an*2+1, an*2+1, an, an, element_shape, 0 );
    if( n < 0 )
    {
        cvErode(src,dst,element,1);
    }
    else
    {
        cvDilate(src,dst,element,1);
    }
    cvReleaseStructuringElement(&element);
    cvShowImage("Erode/Dilate",dst);
}   


int main( int argc, char** argv )
{
    char* filename = argc == 2 ? argv[1] : (char*)"baboon.jpg";
    if( (src = cvLoadImage(filename,1)) == 0 )
        return -1;

    printf( "Hot keys: \n"
        "\tESC - quit the program\n"
        "\tr - use rectangle structuring element\n"
        "\te - use elliptic structuring element\n"
        "\tc - use cross-shaped structuring element\n"
        "\tENTER - loop through all the options\n" );

    dst = cvCloneImage(src);

    //create windows for output images
    cvNamedWindow("Open/Close",1);
    cvNamedWindow("Erode/Dilate",1);

    open_close_pos = erode_dilate_pos = max_iters;
    cvCreateTrackbar("iterations", "Open/Close",&open_close_pos,max_iters*2+1,OpenClose);
    cvCreateTrackbar("iterations", "Erode/Dilate",&erode_dilate_pos,max_iters*2+1,ErodeDilate);

    for(;;)
    {
        int c;
        
        OpenClose(open_close_pos);
        ErodeDilate(erode_dilate_pos);
        c = cvWaitKey(0);

        if( c == 27 )
            break;
        if( c == 'e' )
            element_shape = CV_SHAPE_ELLIPSE;
        else if( c == 'r' )
            element_shape = CV_SHAPE_RECT;
        else if( c == 'c' )
            element_shape = CV_SHAPE_CROSS;
        else if( c == '\r' )
            element_shape = (element_shape + 1) % 3;
    }

    //release images
    cvReleaseImage(&src);
    cvReleaseImage(&dst);

    //destroy windows 
    cvDestroyWindow("Open/Close"); 
    cvDestroyWindow("Erode/Dilate"); 

    return 0;
}
