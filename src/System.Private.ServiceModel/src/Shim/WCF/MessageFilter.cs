// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Dispatcher.MessageFilter
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime.Serialization;
using System.ServiceModel.Channels;

// This class is a stub

namespace System.ServiceModel.Dispatcher
{
  internal abstract class MessageFilter
  {
    public abstract bool Match(MessageBuffer buffer);

    public abstract bool Match(Message message);
  }
}
