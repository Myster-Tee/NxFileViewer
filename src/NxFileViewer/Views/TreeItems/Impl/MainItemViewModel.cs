using System;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Views.ObjectPropertyViewer;
using LibHac.Common;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl
{
    public class MainItemViewModel : DirectoryEntryItemViewModel
    {
        private readonly MainItem _mainItem;

        public MainItemViewModel(MainItem mainItem, IServiceProvider serviceProvider)
            : base(mainItem, serviceProvider)
        {
            _mainItem = mainItem;
        }

        [PropertyView(DescriptionLocalizationKey = "MainModuleIdTooltip")]
        public Buffer32 ModuleId => _mainItem.ModuleId;
    }
}