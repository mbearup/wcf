// Decompiled with JetBrains decompiler
// Type: System.UriTemplateMatchException
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System
{
  /// <summary>Represents an error when matching a <see cref="T:System.Uri" /> to a <see cref="T:System.UriTemplateTable" />.</summary>
  [TypeForwardedFrom("System.ServiceModel.Web, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")]
  [Serializable]
  public class UriTemplateMatchException : SystemException
  {
    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateMatchException" /> class.</summary>
    public UriTemplateMatchException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateMatchException" /> class with the specified error message.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public UriTemplateMatchException(string message)
      : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateMatchException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. </param>
    public UriTemplateMatchException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.UriTemplateMatchException" /> class with serialized data.</summary>
    /// <param name="info">The object that holds the serialized data object.</param>
    /// <param name="context">The contextual information about the source or destination.</param>
    protected UriTemplateMatchException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
