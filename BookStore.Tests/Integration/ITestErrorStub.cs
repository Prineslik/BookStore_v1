using System;
using System.Collections.Generic;
using System.Text;

namespace BookStore.Tests.Integration
{
    public interface ITestErrorStub
    {
        void ThrowIfConfigured();
    }

}
