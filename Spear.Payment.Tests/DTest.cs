using Spear.Core.Dependency;

namespace Spear.Payment.Tests
{
    public class DTest : Framework.DTest
    {
        protected T Resolve<T>() => CurrentIocManager.Resolve<T>();
    }
}
