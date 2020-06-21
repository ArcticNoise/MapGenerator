using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using MapGenerator.Commands;

namespace MapGenerator.ViewModels.Biomes
{
    public class BiomeEditorViewModel : AbstractViewModel
    {
        public const int MinHeightValue = 0;
        public const int MaxHeightValue = 100;

        public ObservableCollection<BiomeEditorItemViewModel> Items { get; }

        public ICommand CreateBiomeCommand => new DelegateCommand(CreateBiome);
        public ICommand DeleteBiomeCommand => new DelegateCommand<BiomeEditorItemViewModel>(DeleteBiome);
        public ICommand MoveBiomeUpCommand => new DelegateCommand<BiomeEditorItemViewModel>(MoveBiomeUp, CanMoveBiomeUp);
        public ICommand MoveBiomeDownCommand => new DelegateCommand<BiomeEditorItemViewModel>(MoveBiomeDown, CanMoveDownBiome);

        public BiomeEditorViewModel()
        {
            Items = new ObservableCollection<BiomeEditorItemViewModel>();
            var source = (IEditableCollectionView)CollectionViewSource.GetDefaultView(Items);
            source.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
        }

        private void CreateBiome(object obj)
        {
            Items.Add(new BiomeEditorItemViewModel());
        }

        private void DeleteBiome(BiomeEditorItemViewModel item)
        {
            Items.Remove(item);
        }

        private bool CanMoveBiomeUp(BiomeEditorItemViewModel item)
        {
            return Items.IndexOf(item) != 0;
        }

        private void MoveBiomeUp(BiomeEditorItemViewModel item)
        {
            var currentIdx = Items.IndexOf(item);
            Items.RemoveAt(currentIdx);
            Items.Insert(currentIdx - 1, item);
        }

        private bool CanMoveDownBiome(BiomeEditorItemViewModel item)
        {
            return Items.IndexOf(item) < Items.Count - 1;
        }

        private void MoveBiomeDown(BiomeEditorItemViewModel item)
        {
            var currentIdx = Items.IndexOf(item);
            Items.RemoveAt(currentIdx);
            Items.Insert(currentIdx + 1, item);
        }
    }
}
