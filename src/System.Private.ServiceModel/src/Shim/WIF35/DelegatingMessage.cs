// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Channels.DelegatingMessage
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Xml;

namespace System.ServiceModel.Channels
{
  internal abstract class DelegatingMessage : Message
  {
    private Message innerMessage;

    public override bool IsEmpty
    {
      get
      {
        return this.innerMessage.IsEmpty;
      }
    }

    public override bool IsFault
    {
      get
      {
        return this.innerMessage.IsFault;
      }
    }

    public override MessageHeaders Headers
    {
      get
      {
        return this.innerMessage.Headers;
      }
    }

    public override MessageProperties Properties
    {
      get
      {
        return this.innerMessage.Properties;
      }
    }

    public override MessageVersion Version
    {
      get
      {
        return this.innerMessage.Version;
      }
    }

    protected Message InnerMessage
    {
      get
      {
        return this.innerMessage;
      }
    }

    protected DelegatingMessage(Message innerMessage)
    {
      if (innerMessage == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerMessage");
      this.innerMessage = innerMessage;
    }

    protected override void OnClose()
    {
      base.OnClose();
      this.innerMessage.Close();
    }

    protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
    {
      this.innerMessage.WriteStartEnvelope(writer);
    }

    protected override void OnWriteStartHeaders(XmlDictionaryWriter writer)
    {
      this.innerMessage.WriteStartHeaders(writer);
    }

    protected override void OnWriteStartBody(XmlDictionaryWriter writer)
    {
      this.innerMessage.WriteStartBody(writer);
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
      this.innerMessage.WriteBodyContents(writer);
    }

    protected override string OnGetBodyAttribute(string localName, string ns)
    {
      return this.innerMessage.GetBodyAttribute(localName, ns);
    }

    protected override void OnBodyToString(XmlDictionaryWriter writer)
    {
      this.innerMessage.BodyToString(writer);
    }
  }
}
