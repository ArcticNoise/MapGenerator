using System.Threading;

namespace MapGenerator.ViewModels.Generators.Interfaces
{
    public interface IGeneratorViewModel
    {
        string Name { get; }

        float[,] GenerateMap(CancellationToken token);
    }
}