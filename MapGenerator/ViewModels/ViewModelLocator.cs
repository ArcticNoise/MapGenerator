using System;
using System.ComponentModel;
using System.Windows;

namespace MapGenerator.ViewModels
{
    /// <summary>
    /// Locates ViewModel placed near the view and appropriate to naming rule: same name as view + model in the end or ViewModel if there is no view at the end of the control name. 
    /// </summary>
    public static class ViewModelLocator
    {
        public static bool GetAutoWireViewModel(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoWireViewModelProperty);
        }

        public static void SetAutoWireViewModel(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoWireViewModelProperty, value);
        }

        public static readonly DependencyProperty AutoWireViewModelProperty =
            DependencyProperty.RegisterAttached("AutoWireViewModel",
                typeof(bool), typeof(ViewModelLocator), new PropertyMetadata(false, AutoWireViewModelChanged));

        private static void AutoWireViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
                return;

            var frameworkElement = d as FrameworkElement;
            if (frameworkElement == null)
            {
                throw new NullReferenceException("AutoWire was attached not to the FrameworkElement");
            }

            var viewType = d.GetType();

            var viewTypeName = viewType.FullName;
            if (viewTypeName == null)
            {
                throw new NullReferenceException("AutoWire failed to get attached object full type name.");
            }

            var viewModelNearViewTypeName = viewTypeName + (viewTypeName.EndsWith("View") ? "Model" : "ViewModel");
            var viewModelInViewModelFolderTypeName = viewModelNearViewTypeName.Replace(".Views.", ".ViewModels.");

            var viewAssembly = viewType.Assembly;

            var viewModelNearViewType = viewAssembly.GetType(viewModelNearViewTypeName);
            var viewModelInViewModelFolderType = viewAssembly.GetType(viewModelInViewModelFolderTypeName);

            object viewModel;
            if (viewModelNearViewType != null)
            {
                viewModel = Activator.CreateInstance(viewModelNearViewType);
            }
            else if (viewModelInViewModelFolderType != null)
            {
                viewModel = Activator.CreateInstance(viewModelInViewModelFolderType);
            }
            else
            {
                throw new NullReferenceException($"AutoWire failed to find viewmodel of type '{viewModelNearViewTypeName}' and type '{viewModelInViewModelFolderTypeName}' inside '{viewAssembly}' assembly");
            }

            frameworkElement.DataContext = viewModel;
        }
    }
}
