﻿using System;
using MapGenerator.Generators;
using MapGenerator.ViewModels.Interfaces;

namespace MapGenerator.ViewModels
{
    public class DiamondSquareGeneratorViewModel : AbstractViewModel, IGeneratorViewModel
    {
        private const int Seed = 666;
        private readonly DiamondSquareGenerator m_Generator;
        
        private const float MinHeightValue = 0;
        private const float MaxHeightValue = 255;

        public int MinPowerValue => 2;
        public int MaxPowerValue => 14;

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
            m_Generator = new DiamondSquareGenerator(Seed);
            //Start value
            PowerOfTwo = 8;
        }

        public float[,] GenerateMap()
        {
            return m_Generator.GenerateMap(PowerOfTwo, MinHeightValue, MaxHeightValue, Roughness);
        }

        private void RecalculateExpectedImageSize()
        {
            var value = (int)Math.Pow(2, m_PowerOfTwo) + 1;
            ExpectedImageSize = value;
        }
    }
}