using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Web.TestHelpers
{
    using System.IO;
    using System.Runtime.Remoting;

    public class NonClosingMemoryStream : Stream
    {
        private readonly MemoryStream _stream;

        public NonClosingMemoryStream(MemoryStream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        /// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="RemotingConfiguration, Infrastructure"/></PermissionSet>
        public override object InitializeLifetimeService()
        {
            return _stream.InitializeLifetimeService();
        }

        /// <summary>
        /// Creates an object that contains all the relevant information required to generate a proxy used to communicate with a remote object.
        /// </summary>
        /// <returns>
        /// Information required to generate a proxy.
        /// </returns>
        /// <param name="requestedType">The <see cref="T:System.Type"/> of the object that the new <see cref="T:System.Runtime.Remoting.ObjRef"/> will reference. </param><exception cref="T:System.Runtime.Remoting.RemotingException">This instance is not a valid remoting object. </exception><exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception><filterpriority>2</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="Infrastructure"/></PermissionSet>
        public override ObjRef CreateObjRef(Type requestedType)
        {
            return _stream.CreateObjRef(requestedType);
        }

        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
        /// <filterpriority>1</filterpriority>
        public override void Close()
        {
        }

        /// <summary>
        /// Releases all resources used by the <see cref="T:System.IO.Stream"/>.
        /// </summary>
        public new void Dispose()
        {
        }

        /// <summary>
        /// Begins an asynchronous read operation.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.IAsyncResult"/> that represents the asynchronous read, which could still be pending.
        /// </returns>
        /// <param name="buffer">The buffer to read the data into. </param><param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data read from the stream. </param><param name="count">The maximum number of bytes to read. </param><param name="callback">An optional asynchronous callback, to be called when the read is complete. </param><param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests. </param><exception cref="T:System.IO.IOException">Attempted an asynchronous read past the end of the stream, or a disk error occurs. </exception><exception cref="T:System.ArgumentException">One or more of the arguments is invalid. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><exception cref="T:System.NotSupportedException">The current Stream implementation does not support the read operation. </exception><filterpriority>2</filterpriority>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _stream.BeginRead(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Waits for the pending asynchronous read to complete.
        /// </summary>
        /// <returns>
        /// The number of bytes read from the stream, between zero (0) and the number of bytes you requested. Streams return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is available.
        /// </returns>
        /// <param name="asyncResult">The reference to the pending asynchronous request to finish. </param><exception cref="T:System.ArgumentNullException"><paramref name="asyncResult"/> is null. </exception><exception cref="T:System.ArgumentException"><paramref name="asyncResult"/> did not originate from a <see cref="M:System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)"/> method on the current stream. </exception><exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception><filterpriority>2</filterpriority>
        public override int EndRead(IAsyncResult asyncResult)
        {
            return _stream.EndRead(asyncResult);
        }

        /// <summary>
        /// Begins an asynchronous write operation.
        /// </summary>
        /// <returns>
        /// An IAsyncResult that represents the asynchronous write, which could still be pending.
        /// </returns>
        /// <param name="buffer">The buffer to write data from. </param><param name="offset">The byte offset in <paramref name="buffer"/> from which to begin writing. </param><param name="count">The maximum number of bytes to write. </param><param name="callback">An optional asynchronous callback, to be called when the write is complete. </param><param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests. </param><exception cref="T:System.IO.IOException">Attempted an asynchronous write past the end of the stream, or a disk error occurs. </exception><exception cref="T:System.ArgumentException">One or more of the arguments is invalid. </exception><exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception><exception cref="T:System.NotSupportedException">The current Stream implementation does not support the write operation. </exception><filterpriority>2</filterpriority>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return _stream.BeginWrite(buffer, offset, count, callback, state);
        }

        /// <summary>
        /// Ends an asynchronous write operation.
        /// </summary>
        /// <param name="asyncResult">A reference to the outstanding asynchronous I/O request. </param><exception cref="T:System.ArgumentNullException"><paramref name="asyncResult"/> is null. </exception><exception cref="T:System.ArgumentException"><paramref name="asyncResult"/> did not originate from a <see cref="M:System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)"/> method on the current stream. </exception><exception cref="T:System.IO.IOException">The stream is closed or an internal error has occurred.</exception><filterpriority>2</filterpriority>
        public override void EndWrite(IAsyncResult asyncResult)
        {
            _stream.EndWrite(asyncResult);
        }

        /// <summary>
        /// Gets a value that determines whether the current stream can time out.
        /// </summary>
        /// <returns>
        /// A value that determines whether the current stream can time out.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override bool CanTimeout
        {
            get { return _stream.CanTimeout; }
        }

        /// <summary>
        /// Gets or sets a value, in miliseconds, that determines how long the stream will attempt to read before timing out. 
        /// </summary>
        /// <returns>
        /// A value, in miliseconds, that determines how long the stream will attempt to read before timing out.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.IO.Stream.ReadTimeout"/> method always throws an <see cref="T:System.InvalidOperationException"/>. </exception><filterpriority>2</filterpriority>
        public override int ReadTimeout
        {
            get { return _stream.ReadTimeout; }
            set { _stream.ReadTimeout = value; }
        }

        /// <summary>
        /// Gets or sets a value, in miliseconds, that determines how long the stream will attempt to write before timing out. 
        /// </summary>
        /// <returns>
        /// A value, in miliseconds, that determines how long the stream will attempt to write before timing out.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.IO.Stream.WriteTimeout"/> method always throws an <see cref="T:System.InvalidOperationException"/>. </exception><filterpriority>2</filterpriority>
        public override int WriteTimeout
        {
            get { return _stream.WriteTimeout; }
            set { _stream.WriteTimeout = value; }
        }

        /// <summary>
        /// Overrides <see cref="M:System.IO.Stream.Flush"/> so that no action is performed.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// Returns the array of unsigned bytes from which this stream was created.
        /// </summary>
        /// <returns>
        /// The byte array from which this stream was created, or the underlying array if a byte array was not provided to the <see cref="T:System.IO.MemoryStream"/> constructor during construction of the current instance.
        /// </returns>
        /// <exception cref="T:System.UnauthorizedAccessException">The MemoryStream instance was not created with a publicly visible buffer. </exception><filterpriority>2</filterpriority>
        public byte[] GetBuffer()
        {
            return _stream.GetBuffer();
        }

        /// <summary>
        /// Reads a block of bytes from the current stream and writes the data to a buffer.
        /// </summary>
        /// <returns>
        /// The total number of bytes written into the buffer. This can be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached before any bytes are read.
        /// </returns>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the characters read from the current stream. </param><param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin storing data from the current stream. </param><param name="count">The maximum number of bytes to read. </param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> is negative. </exception><exception cref="T:System.ArgumentException"><paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>. </exception><exception cref="T:System.ObjectDisposedException">The current stream instance is closed. </exception><filterpriority>2</filterpriority>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Reads a byte from the current stream.
        /// </summary>
        /// <returns>
        /// The byte cast to a <see cref="T:System.Int32"/>, or -1 if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">The current stream instance is closed. </exception><filterpriority>2</filterpriority>
        public override int ReadByte()
        {
            return _stream.ReadByte();
        }

        /// <summary>
        /// Sets the position within the current stream to the specified value.
        /// </summary>
        /// <returns>
        /// The new position within the stream, calculated by combining the initial reference point and the offset.
        /// </returns>
        /// <param name="offset">The new position within the stream. This is relative to the <paramref name="loc"/> parameter, and can be positive or negative. </param><param name="loc">A value of type <see cref="T:System.IO.SeekOrigin"/>, which acts as the seek reference point. </param><exception cref="T:System.IO.IOException">Seeking is attempted before the beginning of the stream. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> is greater than <see cref="F:System.Int32.MaxValue"/>. </exception><exception cref="T:System.ArgumentException">There is an invalid <see cref="T:System.IO.SeekOrigin"/>. -or-<paramref name="offset"/> caused an arithmetic overflow.</exception><exception cref="T:System.ObjectDisposedException">The current stream instance is closed. </exception><filterpriority>2</filterpriority>
        public override long Seek(long offset, SeekOrigin loc)
        {
            return _stream.Seek(offset, loc);
        }

        /// <summary>
        /// Sets the length of the current stream to the specified value.
        /// </summary>
        /// <param name="value">The value at which to set the length. </param><exception cref="T:System.NotSupportedException">The current stream is not resizable and <paramref name="value"/> is larger than the current capacity.-or- The current stream does not support writing. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="value"/> is negative or is greater than the maximum length of the <see cref="T:System.IO.MemoryStream"/>, where the maximum length is(<see cref="F:System.Int32.MaxValue"/> - origin), and origin is the index into the underlying buffer at which the stream starts. </exception><filterpriority>2</filterpriority>
        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the <see cref="P:System.IO.MemoryStream.Position"/> property.
        /// </summary>
        /// <returns>
        /// A new byte array.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public byte[] ToArray()
        {
            return _stream.ToArray();
        }

        /// <summary>
        /// Writes a block of bytes to the current stream using data read from a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to write data from. </param><param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream. </param><param name="count">The maximum number of bytes to write. </param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/> is null. </exception><exception cref="T:System.NotSupportedException">The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.-or- The current position is closer than <paramref name="count"/> bytes to the end of the stream, and the capacity cannot be modified. </exception><exception cref="T:System.ArgumentException"><paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>. </exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="offset"/> or <paramref name="count"/> are negative. </exception><exception cref="T:System.IO.IOException">An I/O error occurs. </exception><exception cref="T:System.ObjectDisposedException">The current stream instance is closed. </exception><filterpriority>2</filterpriority>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Writes a byte to the current stream at the current position.
        /// </summary>
        /// <param name="value">The byte to write. </param><exception cref="T:System.NotSupportedException">The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.-or- The current position is at the end of the stream, and the capacity cannot be modified. </exception><exception cref="T:System.ObjectDisposedException">The current stream is closed. </exception><filterpriority>2</filterpriority>
        public override void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        /// Writes the entire contents of this memory stream to another stream.
        /// </summary>
        /// <param name="stream">The stream to write this memory stream to. </param><exception cref="T:System.ArgumentNullException"><paramref name="stream"/> is null. </exception><exception cref="T:System.ObjectDisposedException">The current or target stream is closed. </exception><filterpriority>2</filterpriority>
        public void WriteTo(Stream stream)
        {
            _stream.WriteTo(stream);
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>
        /// true if the stream is open.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>
        /// true if the stream is open.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>
        /// true if the stream supports writing; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        /// <summary>
        /// Gets or sets the number of bytes allocated for this stream.
        /// </summary>
        /// <returns>
        /// The length of the usable portion of the buffer for the stream.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">A capacity is set that is negative or less than the current length of the stream. </exception><exception cref="T:System.ObjectDisposedException">The current stream is closed. </exception><exception cref="T:System.NotSupportedException">set is invoked on a stream whose capacity cannot be modified. </exception><filterpriority>2</filterpriority>
        public int Capacity
        {
            get { return _stream.Capacity; }
            set { _stream.Capacity = value; }
        }

        /// <summary>
        /// Gets the length of the stream in bytes.
        /// </summary>
        /// <returns>
        /// The length of the stream in bytes.
        /// </returns>
        /// <exception cref="T:System.ObjectDisposedException">The stream is closed. </exception><filterpriority>2</filterpriority>
        public override long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        /// Gets or sets the current position within the stream.
        /// </summary>
        /// <returns>
        /// The current position within the stream.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The position is set to a negative value or a value greater than <see cref="F:System.Int32.MaxValue"/>. </exception><exception cref="T:System.ObjectDisposedException">The stream is closed. </exception><filterpriority>2</filterpriority>
        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public void ForceDispose()
        {
            try
            {
                _stream.Dispose();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch { }
// ReSharper restore EmptyGeneralCatchClause
        }
    }
}
