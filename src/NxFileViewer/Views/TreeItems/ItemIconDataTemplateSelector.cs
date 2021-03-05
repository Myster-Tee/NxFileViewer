using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Views.TreeItems.Impl;
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

        public DataTemplate? CnmtContentEntryItemIconDataTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object? obj, DependencyObject container)
        {
            if (obj == null)
                return null;

            if (!(obj is IItemViewModel itemViewModel))
            {
                Debug.Fail($"{this.GetType().Name} expects objects of type {nameof(IItemViewModel)}!");
                return null;
            }

            switch (itemViewModel)
            {
                case XciItemViewModel _:
                    return XciItemIconDataTemplate;

                case XciPartitionItemViewModel _:
                    return XciPartitionItemIconDataTemplate;

                case PartitionFileEntryItemViewModel _:
                    return PartitionFileEntryItemIconDataTemplate;

                case NspItemViewModel _:
                    return NspItemIconDataTemplate;

                case SectionItemViewModel _:
                    return SectionItemIconDataTemplate;

                case DirectoryEntryItemViewModel directoryEntryItem when directoryEntryItem.DirectoryEntryType == DirectoryEntryType.Directory:
                    return FolderDirectoryEntryItemIconDataTemplate;

                case DirectoryEntryItemViewModel directoryEntryItem when directoryEntryItem.DirectoryEntryType == DirectoryEntryType.File:
                    return FileDirectoryEntryItemIconDataTemplate;

                case CnmtContentEntryItemViewModel:
                    return CnmtContentEntryItemIconDataTemplate;
                default:
                    Debug.Fail($"Icon data template is missing for item of type {itemViewModel.GetType().Name}!");
                    return null;
            }
        }

    }
}
