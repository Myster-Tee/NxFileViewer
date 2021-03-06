﻿using System;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;

namespace Emignatik.NxFileViewer.Model.Overview
{
    public class CnmtContainer 
    {

        public CnmtContainer(CnmtItem cnmtItem)
        {
            CnmtItem = cnmtItem ?? throw new ArgumentNullException(nameof(cnmtItem));
        }

        public CnmtItem CnmtItem { get; }

        /// <summary>
        /// Get the NACP container referenced by this CNMT container if found
        /// </summary>
        public NacpContainer? NacpContainer { get; set; }

    }
}