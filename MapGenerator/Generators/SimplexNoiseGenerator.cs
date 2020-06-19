
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
        
        private readonly SimplexNoise m_Noise;

        public SimplexNoiseGenerator(int seed)
        {
            m_Noise = new SimplexNoise(seed);
        }

        public float[,] GenerateMap(int width, int height, CancellationToken token = new CancellationToken())
        {
            m_Width = width;

            m_Height = height;
            
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
                    m_Map[x, y] = m_Noise.CalcPixel2D(x, y);
                    token.ThrowIfCancellationRequested();
                }

                token.ThrowIfCancellationRequested();
            }
        }
    }
}