using System.Threading.Tasks;

namespace Launcher.Infrastructure.Abstractions
{
    public interface IProcessRunner
    {
        Task<int> RunAsync(string executable, string arguments);
    }
}