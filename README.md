[![](https://img.shields.io/nuget/v/soenneker.httpcontents.pooledbytearrays.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.httpcontents.pooledbytearrays/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.httpcontents.pooledbytearrays/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.httpcontents.pooledbytearrays/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.httpcontents.pooledbytearrays.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.httpcontents.pooledbytearrays/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.httpcontents.pooledbytearrays/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.httpcontents.pooledbytearrays/actions/workflows/codeql.yml)

# Soenneker.HttpContents.PooledByteArrays

Provides HTTP content based on a pooled byte array, enabling efficient transfer of binary data without unnecessary allocations.

## Install

```bash
dotnet add package Soenneker.HttpContents.PooledByteArrays
```

## What you get

- `PooledByteArrayContent` — Provides HTTP content based on a pooled byte array, enabling efficient transfer of binary data without unnecessary allocations.

## Important behavior

- `PooledByteArrayContent`: This class wraps a byte array rented from an `ArrayPool{T}` and exposes it as `HttpContent` for use in HTTP requests or responses. The buffer is returned to the pool when the content is disposed, reducing memory pressure in high-throughput scenarios. The content length is fixed and corresponds to the specified count. This type is sealed and not intended for inheritance.
