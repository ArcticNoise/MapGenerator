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

        private float m_Frequency;

        private float m_Octaves;

        private float m_Redistribution;
        
        private readonly SimplexNoise m_Noise;

        public SimplexNoiseGenerator(int seed)
        {
            m_Noise = new SimplexNoise(seed);
        }

        public float[,] GenerateMap(int width, int height, float frequency = 1, int octaves = 1, float redistribution = 1, CancellationToken token = new CancellationToken())
        {
            m_Width = width;

            m_Height = height;

            m_Frequency = frequency;

            m_Octaves = octaves;

            m_Redistribution = redistribution;
            
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
                    var elevation = 0d;
                    var noiseMod = 1d;

                    var nx = (double)x / m_Width - 0.5d;
                    var ny = (double)y / m_Width - 0.5d;

                    for (var i = 0; i < m_Octaves; i++)
                    {
                        elevation += noiseMod * m_Noise.Evaluate(nx * (1 / noiseMod) * m_Frequency, ny * (1 / noiseMod) * m_Frequency);
                        noiseMod /= 2;
                    }

                    var clampedElevation = Math.Clamp(elevation, -1, 1);

                    var normalizedElevation = (clampedElevation + 1) / 2;

                    var redistributedElevation = (float)Math.Pow(normalizedElevation, m_Redistribution);

                    m_Map[x, y] = redistributedElevation;
                    token.ThrowIfCancellationRequested();
                }

                token.ThrowIfCancellationRequested();
            }
        }
    }
}