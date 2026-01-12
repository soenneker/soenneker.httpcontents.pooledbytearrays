[![](https://img.shields.io/nuget/v/soenneker.httpcontents.pooledbytearrays.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.httpcontents.pooledbytearrays/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.httpcontents.pooledbytearrays/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.httpcontents.pooledbytearrays/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.httpcontents.pooledbytearrays.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.httpcontents.pooledbytearrays/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.HttpContents.PooledByteArrays
### A custom HttpContent that wraps a rented byte array from an ArrayPool, writes only the valid byte range to outgoing streams, and returns the buffer to the pool when disposed to reduce GC allocations.

## Installation

```
dotnet add package Soenneker.HttpContents.PooledByteArrays
```
