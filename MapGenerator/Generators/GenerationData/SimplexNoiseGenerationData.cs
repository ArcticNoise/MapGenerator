namespace MapGenerator.Generators.GenerationData
{
    public struct SimplexNoiseGenerationData
    {
        public int Seed { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Frequency { get; set; }
        public int Octaves { get; set; }
        public float Redistribution { get; set; }
    }
}
