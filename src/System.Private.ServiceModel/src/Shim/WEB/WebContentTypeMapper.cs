// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.WebContentTypeMapper
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

namespace System.ServiceModel.Channels
{
  /// <summary>Specifies the format to which the content type of an incoming message is mapped.</summary>
  public abstract class WebContentTypeMapper
  {
    /// <summary>When overridden in a derived class, returns the message format used for a specified content type.</summary>
    /// <param name="contentType">The content type that indicates the MIME type of data to be interpreted.</param>
    /// <returns>The <see cref="T:System.ServiceModel.Channels.WebContentFormat" /> that specifies the format to which the message content type is mapped. </returns>
    public abstract WebContentFormat GetMessageFormatForContentType(string contentType);
  }
}
