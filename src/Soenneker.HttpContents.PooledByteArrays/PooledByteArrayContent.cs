using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.HttpContents.PooledByteArrays;

/// <summary>
/// Provides HTTP content based on a pooled byte array, enabling efficient transfer of binary data without unnecessary
/// allocations.
/// </summary>
/// <remarks>This class wraps a byte array rented from an <see cref="ArrayPool{T}"/> and exposes it as <see
/// cref="HttpContent"/> for use in HTTP requests or responses. The buffer is returned to the pool when the content is
/// disposed, reducing memory pressure in high-throughput scenarios. The content length is fixed and corresponds to the
/// specified count. This type is sealed and not intended for inheritance.</remarks>
public sealed class PooledByteArrayContent : HttpContent
{
    private readonly ArrayPool<byte> _pool;
    private byte[]? _buffer;
    private readonly int _count;
    private readonly bool _clearArrayOnDispose;

    /// <summary>
    /// Takes ownership of a rented buffer and exposes its first <paramref name="count"/> bytes as HTTP content.
    /// </summary>
    /// <param name="pool">The pool that rented <paramref name="buffer"/> and will receive it when this content is disposed.</param>
    /// <param name="buffer">A buffer rented from <paramref name="pool"/>. The caller must not use or return it after construction.</param>
    /// <param name="count">The number of initialized bytes to expose.</param>
    /// <param name="clearArrayOnDispose">Whether the pool should clear the array before making it available to another renter.</param>
    public PooledByteArrayContent(ArrayPool<byte> pool, byte[] buffer, int count, bool clearArrayOnDispose = false)
    {
        _pool = pool ?? throw new ArgumentNullException(nameof(pool));
        _buffer = buffer ?? throw new ArgumentNullException(nameof(buffer));

        if ((uint)count > (uint)buffer.Length) // catches negative + > Length
            throw new ArgumentOutOfRangeException(nameof(count));

        _count = count;
        _clearArrayOnDispose = clearArrayOnDispose;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] GetBufferOrThrow()
    {
        byte[]? buffer = Volatile.Read(ref _buffer);

        if (buffer is null)
            throw new ObjectDisposedException(nameof(PooledByteArrayContent));

        return buffer;
    }

    protected override bool TryComputeLength(out long length)
    {
        length = _count;
        return true;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        byte[] buffer = GetBufferOrThrow();
        return stream.WriteAsync(buffer.AsMemory(0, _count), cancellationToken)
                     .AsTask();
    }

    // Kept for compatibility; forwards to the cancellable path when available.
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        byte[] buffer = GetBufferOrThrow();

        return stream.WriteAsync(buffer.AsMemory(0, _count), CancellationToken.None)
                     .AsTask();
    }

    protected override Task<Stream> CreateContentReadStreamAsync()
    {
        byte[] buffer = GetBufferOrThrow();
        // Exposes only [0.._count); no extra allocations besides the MemoryStream object itself.
        Stream s = new MemoryStream(buffer, 0, _count, writable: false, publiclyVisible: false);
        return Task.FromResult(s);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            byte[]? buffer = Interlocked.Exchange(ref _buffer, null);

            if (buffer is not null)
                _pool.Return(buffer, _clearArrayOnDispose);
        }
    }
}
