﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Emignatik.NxFileViewer.Model.TreeItems.Impl;
using Emignatik.NxFileViewer.Tools;
using LibHac.Fs;

namespace Emignatik.NxFileViewer.Services.BackgroundTask.RunnableImpl
{
    internal class SaveDirectoryRunnable : ISaveDirectoryRunnable
    {
        private readonly ILibHacFileSaver _libHacFileSaver;
        private readonly IFsSanitizer _fsSanitizer;
        private IEnumerable<DirectoryEntryItem>? _directoryEntryItems;
        private string? _targetDirectory;

        public SaveDirectoryRunnable(ILibHacFileSaver libHacFileSaver, IFsSanitizer fsSanitizer)
        {
            _libHacFileSaver = libHacFileSaver ?? throw new ArgumentNullException(nameof(libHacFileSaver));
            _fsSanitizer = fsSanitizer ?? throw new ArgumentNullException(nameof(fsSanitizer));
        }

        public bool SupportsCancellation => true;

        public bool SupportProgress => true;

        public void Setup(IEnumerable<DirectoryEntryItem> directoryEntryItems, string targetDirectory)
        {
            _directoryEntryItems = directoryEntryItems ?? throw new ArgumentNullException(nameof(directoryEntryItems));
            _targetDirectory = targetDirectory ?? throw new ArgumentNullException(nameof(targetDirectory));
        }

        public void Run(IProgressReporter progressReporter, CancellationToken cancellationToken)
        {
            if (_directoryEntryItems == null || _targetDirectory == null)
                throw new InvalidOperationException($"{nameof(Setup)} should be called first.");

            var elements = ListElements(_directoryEntryItems);

            var nbElements = elements.Count;

            var elementNum = 0;
            foreach (var (item, relativePath) in elements)
            {
                elementNum++;
                progressReporter.SetText($"Saving directory ({elementNum}/{nbElements})");

                var dstPath = Path.Combine(_targetDirectory, relativePath);

                switch (item.DirectoryEntryType)
                {
                    case DirectoryEntryType.Directory:
                        if (!Directory.Exists(dstPath))
                            Directory.CreateDirectory(dstPath);
                        break;
                    case DirectoryEntryType.File:
                        var file = item.GetFile();
                        _libHacFileSaver.Save(file, dstPath, cancellationToken, progressReporter.SetPercentage);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

        private List<ElementToCopy> ListElements(IEnumerable<DirectoryEntryItem> directoryEntryItem)
        {
            var elements = new List<ElementToCopy>();

            var remainingElementsToBrowse = new List<ElementToCopy>();

            foreach (var childItem in directoryEntryItem)
            {
                var relativePath = _fsSanitizer.SanitizeFileName(childItem.Name);
                remainingElementsToBrowse.Add(new ElementToCopy(childItem, relativePath));
            }

            while (remainingElementsToBrowse.Count > 0)
            {
                var element = remainingElementsToBrowse[0];
                remainingElementsToBrowse.RemoveAt(0);
                elements.Add(element);

                foreach (var childItem in element.Item.ChildItems)
                {
                    var relativePath = Path.Combine(element.RelativePath, _fsSanitizer.SanitizeFileName(childItem.Name));
                    remainingElementsToBrowse.Add(new ElementToCopy(childItem, relativePath));
                }
            }

            return elements;
        }

        private class ElementToCopy
        {

            public ElementToCopy(DirectoryEntryItem item, string relativePath)
            {
                Item = item;
                RelativePath = relativePath;
            }

            public DirectoryEntryItem Item { get; }

            public string RelativePath { get; }

            public void Deconstruct(out DirectoryEntryItem item, out string relativePath)
            {
                item = Item;
                relativePath = RelativePath;
            }
        }

    }

    public interface ISaveDirectoryRunnable : IRunnable
    {
        void Setup(IEnumerable<DirectoryEntryItem> directoryEntryItems, string targetDirectory);
    }
}