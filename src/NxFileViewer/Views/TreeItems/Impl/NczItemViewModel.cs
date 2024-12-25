using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public class NczItemViewModel : NcaItemViewModel
{
    public NczItemViewModel(NczItem nczItem, IServiceProvider serviceProvider) : base(nczItem, serviceProvider)
    {
    }
}