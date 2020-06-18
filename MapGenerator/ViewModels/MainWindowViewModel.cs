using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapGenerator.Commands;
using MapGenerator.Extensions;
using MapGenerator.Generators;
using MapGenerator.ViewModels.Interfaces;
using Microsoft.Win32;

namespace MapGenerator.ViewModels
{
    public class MainWindowViewModel : AbstractViewModel
    {
        private float[,] m_Map;
        private IGeneratorViewModel m_Generator;

        public IGeneratorViewModel Generator
        {
            get => m_Generator;
            set => SetProperty(ref m_Generator, value);
        }

        private bool m_IsGenerating;
        public bool IsGenerating
        {
            get => m_IsGenerating;
            set => SetProperty(ref m_IsGenerating, value);
        }

        private int m_Width;
        public int Width
        {
            get => m_Width;
            set => SetProperty(ref m_Width, value);
        }

        private int m_Height;
        public int Height
        {
            get => m_Height;
            set => SetProperty(ref m_Height, value);
        }

        private BitmapImage m_Image;
        public BitmapImage Image
        {
            get => m_Image;
            set => SetProperty(ref m_Image, value);
        }

        public IAsyncCommand GenerateMapAsyncCommand { get; }
        public ICommand SaveMapCommand => new DelegateCommand(obj => SaveMap(), obj => m_Image != null && !m_IsGenerating);
        
        public MainWindowViewModel()
        {
            Generator = new DiamondSquareGeneratorViewModel();
            GenerateMapAsyncCommand = AsyncCommandFactory.Create(GenerateMapAsync, obj => !m_IsGenerating);
        }

        private async Task GenerateMapAsync(CancellationToken token = new CancellationToken())
        {
            await Task.Run(() => GenerateMap(token), token);
        }

        private void GenerateMap(CancellationToken token)
        {
            ResetValues();
            IsGenerating = true;

            try
            {
                m_Map = Generator.GenerateMap(token);
                Image = ImageGenerator.GenerateGreyscale(m_Map, token);
                Width = m_Map.GetLength(0);
                Height = m_Map.GetLength(1);
            }
            catch(OperationCanceledException)
            {
                ResetValues();
            }
            finally
            {
                IsGenerating = false;
            }
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
            Width = 0;
            Height = 0;
            m_Map = null;
            Image = null;
        }
    }
}
