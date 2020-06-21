using System;
using System.Threading;
using MapGenerator.Generators.GenerationData;

namespace MapGenerator.Generators
{
    public class DiamondSquareGenerator
    {
        private float[,] m_Map;
        private Random m_Random;

        private const float MinValue = 0;
        private const float MaxValue = 1;

        private const int MinValueOfTwo = 2;
        private const int MaxPowerOfTwo = 14;
        
        public float[,] GenerateMap(DiamondSquareGenerationData generationData, CancellationToken token)
        {
            if (generationData.PowerOfTwo > MaxPowerOfTwo)
            {
                throw new ArgumentException($"n should not be more then {MaxPowerOfTwo} or it can cause an overflow.");
            }

            if (generationData.PowerOfTwo < MinValueOfTwo)
            {
                throw new ArgumentException($"n should not be less then {MinValueOfTwo}.");
            }
            
            GenerateMapInternal(generationData, token);

            return m_Map;
        }
        
        private void GenerateMapInternal(DiamondSquareGenerationData generationData, CancellationToken token)
        {
            m_Random = new Random(generationData.Seed);

            var size = (int)Math.Pow(2, generationData.PowerOfTwo) + 1;

            m_Map = new float[size, size];

            var startValue = RandRange(MinValue, MaxValue);

            var maxIndex = size - 1;

            m_Map[0, 0] = startValue;
            m_Map[0, maxIndex] = startValue;
            m_Map[maxIndex, 0] = startValue;
            m_Map[maxIndex, maxIndex] = startValue;

            for (var i = maxIndex; i > 1; i /= 2)
            {
                var noiseModifier = (MaxValue - MinValue) * generationData.Roughness * ((float)i / maxIndex);

                for (var y = 0; y < maxIndex; y += i)
                {
                    for (var x = 0; x < maxIndex; x += i)
                    {
                        DiamondStep(x, y, i, noiseModifier);
                    }
                }

                for (var y = 0; y < maxIndex; y += i)
                {
                    for (var x = 0; x < maxIndex; x += i)
                    {
                        SquareStep(x, y, i, noiseModifier, maxIndex);
                    }
                }

                token.ThrowIfCancellationRequested();
            }
        }

        private void DiamondStep(int x, int y, int size, float noiseModifier)
        {
            var leftTopValue = m_Map[x, y];
            var leftBottomValue = m_Map[x + size, y];
            var rightTopValue = m_Map[x, y + size];
            var rightBottomValue = m_Map[x + size, y + size];

            var middleX = x + size / 2;
            var middleY = y + size / 2;

            var newMiddleValue = Normalize((leftTopValue + leftBottomValue + rightTopValue + rightBottomValue) / 4f + RandRange(-noiseModifier, noiseModifier));

            m_Map[middleX, middleY] = newMiddleValue;
        }
        
        private void SquareStep(int x, int y, int size, float noiseModifier, int maxIndex)
        {
            var leftTopValue = m_Map[x, y];
            var leftBottomValue = m_Map[x + size, y];
            var rightTopValue = m_Map[x, y + size];
            var rightBottomValue = m_Map[x + size, y + size];

            var middleValue = m_Map[x + (size / 2), y + (size / 2)];

            var newTopValue = y <= 0 ? (leftTopValue + leftBottomValue + middleValue) / 3.0f
                                 : (leftTopValue + leftBottomValue + middleValue + m_Map[x + (size / 2), y - (size / 2)]) / 4.0f;

            m_Map[x + (size / 2), y] = Normalize(newTopValue + RandRange(-noiseModifier, noiseModifier));

            var newLeftValue = x <= 0 ? (leftTopValue + middleValue + rightTopValue) / 3.0f
                                 : (leftTopValue + middleValue + rightTopValue + m_Map[x - (size / 2), y + (size / 2)]) / 4.0f;

            m_Map[x, y + (size / 2)] = Normalize(newLeftValue + RandRange(-noiseModifier, noiseModifier));
            
            var newRightValue = x >= maxIndex - size ? (leftBottomValue + middleValue + rightBottomValue) / 3.0f
                                               : (leftBottomValue + middleValue + rightBottomValue + m_Map[x + size + (size / 2), y + (size / 2)]) / 4.0f;

            m_Map[x + size, y + (size / 2)] = Normalize(newRightValue + RandRange(-noiseModifier, noiseModifier));

            var newBottomValue = y >= maxIndex - size ? (middleValue + rightTopValue + rightBottomValue) / 3.0f
                                               : (middleValue + rightTopValue + rightBottomValue + m_Map[x + (size / 2), y + size + (size / 2)]) / 4.0f;

            m_Map[x + (size / 2), y + size] = Normalize(newBottomValue + RandRange(-noiseModifier, noiseModifier));
        }

        private int RandRange(int minValue, int maxValue)
        {
            return minValue + m_Random.Next() * (maxValue - minValue);
        }

        private double RandRange(double minValue, double maxValue)
        {
            return minValue + m_Random.NextDouble() * (maxValue - minValue);
        }

        private float RandRange(float minValue, float maxValue)
        {
            return minValue + (float)m_Random.NextDouble() * (maxValue - minValue);
        }

        private float Normalize(float value)
        {
            return Math.Clamp(value, MinValue, MaxValue);
        }
        
        private bool IsPowerOf2(int value)
        {
            return value != 0 && (value & value - 1) == 0;
        }
    }
}
