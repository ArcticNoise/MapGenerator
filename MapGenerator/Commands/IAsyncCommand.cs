using System.Threading.Tasks;
using System.Windows.Input;

namespace MapGenerator.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}