using System.Threading;

namespace MapGenerator.ViewModels.Interfaces
{
    public interface IGeneratorViewModel
    {
        float[,] GenerateMap();
        float[,] GenerateMap(CancellationToken token);
    }
}