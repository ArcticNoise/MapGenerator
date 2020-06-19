using System.Threading;

namespace MapGenerator.ViewModels.Interfaces
{
    public interface IGeneratorViewModel
    {
        string Name { get; }

        float[,] GenerateMap();
        float[,] GenerateMap(CancellationToken token);
    }
}