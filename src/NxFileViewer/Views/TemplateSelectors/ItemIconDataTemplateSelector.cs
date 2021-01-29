using System;
using System.Windows;
using System.Windows.Controls;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Views.TemplateSelectors
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

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is XciItem)
                return XciItemIconDataTemplate;

            if (item is XciPartitionItem)
                return XciPartitionItemIconDataTemplate;

            if (item is PartitionFileEntryItem)
                return PartitionFileEntryItemIconDataTemplate;

            if (item is NspItem)
                return NspItemIconDataTemplate;

            if (item is SectionItem)
                return SectionItemIconDataTemplate;

            if (item is DirectoryEntryItem directoryEntryItem)
            {
                if (directoryEntryItem.DirectoryEntryType == DirectoryEntryType.Directory)
                    return FolderDirectoryEntryItemIconDataTemplate;
                if (directoryEntryItem.DirectoryEntryType == DirectoryEntryType.File)
                    return FileDirectoryEntryItemIconDataTemplate;
            }

            return null;
        }

    }
}
