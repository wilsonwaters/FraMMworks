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

    /// <summary>
    /// Base class for filtering grayscale images without changing pixel format, but
    /// using image copy in the background. The base class allows to filter as entire image,
    /// as only specified rectangle.
    /// </summary>
    /// 
    /// <remarks><para>The abstract class is the base class for all filters, which can
    /// be applied to grayscale images without changing their pixel format
    /// and image dimension. The base class is used for filters, which can not do
    /// direct manipulations with source image. To make effect of in-place filtering,
    /// these filters create a background copy of the original image (done by this
    /// base class) and then do manipulations with it putting result back to the original
    /// source image.</para>
    /// <para>The base class is for those filters, which support partial filtering on specified
    /// rectangle only.</para>
    /// </remarks>
    /// 
    public abstract class FilterGrayToGrayUsingCopyPartial : IFilter, IInPlaceFilter, IInPlacePartialFilter
    {
        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        /// <remarks>The method keeps the source image unchanged and returns the
        /// the result of image processing filter as new image.</remarks> 
        ///
        public Bitmap Apply( Bitmap image )
        {
            // lock source bitmap data
            BitmapData srcData = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed );

            // apply the filter
            Bitmap dstImage = Apply( srcData );

            // unlock source image
            image.UnlockBits( srcData );

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Source image to apply filter to.</param>
        /// 
        /// <returns>Returns filter's result obtained by applying the filter to
        /// the source image.</returns>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        /// <remarks>The filter accepts bitmap data as input and returns the result
        /// of image processing filter as new image. The source image data are kept
        /// unchanged.</remarks>
        /// 
        public Bitmap Apply( BitmapData imageData )
        {
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new ArgumentException( "The filter can be applied to graysclae (8bpp indexed) image only" );

            // get image dimension
            int width = imageData.Width;
            int height = imageData.Height;

            // create new image
            Bitmap dstImage = AForge.Imaging.Image.CreateGrayscaleImage( width, height );

            // lock destination bitmap data
            BitmapData dstData = dstImage.LockBits(
                new Rectangle( 0, 0, width, height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            // process the filter
            ProcessFilter( imageData.Scan0, dstData, new Rectangle( 0, 0, width, height ) );

            // unlock destination images
            dstImage.UnlockBits( dstData );

            return dstImage;
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image.</remarks>
        /// 
        public void ApplyInPlace( Bitmap image )
        {
            // apply the filter
            ApplyInPlace( image, new Rectangle( 0, 0, image.Width, image.Height ) );
        }

        /// <summary>
        /// Apply filter to an image.
        /// </summary>
        /// 
        /// <param name="imageData">Image to apply filter to.</param>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image data.</remarks>
        /// 
        public void ApplyInPlace( BitmapData imageData )
        {
            // apply the filter
            ApplyInPlace( imageData, new Rectangle( 0, 0, imageData.Width, imageData.Height ) );
        }

        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="image">Image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image.</remarks>
        /// 
        public void ApplyInPlace( Bitmap image, Rectangle rect )
        {
            // lock source bitmap data
            BitmapData data = image.LockBits(
                new Rectangle( 0, 0, image.Width, image.Height ),
                ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed );

            // process the filter
            ApplyInPlace( data, rect );

            // unlock image
            image.UnlockBits( data );
        }

        /// <summary>
        /// Apply filter to an image or its part.
        /// </summary>
        /// 
        /// <param name="imageData">Image to apply filter to.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        /// <exception cref="ArgumentException">The source image has incorrect pixel format.</exception>
        /// 
        /// <remarks>The method applies the filter directly to the provided
        /// image data.</remarks>
        /// 
        public void ApplyInPlace( BitmapData imageData, Rectangle rect )
        {
            if ( imageData.PixelFormat != PixelFormat.Format8bppIndexed )
                throw new ArgumentException( "The filter can be applied to graysclae (8bpp indexed) image only" );

            // validate rectangle
            rect.Intersect( new Rectangle( 0, 0, imageData.Width, imageData.Height ) );

            // process the filter if rectangle is not empty
            if ( ( rect.Width | rect.Height ) != 0 )
            {
                // create a copy of the source image
                int size = imageData.Stride * imageData.Height;

                IntPtr sourceData = MemoryManager.Alloc( size );
                AForge.Win32.memcpy( sourceData, imageData.Scan0, size );

                // process the filter
                ProcessFilter( sourceData, imageData, rect );

                MemoryManager.Free( sourceData );
            }
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Pointer to source image data (first scan line).</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <param name="rect">Image rectangle for processing by the filter.</param>
        /// 
        protected abstract unsafe void ProcessFilter( IntPtr sourceData, BitmapData destinationData, Rectangle rect );
    }
}
