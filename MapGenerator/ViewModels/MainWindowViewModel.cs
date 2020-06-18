using System;
using System.Drawing;
using System.IO;
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

        private int m_MapSideSize;
        public int MapSideSize
        {
            get => m_MapSideSize;
            set => SetProperty(ref m_MapSideSize, value);
        }

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
            Generator = new DiamondSquareGeneratorViewModel();
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

                m_Map = m_Generator.GenerateMap();
                Image = ImageGenerator.GenerateGreyscale(m_Map);
                MapSideSize = m_Map.GetLength(0);
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
            MapSideSize = 0;
            Image = null;
        }
    }
}
