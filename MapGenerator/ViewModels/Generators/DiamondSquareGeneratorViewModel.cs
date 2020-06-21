using System;
using System.Threading;
using MapGenerator.Generators;
using MapGenerator.Generators.GenerationData;
using MapGenerator.ViewModels.Generators.Interfaces;

namespace MapGenerator.ViewModels.Generators
{
    public class DiamondSquareGeneratorViewModel : AbstractViewModel, IGeneratorViewModel
    {
        private readonly DiamondSquareGenerator m_Generator;

        public int MinPowerValue => 2;
        public int MaxPowerValue => 14;

        public string Name => "Diamond-Square";

        private int m_Seed;
        public int Seed
        {
            get => m_Seed;
            set => SetProperty(ref m_Seed, value);
        }

        private int m_PowerOfTwo;
        public int PowerOfTwo
        {
            get => m_PowerOfTwo;
            set
            {
                if (value != m_PowerOfTwo)
                {
                    m_PowerOfTwo = value;
                    OnPropertyChanged();
                    RecalculateExpectedImageSize();
                }
            }
        }

        private int m_Roughness;
        public int Roughness
        {
            get => m_Roughness;
            set => SetProperty(ref m_Roughness, value);
        }

        public string ExpectedImageSizeString => $"({ExpectedImageSize}x{ExpectedImageSize})";

        private int m_ExpectedImageSize;
        public int ExpectedImageSize
        {
            get => m_ExpectedImageSize;
            set
            {
                if (m_ExpectedImageSize != value)
                {
                    m_ExpectedImageSize = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ExpectedImageSizeString));
                }
            }
        }

        public DiamondSquareGeneratorViewModel()
        {
            m_Generator = new DiamondSquareGenerator();
            InitWithDefaultValues();
        }

        public float[,] GenerateMap(CancellationToken token = new CancellationToken())
        {
            var generationData = new DiamondSquareGenerationData()
            {
                Seed = Seed, 
                PowerOfTwo= PowerOfTwo, 
                Roughness= Roughness
            };

            return m_Generator.GenerateMap(generationData, token);
        }

        private void RecalculateExpectedImageSize()
        {
            var value = (int)Math.Pow(2, m_PowerOfTwo) + 1;
            ExpectedImageSize = value;
        }

        private void InitWithDefaultValues()
        {
            PowerOfTwo = 8;
            Seed = 666;
            Roughness = 1;
        }
    }
}
