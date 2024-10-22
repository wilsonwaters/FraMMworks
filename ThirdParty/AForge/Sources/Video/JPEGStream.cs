// AForge Video Library
// AForge.NET framework
//
// Copyright � Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video
{
    using System;
	using System.Drawing;
	using System.IO;
	using System.Threading;
	using System.Net;

	/// <summary>
	/// JPEG video source.
    /// </summary>
    /// 
    /// <remarks><para>The video source downloads constantly JPEG files from the specified URL.</para>
    /// <para>Sample usage:</para>
    /// <code>
    /// // create JPEG video source
    /// JPEGStream stream = new JPEGStream( "some url" );
    /// // set NewFrame event handler
    /// stream.NewFrame += new NewFrameEventHandler( video_NewFrame );
    /// // start the video source
    /// stream.Start( );
    /// // ...
    /// // signal to stop
    /// stream.SignalToStop( );
    /// // ...
    /// 
    /// private void video_NewFrame( object sender, NewFrameEventArgs eventArgs )
    /// {
    ///     // get new frame
    ///     Bitmap bitmap = eventArgs.Frame;
    ///     // process the frame
    /// }
    /// </code>
    /// <para><note>Some cameras produce HTTP header, which does conform strictly to
    /// standard, what leads to .NET exception. To avoid this exception the <b>useUnsafeHeaderParsing</b>
    /// configuration option of <b>httpWebRequest</b> should be set, what may be done using application
    /// configuration file.</note></para>
    /// <code>
    /// &lt;configuration&gt;
    /// 	&lt;system.net&gt;
    /// 		&lt;settings&gt;
    /// 			&lt;httpWebRequest useUnsafeHeaderParsing="true" /&gt;
    /// 		&lt;/settings&gt;
    /// 	&lt;/system.net&gt;
    /// &lt;/configuration&gt;
    /// </code>
    /// </remarks>
    /// 
	public class JPEGStream : IVideoSource
	{
        // URL for JPEG files
		private string source;
        // login and password for HTTP authentication
		private string login = null;
		private string password = null;
        // user data associated with the video source
		private object userData = null;
        // received frames count
		private int framesReceived;
        // recieved byte count
		private int bytesReceived;
        // use separate HTTP connection group or use default
		private bool useSeparateConnectionGroup = false;
        // prevent cashing or not
		private bool preventCaching = true;
        // frame interval in milliseconds
		private int frameInterval = 0;
        // timeout value for web request
        private int requestTimeout = 10000;

        // buffer size used to download JPEG image
		private const int bufferSize = 512 * 1024;
        // size of portion to read at once
		private const int readSize = 1024;		

		private Thread thread = null;
		private ManualResetEvent stopEvent = null;

        /// <summary>
        /// New frame event.
        /// </summary>
        /// 
        /// <remarks>Notifies client about new available frame from video source.</remarks>
        /// 
        public event NewFrameEventHandler NewFrame;

        /// <summary>
        /// Video source error event.
        /// </summary>
        /// 
        /// <remarks>The event is used to notify client about any type error occurred in
        /// video source object, for example exceptions.</remarks>
        /// 
        public event VideoSourceErrorEventHandler VideoSourceError;

        /// <summary>
        /// Use or not separate connection group.
        /// </summary>
        /// 
        /// <remarks>The property indicates to open web request in separate connection group.</remarks>
        /// 
		public bool SeparateConnectionGroup
		{
			get { return useSeparateConnectionGroup; }
			set { useSeparateConnectionGroup = value; }
		}

        /// <summary>
        /// Use or not caching.
        /// </summary>
        /// 
        /// <remarks>If the property is set to <b>true</b>, then a fake parameter will be added
        /// to URL to prevent caching. It's required for client, who are behind proxy server.</remarks>
        /// 
		public bool PreventCaching
		{
			get { return preventCaching; }
			set { preventCaching = value; }
		}

        /// <summary>
        /// Frame interval.
        /// </summary>
        /// 
        /// <remarks>The property sets the interval in milliseconds betwen frames. If the property is
        /// set to 100, then the desired frame rate will be 10 frames per second. Default value is 0 -
        /// get new frames as fast as possible.</remarks>
        /// 
		public int FrameInterval
		{
			get { return frameInterval; }
			set { frameInterval = value; }
		}

        /// <summary>
        /// Video source.
        /// </summary>
        /// 
        /// <remarks>URL, which provides JPEG files.</remarks>
        /// 
        public virtual string Source
		{
			get { return source; }
			set { source = value; }
		}

        /// <summary>
        /// Login value.
        /// </summary>
        /// 
        /// <remarks>Login required to access video source.</remarks>
        /// 
		public string Login
		{
			get { return login; }
			set { login = value; }
		}

        /// <summary>
        /// Password value.
        /// </summary>
        /// 
        /// <remarks>Password required to access video source.</remarks>
        /// 
        public string Password
		{
			get { return password; }
			set { password = value; }
		}

        /// <summary>
        /// Received frames count.
        /// </summary>
        /// 
        /// <remarks>Number of frames the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int FramesReceived
		{
			get
			{
				int frames = framesReceived;
				framesReceived = 0;
				return frames;
			}
		}

        /// <summary>
        /// Received bytes count.
        /// </summary>
        /// 
        /// <remarks>Number of bytes the video source provided from the moment of the last
        /// access to the property.
        /// </remarks>
        /// 
        public int BytesReceived
		{
			get
			{
				int bytes = bytesReceived;
				bytesReceived = 0;
				return bytes;
			}
		}

        /// <summary>
        /// User data.
        /// </summary>
        /// 
        /// <remarks>The property allows to associate user data with video source object.</remarks>
        /// 
        public object UserData
		{
			get { return userData; }
			set { userData = value; }
		}

        /// <summary>
        /// Request timeout value.
        /// </summary>
        /// 
        /// <remarks>The property sets timeout value in milliseconds for web requests.
        /// Default value is 10000 milliseconds.</remarks>
        /// 
        public int RequestTimeout
        {
            get { return requestTimeout; }
            set { requestTimeout = value; }
        }

        /// <summary>
        /// State of the video source.
        /// </summary>
        /// 
        /// <remarks>Current state of video source object.</remarks>
        /// 
        public bool IsRunning
		{
			get
			{
				if ( thread != null )
				{
                    // check thread status
					if ( thread.Join( 0 ) == false )
						return true;

					// the thread is not running, free resources
					Free( );
				}
				return false;
			}
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="JPEGStream"/> class.
        /// </summary>
        /// 
        public JPEGStream( ) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPEGStream"/> class.
        /// </summary>
        /// 
        /// <param name="source">URL, which provides JPEG files.</param>
        /// 
        public JPEGStream( string source )
        {
            this.source = source;
        }

        /// <summary>
        /// Start video source.
        /// </summary>
        /// 
        /// <remarks>Start video source and return execution to caller. Video source
        /// object creates background thread and notifies about new frames with the
        /// help of <see cref="NewFrame"/> event.</remarks>
        /// 
        public void Start( )
		{
			if ( thread == null )
			{
                // check source
                if ( ( source == null ) || ( source == string.Empty ) )
                    throw new ArgumentException( "Video source is not specified" );

				framesReceived = 0;
				bytesReceived = 0;

				// create events
				stopEvent = new ManualResetEvent( false );
				
				// create and start new thread
				thread = new Thread( new ThreadStart( WorkerThread ) );
				thread.Name = source; // mainly for debugging
				thread.Start( );
			}
		}

        /// <summary>
        /// Signal video source to stop its work.
        /// </summary>
        /// 
        /// <remarks>Signals video source to stop its background thread, stop to
        /// provide new frames and free resources.</remarks>
        /// 
        public void SignalToStop( )
		{
			// stop thread
			if ( thread != null )
			{
				// signal to stop
				stopEvent.Set( );
			}
		}

        /// <summary>
        /// Wait for video source has stopped.
        /// </summary>
        /// 
        /// <remarks>Waits for source stopping after it was signalled to stop using
        /// <see cref="SignalToStop"/> method.</remarks>
        /// 
        public void WaitForStop( )
		{
			if ( thread != null )
			{
				// wait for thread stop
				thread.Join( );

				Free( );
			}
		}

        /// <summary>
        /// Stop video source.
        /// </summary>
        /// 
        /// <remarks>Stops video source aborting its thread.</remarks>
        /// 
        public void Stop( )
		{
			if ( this.IsRunning )
			{
				thread.Abort( );
				WaitForStop( );
			}
		}

        /// <summary>
        /// Free resource.
        /// </summary>
        /// 
		private void Free( )
		{
			thread = null;

			// release events
			stopEvent.Close( );
			stopEvent = null;
		}

        /// <summary>
        /// Worker thread.
        /// </summary>
        /// 
		private void WorkerThread( )
		{
            // buffer to read stream
			byte[] buffer = new byte[bufferSize];
            // HTTP web request
			HttpWebRequest request = null;
            // web responce
			WebResponse response = null;
            // stream for JPEG downloading
			Stream stream = null;
            // random generator to add fake parameter for cache preventing
			Random rand = new Random( (int) DateTime.Now.Ticks );
            // download start time and duration
			DateTime start;
			TimeSpan span;

			while ( true )
			{
				int	read, total = 0;

				try
				{
                    // set dowbload start time
					start = DateTime.Now;

					// create request
					if ( !preventCaching )
					{
                        // request without cache prevention
                        request = (HttpWebRequest) WebRequest.Create( source );
					}
					else
					{
                        // request with cache prevention
                        request = (HttpWebRequest) WebRequest.Create( source + ( ( source.IndexOf( '?' ) == -1 ) ? '?' : '&' ) + "fake=" + rand.Next( ).ToString( ) );
					}
                    // set timeout value for the request
                    request.Timeout = requestTimeout;
					// set login and password
					if ( ( login != null ) && ( password != null ) && ( login != string.Empty ) )
                        request.Credentials = new NetworkCredential( login, password );
					// set connection group name
					if ( useSeparateConnectionGroup )
                        request.ConnectionGroupName = GetHashCode( ).ToString( );
					// get response
                    response = request.GetResponse( );
					// get response stream
                    stream = response.GetResponseStream( );

					// loop
					while ( !stopEvent.WaitOne( 0, true ) )
					{
						// check total read
						if ( total > bufferSize - readSize )
						{
							total = 0;
						}

						// read next portion from stream
						if ( ( read = stream.Read( buffer, total, readSize ) ) == 0 )
							break;

						total += read;

						// increment received bytes counter
						bytesReceived += read;
					}

					if ( !stopEvent.WaitOne( 0, true ) )
					{
						// increment frames counter
						framesReceived++;

						// provide new image to clients
						if ( NewFrame != null )
						{
							Bitmap bitmap = (Bitmap) Bitmap.FromStream( new MemoryStream( buffer, 0, total ) );
							// notify client
                            NewFrame( this, new NewFrameEventArgs( bitmap ) );
							// release the image
                            bitmap.Dispose( );
                            bitmap = null;
						}
					}

					// wait for a while ?
					if ( frameInterval > 0 )
					{
						// get download duration
						span = DateTime.Now.Subtract( start );
						// miliseconds to sleep
						int msec = frameInterval - (int) span.TotalMilliseconds;

						while ( ( msec > 0 ) && ( stopEvent.WaitOne( 0, true ) == false ) )
						{
							// sleeping ...
							Thread.Sleep( ( msec < 100 ) ? msec : 100 );
							msec -= 100;
						}
					}
				}
				catch ( WebException exception )
				{
                    // provide information to clients
                    if ( VideoSourceError != null )
                    {
                        VideoSourceError( this, new VideoSourceErrorEventArgs( exception.Message ) );
                    }
					// wait for a while before the next try
					Thread.Sleep( 250 );
				}
				catch ( Exception )
				{
				}
				finally
				{
					// abort request
					if ( request != null)
					{
                        request.Abort( );
                        request = null;
					}
					// close response stream
					if ( stream != null )
					{
						stream.Close( );
						stream = null;
					}
					// close response
					if ( response != null )
					{
                        response.Close( );
                        response = null;
					}
				}

				// need to stop ?
				if ( stopEvent.WaitOne( 0, true ) )
					break;
			}
		}
	}
}
