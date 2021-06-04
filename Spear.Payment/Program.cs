using Spear.WebApi;
using System.Threading.Tasks;

namespace Spear.Gateway.Payment
{
    public class Program : DHost<Startup>
    {
        public static async Task Main(string[] args)
        {
            await Start(args);
        }
    }
}
