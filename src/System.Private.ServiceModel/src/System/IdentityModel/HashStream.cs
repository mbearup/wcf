// Decompiled with JetBrains decompiler
// Type: System.IdentityModel.HashStream
// Assembly: System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// MVID: C5D1B514-2F32-41F2-B2CB-D016BF368CD2
// Assembly location: C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.IdentityModel.dll

using System.IO;
using System.ServiceModel;
using System.Security.Cryptography;

namespace System.IdentityModel
{
  internal sealed class HashStream : Stream
  {
    private HashAlgorithm hash;
    private long length;
    private bool disposed;
    private bool hashNeedsReset;
    private MemoryStream logStream;

    public override bool CanRead
    {
      get
      {
        return false;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return false;
      }
    }

    public HashAlgorithm Hash
    {
      get
      {
        return this.hash;
      }
    }

    public override long Length
    {
      get
      {
        return this.length;
      }
    }

    public override long Position
    {
      get
      {
        return this.length;
      }
      set
      {
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
      }
    }

    public HashStream(HashAlgorithm hash)
    {
      if (hash == null)
        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("hash");
      this.Reset(hash);
    }

    public override void Flush()
    {
    }

    public void FlushHash()
    {
      this.FlushHash((MemoryStream) null);
    }

    public void FlushHash(MemoryStream preCanonicalBytes)
    {
      this.hash.TransformFinalBlock(CryptoHelper.EmptyBuffer, 0, 0);
      if (!DigestTraceRecordHelper.ShouldTraceDigest)
        return;
      DigestTraceRecordHelper.TraceDigest(this.logStream, this.hash);
    }

    public byte[] FlushHashAndGetValue()
    {
      return this.FlushHashAndGetValue((MemoryStream) null);
    }

    public byte[] FlushHashAndGetValue(MemoryStream preCanonicalBytes)
    {
      this.FlushHash(preCanonicalBytes);
      return this.hash.Hash;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    public void Reset()
    {
      if (this.hashNeedsReset)
      {
        this.hash.Initialize();
        this.hashNeedsReset = false;
      }
      this.length = 0L;
      if (!DigestTraceRecordHelper.ShouldTraceDigest)
        return;
      this.logStream = new MemoryStream();
    }

    public void Reset(HashAlgorithm hash)
    {
      this.hash = hash;
      this.hashNeedsReset = false;
      this.length = 0L;
      if (!DigestTraceRecordHelper.ShouldTraceDigest)
        return;
      this.logStream = new MemoryStream();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.hash.TransformBlock(buffer, offset, count, buffer, offset);
      this.length = this.length + (long) count;
      this.hashNeedsReset = true;
      if (!DigestTraceRecordHelper.ShouldTraceDigest)
        return;
      this.logStream.Write(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    public override void SetLength(long length)
    {
      throw DiagnosticUtility.ExceptionUtility.ThrowHelperError((Exception) new NotSupportedException());
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (this.disposed)
        return;
      if (disposing && this.logStream != null)
      {
        this.logStream.Dispose();
        this.logStream = (MemoryStream) null;
      }
      this.disposed = true;
    }
  }
}
