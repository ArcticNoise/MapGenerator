using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapGenerator.Commands;
using MapGenerator.Extensions;
using MapGenerator.Noise;
using Microsoft.Win32;

namespace MapGenerator.ViewModels
{
    public class MainWindowViewModel : AbstractViewModel
    {
        private const int Seed = 666;

        private const float MinHeightValue = 0;
        private const float MaxHeightValue = 255;

        public int MinPowerValue => 2;
        public int MaxPowerValue => 14;
        
        private float[,] m_Map;
        private readonly DiamondSquareGenerator m_Generator;

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

        private void RecalculateExpectedImageSize()
        {
            var value = (int)Math.Pow(2, m_PowerOfTwo) + 1;
            ExpectedImageSize = value;
        }

        private int m_MapSideSize;
        public int MapSideSize
        {
            get => m_MapSideSize;
            set => SetProperty(ref m_MapSideSize, value);
        }

        private int m_Roughness;
        public int Roughness
        {
            get => m_Roughness;
            set => SetProperty(ref m_Roughness, value);
        }

        private bool m_IsGenerating;
        public bool IsGenerating
        {
            get => m_IsGenerating;
            set => SetProperty(ref m_IsGenerating, value);
        }

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

        public string ExpectedImageSizeString => $"({ExpectedImageSize}x{ExpectedImageSize})";

        private BitmapImage m_Image;
        public BitmapImage Image
        {
            get => m_Image;
            set => SetProperty(ref m_Image, value);
        }

        public IAsyncCommand GenerateMapAsyncCommand => AsyncCommandFactory.Create(GenerateMapAsync, obj => !m_IsGenerating);

        public ICommand SaveMapCommand => new DelegateCommand(obj => SaveMap(), obj => m_Image != null && !m_IsGenerating);
        
        public MainWindowViewModel()
        {
            m_Generator = new DiamondSquareGenerator(Seed);
            
            //Start value
            PowerOfTwo = 8;
        }

        private async Task GenerateMapAsync()
        {
            await Task.Run(GenerateMap);
        }

        private void GenerateMap()
        {
            try
            {
                ResetValues();
                IsGenerating = true;

                m_Map = m_Generator.GenerateMap(PowerOfTwo, MinHeightValue, MaxHeightValue, Roughness);
                GenerateBitmap();
            }
            finally
            {
                IsGenerating = false;
            }
        }

        private void GenerateBitmap()
        {
            var size = m_Map.GetLength(0);
            var bitmap = new Bitmap(size, size);
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < size; y++)
                {
                    var height = m_Map[x, y];
                    var pixelColorValue = (int)height;
                    var pixelColor = Color.FromArgb(pixelColorValue, pixelColorValue, pixelColorValue);
                    bitmap.SetPixel(x, y, pixelColor);
                }
            }

            MapSideSize = size;
            Image = BitmapToImageSource(bitmap);
        }

        private void SaveMap()
        {
            var saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Images (*.png)|*.png",
                CheckPathExists = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    Image.Save(saveFileDialog.FileName);
                    MessageBox.Show("Image successfully saved", "Saving image", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"Failed to save file.\nReason: {exception.Message}\nStackTrace: {exception.StackTrace}", "Saving image", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ResetValues()
        {
            MapSideSize = 0;
            Image = null;
        }

        private static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                var bImage = new BitmapImage();
                bImage.BeginInit();
                bImage.StreamSource = memory;
                bImage.CacheOption = BitmapCacheOption.OnLoad;
                bImage.EndInit();

                bImage.Freeze();
                
                return bImage;
            }
        }
    }
}
