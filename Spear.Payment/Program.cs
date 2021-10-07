using System.Threading.Tasks;
using Spear.WebApi;

namespace Spear.Payment
{
    public class Program : DHost<Startup>
    {
        public static async Task Main(string[] args)
        {
            await Start(args);
        }
    }
}
