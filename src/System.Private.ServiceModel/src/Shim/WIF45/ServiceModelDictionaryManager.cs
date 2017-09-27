// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.ServiceModelDictionaryManager
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.IdentityModel;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
  internal class ServiceModelDictionaryManager
  {
    private static DictionaryManager dictionaryManager;

    public static DictionaryManager Instance
    {
      get
      {
        if (ServiceModelDictionaryManager.dictionaryManager == null)
          ServiceModelDictionaryManager.dictionaryManager = new DictionaryManager(BinaryMessageEncoderFactory.XmlDictionary);
        return ServiceModelDictionaryManager.dictionaryManager;
      }
    }
  }
}
