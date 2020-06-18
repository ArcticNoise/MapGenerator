
using System;
using System.Threading;
using MapGenerator.Noise;

namespace MapGenerator.Generators
{
    public class SimplexNoiseGenerator
    {
        private float[,] m_Map;

        private int m_Width;
        private int m_Height;

        private float m_MinValue;
        private float m_MaxValue;

        private readonly SimplexNoise m_Noise;

        public SimplexNoiseGenerator(int seed)
        {
            m_Noise = new SimplexNoise(seed);
        }

        public float[,] GenerateMap(int width, int height, float minValue, float maxValue, CancellationToken token = new CancellationToken())
        {
            m_Width = width;

            m_Height = height;

            m_MinValue = minValue;

            m_MaxValue = maxValue;

            GenerateMapInternal(token);

            return m_Map;
        }

        private void GenerateMapInternal(CancellationToken token)
        {
            m_Map = new float[m_Width, m_Height];

            for (var x = 0; x < m_Width; x++)
            {
                for (var y = 0; y < m_Height; y++)
                {
                    m_Map[x,y] =(float)NormalizeValue(m_Noise.Evaluate(x, y));
                    token.ThrowIfCancellationRequested();
                }

                token.ThrowIfCancellationRequested();
            }
        }

        private double NormalizeValue(double value)
        {
            var newValue = Math.Clamp(m_MaxValue/ 2 * (value + 1), m_MinValue, m_MaxValue);
            return newValue;
        }
    }
}