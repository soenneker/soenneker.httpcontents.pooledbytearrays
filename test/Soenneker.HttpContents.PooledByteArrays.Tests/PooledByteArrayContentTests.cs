using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Soenneker.Tests.Unit;

namespace Soenneker.HttpContents.PooledByteArrays.Tests;

public sealed class PooledByteArrayContentTests : UnitTest
{
    [Test]
    public void Default()
    {

    }

    [Test]
    public async Task Exposes_only_count_and_returns_buffer_once()
    {
        var pool = new TrackingPool();
        byte[] buffer = [1, 2, 3, 99, 100];
        var content = new PooledByteArrayContent(pool, buffer, 3, clearArrayOnDispose: true);

        byte[] body = await content.ReadAsByteArrayAsync();

        await Assert.That(body.SequenceEqual(new byte[] { 1, 2, 3 })).IsTrue();
        await Assert.That(content.Headers.ContentLength).IsEqualTo(3);

        content.Dispose();
        content.Dispose();

        await Assert.That(pool.ReturnCount).IsEqualTo(1);
        await Assert.That(pool.Returned).IsSameReferenceAs(buffer);
        await Assert.That(pool.ClearArray).IsTrue();
    }

    private sealed class TrackingPool : ArrayPool<byte>
    {
        public int ReturnCount { get; private set; }
        public byte[]? Returned { get; private set; }
        public bool ClearArray { get; private set; }

        public override byte[] Rent(int minimumLength) => new byte[minimumLength];

        public override void Return(byte[] array, bool clearArray = false)
        {
            ReturnCount++;
            Returned = array;
            ClearArray = clearArray;
        }
    }
}
