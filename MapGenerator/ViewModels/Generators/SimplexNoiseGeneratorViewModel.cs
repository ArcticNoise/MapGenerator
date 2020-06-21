using System.Threading;
using MapGenerator.Generators;
using MapGenerator.Generators.GenerationData;
using MapGenerator.ViewModels.Generators.Interfaces;

namespace MapGenerator.ViewModels.Generators
{
    public class SimplexNoiseGeneratorViewModel : AbstractViewModel, IGeneratorViewModel
    {
        public float MinFrequencyValue => 1;
        public float MaxFrequencyValue => 10;

        public float MinOctavesValue => 1;
        public float MaxOctavesValue => 7;

        public float MinRedistributionValue => 0.01f;
        public float MaxRedistributionValue => 10f;

        public string Name => "Simplex noise";

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

        private int m_Seed;
        public int Seed
        {
            get => m_Seed;
            set => SetProperty(ref m_Seed, value);
        }

        private float m_Frequency;
        public float Frequency
        {
            get => m_Frequency;
            set => SetProperty(ref m_Frequency, value);
        }

        private int m_Octaves;
        public int Octaves
        {
            get => m_Octaves;
            set => SetProperty(ref m_Octaves, value);
        }

        private float m_Redistribution;
        public float Redistribution
        {
            get => m_Redistribution;
            set => SetProperty(ref m_Redistribution, value);
        }

        public SimplexNoiseGeneratorViewModel()
        {
            m_Generator = new SimplexNoiseGenerator();

            InitDefaultValues();
        }
        
        public float[,] GenerateMap(CancellationToken token = new CancellationToken())
        {
            var generationData = new SimplexNoiseGenerationData()
            {
                Seed = Seed,
                Width = Width,
                Height = Height,
                Frequency = Frequency,
                Octaves = Octaves,
                Redistribution = Redistribution
            };

            return m_Generator.GenerateMap(generationData, token);
        }

        private void InitDefaultValues()
        {
            Seed = 666;
            Width = 512;
            Height = 512;
            Frequency = 1;
            Octaves = 1;
            Redistribution = 1;
        }
    }
}
