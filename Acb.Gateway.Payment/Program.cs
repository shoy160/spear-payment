using Acb.WebApi;
using System.Threading.Tasks;

namespace Acb.Gateway.Payment
{
    public class Program : DHost<Startup>
    {
        public static async Task Main(string[] args)
        {
            await Start(args);
        }
    }
}
