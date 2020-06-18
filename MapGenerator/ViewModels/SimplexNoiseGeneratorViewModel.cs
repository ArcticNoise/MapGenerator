using System.Threading;
using MapGenerator.Generators;
using MapGenerator.ViewModels.Interfaces;

namespace MapGenerator.ViewModels
{
    public class SimplexNoiseGeneratorViewModel : AbstractViewModel, IGeneratorViewModel
    {
        private const int Seed = 666;

        private const float MinHeightValue = 0;
        private const float MaxHeightValue = 255;

        private readonly SimplexNoiseGenerator m_Generator;

        private int m_Width;
        public int Width
        {
            get => m_Width;
            set => SetProperty(ref m_Width, value);
        }

        private int m_Height;
        public int Height
        {
            get => m_Height;
            set => SetProperty(ref m_Height, value);
        }
        

        public SimplexNoiseGeneratorViewModel()
        {
            m_Generator = new SimplexNoiseGenerator(Seed);
            Width = 256;
            Height = 256;
        }

        public float[,] GenerateMap()
        {
            return m_Generator.GenerateMap(Width, Height, MinHeightValue, MaxHeightValue);
        }

        public float[,] GenerateMap(CancellationToken token)
        {
            return m_Generator.GenerateMap(Width, Height, MinHeightValue, MaxHeightValue, token);
        }
    }
}
