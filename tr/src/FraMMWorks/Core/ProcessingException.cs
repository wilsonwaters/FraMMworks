using System;
using System.Collections.Generic;
using System.Text;

namespace FraMMWorks.Core
{
   /// <remarks>
   /// An exception which indicates there was some problem within a plugin.
   /// The Exception may be marked "recoverable", in which case processing will
   /// continue with the next frame.
   /// </remarks>
   public class ProcessingException : Exception
   {
      private bool recoverable;

      /// <summary>
      /// Whether this exception may be recovered from (by recalling the same method again)
      /// </summary>
      public bool Recoverable
      {
         get { return recoverable; }
      }

      public ProcessingException()
         : base()
      {
         this.recoverable = false;
      }

      public ProcessingException(String message, bool recoverable)
         :base(message)
      {
         this.recoverable = recoverable;
      }

      public ProcessingException(String message)
         : base(message)
      {
         this.recoverable = false;
      }

      public ProcessingException(String message, Exception innerException)
         : base(message, innerException)
      {
         this.recoverable = false;
      }

      public ProcessingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
         : base(info, context)
      {
         this.recoverable = false;
      }
   }
}
