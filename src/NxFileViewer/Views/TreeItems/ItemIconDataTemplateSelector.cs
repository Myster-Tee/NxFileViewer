using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Views.TreeItems
{
    public class ItemIconDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? XciItemIconDataTemplate { get; set; }

        public DataTemplate? XciPartitionItemIconDataTemplate { get; set; }

        public DataTemplate? NspItemIconDataTemplate { get; set; }

        public DataTemplate? SectionItemIconDataTemplate { get; set; }

        public DataTemplate? PartitionFileEntryItemIconDataTemplate { get; set; }

        public DataTemplate? FolderDirectoryEntryItemIconDataTemplate { get; set; }

        public DataTemplate? FileDirectoryEntryItemIconDataTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object? obj, DependencyObject container)
        {
            if (obj == null)
                return null;

            if (!(obj is IItemViewModel itemViewModel))
            {
                Debug.Fail($"{this.GetType().Name} expects objects of type {nameof(IItemViewModel)}!");
                return null;
            }

            var item = itemViewModel.AttachedItem;
            switch (item)
            {
                case XciItem _:
                    return XciItemIconDataTemplate;

                case XciPartitionItem _:
                    return XciPartitionItemIconDataTemplate;

                case PartitionFileEntryItem _:
                    return PartitionFileEntryItemIconDataTemplate;

                case NspItem _:
                    return NspItemIconDataTemplate;

                case SectionItem _:
                    return SectionItemIconDataTemplate;

                case DirectoryEntryItem directoryEntryItem when directoryEntryItem.DirectoryEntryType == DirectoryEntryType.Directory:
                    return FolderDirectoryEntryItemIconDataTemplate;

                case DirectoryEntryItem directoryEntryItem when directoryEntryItem.DirectoryEntryType == DirectoryEntryType.File:
                    return FileDirectoryEntryItemIconDataTemplate;

                default:
                    Debug.Fail($"Icon data template is missing for item of type {item.GetType().Name}!");
                    return null;
            }
        }

    }
}
