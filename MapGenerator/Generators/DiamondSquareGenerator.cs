using System;
using System.Threading;

namespace MapGenerator.Generators
{
    public class DiamondSquareGenerator
    {
        private int m_Size;
        private float[,] m_Map;
        private Random m_Random;
        private float m_Roughness;

        private readonly int m_Seed;

        private float m_MinValue;
        private float m_MaxValue;

        private const int MaxPowerOfTwo = 14;

        public DiamondSquareGenerator(int randomSeed)
        {
            m_Seed = randomSeed;
            ResetRandom();
        }
        
        public float[,] GenerateMap(int n, float minValue = 0, float maxValue = 1, float roughness = 2, CancellationToken token = new CancellationToken())
        {
            if (n > MaxPowerOfTwo)
            {
                throw new ArgumentException($"n should not be more then {MaxPowerOfTwo} or it can cause an overflow.");
            }

            m_Size = (int)Math.Pow(2, n) + 1;

            m_MinValue = minValue;

            m_MaxValue = maxValue;

            m_Roughness = roughness;

            GenerateMapInternal(token);

            return m_Map;
        }

        public void ResetRandom()
        {
            m_Random = new Random(m_Seed);
        }

        private void GenerateMapInternal(CancellationToken token)
        {
            m_Map = new float[m_Size, m_Size];

            var startValue = RandRange(m_MinValue, m_MaxValue);

            var maxIndex = m_Size - 1;

            m_Map[0, 0] = startValue;
            m_Map[0, maxIndex] = startValue;
            m_Map[maxIndex, 0] = startValue;
            m_Map[maxIndex, maxIndex] = startValue;

            for (var i = maxIndex; i > 1; i /= 2)
            {
                var noiseModifier = (m_MaxValue - m_MinValue) * m_Roughness * ((float)i / maxIndex);

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
                        SquareStep(x, y, i, maxIndex, noiseModifier);
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
        
        private void SquareStep(int x, int y, int size, int maxIndex, float noiseModifier)
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
            return Math.Clamp(value, m_MinValue, m_MaxValue);
        }
        
        private bool IsPowerOf2(int value)
        {
            return value != 0 && (value & value - 1) == 0;
        }
    }
}
