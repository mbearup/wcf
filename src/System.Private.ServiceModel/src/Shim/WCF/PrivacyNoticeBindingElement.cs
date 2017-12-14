// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.PrivacyNoticeBindingElement
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.ServiceModel.Description;
using System.Xml;

namespace System.ServiceModel.Channels
{
  /// <summary>Represents the binding element that contains the privacy policy for the WS-Federation binding.</summary>
  public sealed class PrivacyNoticeBindingElement : BindingElement, IPolicyExportExtension
  {
    private Uri url;
    private int version;

    /// <summary>Gets or sets the URI at which the privacy notice is located.</summary>
    /// <returns>The <see cref="T:System.Uri" /> at which the privacy notice is located.</returns>
    public Uri Url
    {
      get
      {
        return this.url;
      }
      set
      {
        if (value == (Uri) null)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
        this.url = value;
      }
    }

    /// <summary>Gets or sets the privacy notice version number for the binding.</summary>
    /// <returns>The version number of the privacy notice.</returns>
    public int Version
    {
      get
      {
        return this.version;
      }
      set
      {
        if (value < 0)
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value", (object) value, SR.GetString("ValueMustBePositive")));
        this.version = value;
      }
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.PrivacyNoticeBindingElement" /> class.</summary>
    public PrivacyNoticeBindingElement()
    {
      this.url = (Uri) null;
    }

    /// <summary>Initializes a new instance of the <see cref="T:System.ServiceModel.Channels.PrivacyNoticeBindingElement" /> class from an existing element.</summary>
    /// <param name="elementToBeCloned">The <see cref="T:System.ServiceModel.Channels.PrivacyNoticeBindingElement" /> used to initialize the new element.</param>
    public PrivacyNoticeBindingElement(PrivacyNoticeBindingElement elementToBeCloned)
      : base((BindingElement) elementToBeCloned)
    {
      this.url = elementToBeCloned.url;
      this.version = elementToBeCloned.version;
    }

    /// <summary>Creates a copy of the current binding element.</summary>
    /// <returns>A <see cref="T:System.ServiceModel.Channels.BindingElement" /> that is a copy of the current element.</returns>
    public override BindingElement Clone()
    {
      return (BindingElement) new PrivacyNoticeBindingElement(this);
    }

    /// <summary>Queries the binding element stack to see whether it supports a particular interface.</summary>
    /// <param name="context">The <see cref="T:System.ServiceModel.Channels.BindingContext" /> for the current binding element.</param>
    /// <typeparam name="T">The interface whose support is being tested.</typeparam>
    /// <returns>The interface whose support is being tested.</returns>
    public override T GetProperty<T>(BindingContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      return context.GetInnerProperty<T>();
    }

#if !FEATURE_CORECLR
    void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
    {
      if (context == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
      if (context.BindingElements == null)
        return;
      PrivacyNoticeBindingElement noticeBindingElement = context.BindingElements.Find<PrivacyNoticeBindingElement>();
      if (noticeBindingElement == null)
        return;
      XmlElement element = new XmlDocument().CreateElement("ic", "PrivacyNotice", "http://schemas.xmlsoap.org/ws/2005/05/identity");
      element.InnerText = noticeBindingElement.Url.ToString();
      element.SetAttribute("Version", "http://schemas.xmlsoap.org/ws/2005/05/identity", XmlConvert.ToString(noticeBindingElement.Version));
      context.GetBindingAssertions().Add(element);
    }
#endif

#if FEATURE_CORECLR
    public override bool IsMatch(BindingElement b)
#else
    internal override bool IsMatch(BindingElement b)
#endif
    {
      if (b == null)
        return false;
      PrivacyNoticeBindingElement noticeBindingElement = b as PrivacyNoticeBindingElement;
      if (noticeBindingElement == null || !(this.url == noticeBindingElement.url))
        return false;
      return this.version == noticeBindingElement.version;
    }
  }
}
