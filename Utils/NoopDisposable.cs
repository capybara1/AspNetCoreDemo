using System;

namespace AspNetCoreDemo.Utils
{
    internal class NoopDisposable : IDisposable
    {
        public static readonly NoopDisposable Instance = new NoopDisposable();

        public void Dispose()
        { }
    }
}
