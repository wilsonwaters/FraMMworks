// AForge Image Processing Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2005-2007
// andrew.kirillov@gmail.com
//
namespace AForge.Imaging.Filters
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using AForge;

    /// <summary>
    /// Base class for error diffusion dithering.
    /// </summary>
    /// 
    /// <remarks><para>The class is the base class for binarization algorithms based on error diffusion.</para>
    /// <para>Binarization with error diffusion in its idea is similar with binarization based on thresholding
    /// of pixels' cumulative value (see <see cref="ThresholdWithCarry"/>). Each pixel is binarized based not only
    /// on its own value, but on values of some surrounding pixels. During pixel's binarization, its <b>binarization
    /// error</b> is distributed (diffused) to some neighbor pixels with some coefficients. This error diffusion
    /// updates neighbor pixels changing their values, what affects their upcoming binarization. Error diffuses
    /// only on unprocessed yet neighbor pixels, which are right and bottom pixels usually (in the case if image
    /// processing is done from upper left corner to bottom right corner). <b>Binarization error</b> equals
    /// to processing pixel value, if it is below threshold value, or pixel value minus 255 otherwise.</para>
    /// </remarks>
    /// 
    public abstract class ErrorDiffusionDithering : FilterGrayToGrayPartial
    {
        private byte threshold = 128;

        /// <summary>
        /// Threshold value.
        /// </summary>
        /// 
        /// <remarks>Default value is 128.</remarks>
        /// 
        public byte ThresholdValue
        {
            get { return threshold; }
            set { threshold = value; }
        }

        /// <summary>
        /// Current processing X coordinate.
        /// </summary>
        protected int x;

        /// <summary>
        /// Current processing Y coordinate.
        /// </summary>
        protected int y;

        /// <summary>
        /// Processing X start position.
        /// </summary>
        protected int startX;

        /// <summary>
        /// Processing Y start position.
        /// </summary>
        protected int startY;

        /// <summary>
        /// Processing X stop position.
        /// </summary>
        protected int stopX;

        /// <summary>
        /// Processing Y stop position.
        /// </summary>
        protected int stopY;

        /// <summary>
        /// Processing image's stride (line size).
        /// </summary>
        protected int stride;

        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// 
        /// <param name="error">Current error value.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        /// 
        /// <remarks>All parameters of the image and current processing pixel's coordinates
        /// are initialized in protected members.</remarks>
        /// 
        protected abstract unsafe void Diffuse( int error, byte* ptr );

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        ///
        /// <param name="imageData">Image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected override unsafe void ProcessFilter( BitmapData imageData, Rectangle rect )
        {
            // processing start and stop X,Y positions
            startX  = rect.Left;
            startY  = rect.Top;
            stopX   = startX + rect.Width;
            stopY   = startY + rect.Height;
            stride  = imageData.Stride;

            int offset = stride - rect.Width;

            // pixel value and error value
            int v, error;

            // do the job
            byte* ptr = (byte*) imageData.Scan0.ToPointer( );

            // allign pointer to the first pixel to process
            ptr += ( startY * stride + startX );

            // for each line
            for ( y = startY; y < stopY; y++ )
            {
                // for each pixels
                for ( x = startX; x < stopX; x++, ptr++ )
                {
                    v = *ptr;

                    // fill the next destination pixel
                    if ( v >= threshold )
                    {
                        *ptr = 255;
                        error = v - 255;
                    }
                    else
                    {
                        *ptr = 0;
                        error = v;
                    }

                    // do error diffusion
                    Diffuse( error, ptr );
                }
                ptr += offset;
            }
        }
    }
}
