using System;

namespace Wind.iSeller.Framework.Core.Utils
{
    /// <summary>
    /// This class is used to simulate a Disposable that does nothing.
    /// </summary>
    internal sealed class NullDisposable : IDisposable
    {
        public static NullDisposable Instance { get; private set; } 

        private NullDisposable()
        {

        }

        static NullDisposable()
        {
            NullDisposable.Instance = new NullDisposable();
        }

        public void Dispose()
        {

        }
    }
}
