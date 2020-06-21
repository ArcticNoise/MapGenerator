using System;
using System.Threading;
using MapGenerator.Generators.GenerationData;
using MapGenerator.Noise;

namespace MapGenerator.Generators
{
    public class SimplexNoiseGenerator
    {
        private float[,] m_Map;
        
        private SimplexNoise m_Noise;

        public float[,] GenerateMap(SimplexNoiseGenerationData generationData, CancellationToken token)
        {
            GenerateMapInternal(generationData, token);

            return m_Map;
        }

        private void GenerateMapInternal(SimplexNoiseGenerationData generationData, CancellationToken token)
        {
            m_Noise = new SimplexNoise(generationData.Seed);

            m_Map = new float[generationData.Width, generationData.Height];

            for (var x = 0; x < generationData.Width; x++)
            {
                for (var y = 0; y < generationData.Height; y++)
                {
                    var elevation = 0d;
                    var noiseMod = 1d;

                    var nx = (double)x / generationData.Width - 0.5d;
                    var ny = (double)y / generationData.Height - 0.5d;

                    for (var i = 0; i < generationData.Octaves; i++)
                    {
                        elevation += noiseMod * m_Noise.Evaluate(nx * (1 / noiseMod) * generationData.Frequency, ny * (1 / noiseMod) * generationData.Frequency);
                        noiseMod /= 2;
                    }

                    var clampedElevation = Math.Clamp(elevation, -1, 1);

                    var normalizedElevation = (clampedElevation + 1) / 2;

                    var redistributedElevation = (float)Math.Pow(normalizedElevation, generationData.Redistribution);

                    m_Map[x, y] = redistributedElevation;
                    token.ThrowIfCancellationRequested();
                }

                token.ThrowIfCancellationRequested();
            }
        }
    }
}