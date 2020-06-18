using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace MapGenerator.Selectors
{
    [ContentProperty(nameof(Templates))]
    public class DataTypeTemplateSelector : DataTemplateSelector
    {
        public Collection<DataTemplate> Templates { get; } = new Collection<DataTemplate>();

        public DataTemplate NewItemPlaceholderTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is null)
            {
                return base.SelectTemplate(null, container);
            }

            if (item == CollectionView.NewItemPlaceholder && NewItemPlaceholderTemplate != null)
            {
                return NewItemPlaceholderTemplate;
            }

            Type itemType = item.GetType();

            // Return the first template where (DataTemplate.DataType == itemType)
            var matchingTemplates = Templates.Where(template => itemType.Equals(template.DataType));
            if (matchingTemplates.FirstOrDefault() is { } matchingTemplate)
            {
                return matchingTemplate;
            }

            // Return the first template where DataTemplate.DataType is assignable from itemType
            var baseTypeTemplates = Templates.Where(template => template.DataType is Type dataType && dataType.IsAssignableFrom(itemType));
            if (baseTypeTemplates.FirstOrDefault() is { } baseTypeTemplate)
            {
                return baseTypeTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
