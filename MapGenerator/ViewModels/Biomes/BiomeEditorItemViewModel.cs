using System.Drawing;

namespace MapGenerator.ViewModels.Biomes
{
    public class BiomeEditorItemViewModel : AbstractViewModel
    {
        private Brush m_ColorBrush;
        public Brush ColorBrush
        {
            get => m_ColorBrush;
            set => SetProperty(ref m_ColorBrush, value);
        }

        private string m_ColorValue;
        public string ColorValue
        {
            get => m_ColorValue;
            set
            {
                if (value != m_ColorValue)
                {
                    m_ColorValue = value;
                    OnPropertyChanged();
                    OnColorValueChanged();
                }
            }
        }

        private int m_Height;
        public int Height
        {
            get => m_Height;
            set
            {
                if (value != m_Height)
                {
                    m_Height = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnColorValueChanged()
        {

        }
    }
}
