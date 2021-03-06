﻿// Decompiled with JetBrains decompiler
// Type: System.ServiceModel.Security.SessionKeyExpiredException
// Assembly: System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: D99220A3-68F3-4C93-A7DA-DD40486B6567
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.ServiceModel.dll

using System.Runtime.Serialization;

namespace System.ServiceModel.Security
{
  [Serializable]
  internal class SessionKeyExpiredException : MessageSecurityException
  {
    public SessionKeyExpiredException()
    {
    }

    public SessionKeyExpiredException(string message)
      : base(message)
    {
    }

    public SessionKeyExpiredException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected SessionKeyExpiredException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
