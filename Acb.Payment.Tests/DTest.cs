using Acb.Core.Dependency;

namespace Acb.Payment.Tests
{
    public class DTest : Framework.DTest
    {
        protected T Resolve<T>() => CurrentIocManager.Resolve<T>();
    }
}
