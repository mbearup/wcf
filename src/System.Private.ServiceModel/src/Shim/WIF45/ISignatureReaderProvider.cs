// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.ISignatureReaderProvider
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.Xml;

namespace System.IdentityModel
{
  internal interface ISignatureReaderProvider
  {
    XmlDictionaryReader GetReader(object callbackContext);
  }
}
