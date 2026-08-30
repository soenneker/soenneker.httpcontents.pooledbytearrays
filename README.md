[![](https://img.shields.io/nuget/v/soenneker.httpcontents.pooledbytearrays.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.httpcontents.pooledbytearrays/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.httpcontents.pooledbytearrays/build-and-test.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.httpcontents.pooledbytearrays/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.httpcontents.pooledbytearrays/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.httpcontents.pooledbytearrays/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.httpcontents.pooledbytearrays.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.httpcontents.pooledbytearrays/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.httpcontents.pooledbytearrays/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.httpcontents.pooledbytearrays/actions/workflows/codeql.yml)

# Soenneker.HttpContents.PooledByteArrays

An `HttpContent` implementation that sends initialized bytes from an `ArrayPool<byte>` rental and returns the array on disposal.

## Install

```bash
dotnet add package Soenneker.HttpContents.PooledByteArrays
```

## Usage

```csharp
using System.Buffers;
using System.Net.Http.Headers;
using Soenneker.HttpContents.PooledByteArrays;

ArrayPool<byte> pool = ArrayPool<byte>.Shared;
byte[] buffer = pool.Rent(payload.Length);
payload.CopyTo(buffer);

using var content = new PooledByteArrayContent(
    pool,
    buffer,
    payload.Length,
    clearArrayOnDispose: true);

content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

using HttpResponseMessage response = await httpClient.PostAsync(
    "uploads",
    content,
    cancellationToken);

response.EnsureSuccessStatusCode();
```

Construction transfers ownership of `buffer` to `PooledByteArrayContent`. Do not read, write, or return the array afterward. Disposing the content returns it to the exact pool supplied to the constructor; disposing more than once does not return it twice.

`count` is the content length and must be between zero and `buffer.Length`. Bytes after `count` are never serialized.

Set `clearArrayOnDispose: true` when the rental may contain credentials, personal data, or other sensitive bytes. Clearing is disabled by default to avoid the extra full-array write for non-sensitive, high-throughput payloads.

Keep the content alive until the HTTP operation has completed, and do not send the same instance concurrently. A send uses the owned array directly; disposing or mutating ownership while an operation is in flight is invalid.
