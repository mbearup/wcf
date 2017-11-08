// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Web.WebGetAttribute
// Assembly: System.ServiceModel.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A1A7EEFD-8CC6-4A24-A736-BEC1D3402EEA
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.Web.dll

using System.ServiceModel.Administration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace System.ServiceModel.Web
{
  /// <summary>Represents an attribute indicating that a service operation is logically a retrieval operation and that it can be called by the WCF REST programming model.</summary>
  [AttributeUsage(AttributeTargets.Method)]
  public sealed class WebGetAttribute : Attribute, IOperationContractAttributeProvider, IOperationBehavior, IWmiInstanceProvider
  {
    private WebMessageBodyStyle bodyStyle;
    private bool isBodyStyleDefined;
    private bool isRequestMessageFormatSet;
    private bool isResponseMessageFormatSet;
    private WebMessageFormat requestMessageFormat;
    private WebMessageFormat responseMessageFormat;
    private string uriTemplate;

    /// <summary>Gets and sets the body style of the messages that are sent to and from the service operation.</summary>
    /// <returns>One of the <see cref="T:System.ServiceModel.Web.WebMessageBodyStyle" /> enumeration values.</returns>
    public WebMessageBodyStyle BodyStyle
    {
      get
      {
        return this.bodyStyle;
      }
      set
      {
        if (!WebMessageBodyStyleHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.bodyStyle = value;
        this.isBodyStyleDefined = true;
      }
    }

    /// <summary>Gets the <see cref="P:System.ServiceModel.Web.WebGetAttribute.IsBodyStyleSetExplicitly" /> property.</summary>
    /// <returns>A value that specifies whether the <see cref="P:System.ServiceModel.Web.WebGetAttribute.BodyStyle" /> property is set.</returns>
    public bool IsBodyStyleSetExplicitly
    {
      get
      {
        return this.isBodyStyleDefined;
      }
    }

    /// <summary>Gets the <see cref="P:System.ServiceModel.Web.WebGetAttribute.IsRequestFormatSetExplicitly" /> property.</summary>
    /// <returns>A value that specifies whether the <see cref="P:System.ServiceModel.Web.WebGetAttribute.RequestFormat" /> property was set.</returns>
    public bool IsRequestFormatSetExplicitly
    {
      get
      {
        return this.isRequestMessageFormatSet;
      }
    }

    /// <summary>Gets the <see cref="P:System.ServiceModel.Web.WebGetAttribute.IsResponseFormatSetExplicitly" /> property.</summary>
    /// <returns>A value that specifies whether the <see cref="P:System.ServiceModel.Web.WebGetAttribute.ResponseFormat" /> property was set.</returns>
    public bool IsResponseFormatSetExplicitly
    {
      get
      {
        return this.isResponseMessageFormatSet;
      }
    }

    /// <summary>Gets and sets the <see cref="P:System.ServiceModel.Web.WebGetAttribute.RequestFormat" /> property.</summary>
    /// <returns>One of the <see cref="T:System.ServiceModel.Web.WebMessageFormat" /> enumeration values.</returns>
    public WebMessageFormat RequestFormat
    {
      get
      {
        return this.requestMessageFormat;
      }
      set
      {
        if (!WebMessageFormatHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.requestMessageFormat = value;
        this.isRequestMessageFormatSet = true;
      }
    }

    /// <summary>Gets and sets the <see cref="P:System.ServiceModel.Web.WebGetAttribute.ResponseFormat" /> property.</summary>
    /// <returns>One of the <see cref="T:System.ServiceModel.Web.WebMessageFormat" /> enumeration values.</returns>
    public WebMessageFormat ResponseFormat
    {
      get
      {
        return this.responseMessageFormat;
      }
      set
      {
        if (!WebMessageFormatHelper.IsDefined(value))
          throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new ArgumentOutOfRangeException("value"));
        this.responseMessageFormat = value;
        this.isResponseMessageFormatSet = true;
      }
    }

    /// <summary>Gets and sets the Uniform Resource Identifier (URI) template for the service operation.</summary>
    /// <returns>The URI template for the service operation.</returns>
    public string UriTemplate
    {
      get
      {
        return this.uriTemplate;
      }
      set
      {
        this.uriTemplate = value;
      }
    }

    void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
    }

    void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
    }

    void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
    }

    void IOperationBehavior.Validate(OperationDescription operationDescription)
    {
    }

    void IWmiInstanceProvider.FillInstance(IWmiInstance wmiInstance)
    {
      if (wmiInstance == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("wmiInstance");
      wmiInstance.SetProperty("BodyStyle", (object) this.BodyStyle.ToString());
      IWmiInstance wmiInstance1 = wmiInstance;
      string name1 = "IsBodyStyleSetExplicitly";
      bool flag = this.IsBodyStyleSetExplicitly;
      string str1 = flag.ToString();
      wmiInstance1.SetProperty(name1, (object) str1);
      IWmiInstance wmiInstance2 = wmiInstance;
      string name2 = "RequestFormat";
      WebMessageFormat webMessageFormat = this.RequestFormat;
      string str2 = webMessageFormat.ToString();
      wmiInstance2.SetProperty(name2, (object) str2);
      IWmiInstance wmiInstance3 = wmiInstance;
      string name3 = "IsRequestFormatSetExplicitly";
      flag = this.IsRequestFormatSetExplicitly;
      string str3 = flag.ToString();
      wmiInstance3.SetProperty(name3, (object) str3);
      IWmiInstance wmiInstance4 = wmiInstance;
      string name4 = "ResponseFormat";
      webMessageFormat = this.ResponseFormat;
      string str4 = webMessageFormat.ToString();
      wmiInstance4.SetProperty(name4, (object) str4);
      IWmiInstance wmiInstance5 = wmiInstance;
      string name5 = "IsResponseFormatSetExplicitly";
      flag = this.IsResponseFormatSetExplicitly;
      string str5 = flag.ToString();
      wmiInstance5.SetProperty(name5, (object) str5);
      wmiInstance.SetProperty("UriTemplate", (object) this.UriTemplate);
    }

    string IWmiInstanceProvider.GetInstanceType()
    {
      return "WebGetAttribute";
    }

    internal WebMessageBodyStyle GetBodyStyleOrDefault(WebMessageBodyStyle defaultStyle)
    {
      if (this.IsBodyStyleSetExplicitly)
        return this.BodyStyle;
      return defaultStyle;
    }

    OperationContractAttribute IOperationContractAttributeProvider.GetOperationContractAttribute()
    {
      return new OperationContractAttribute();
    }
  }
}
