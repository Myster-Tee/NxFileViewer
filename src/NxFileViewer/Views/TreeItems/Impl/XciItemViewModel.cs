﻿using System;
using Emignatik.NxFileViewer.Models.TreeItems.Impl;

namespace Emignatik.NxFileViewer.Views.TreeItems.Impl;

public class XciItemViewModel : ItemViewModel
{
    public XciItemViewModel(XciItem xciItem, IServiceProvider serviceProvider)
        : base(xciItem, serviceProvider)
    {

    }
}