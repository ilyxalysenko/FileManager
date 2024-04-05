using DataBaseHelper.Entities;
using FileSystemHelper;
using Microsoft.Win32;
using RootCat.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RootCat.ViewModels
{
    public class JumpToNodeChildren : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private MainWindowViewModel _vm;
        public JumpToNodeChildren(MainWindowViewModel vm)
        {
            _vm = vm;
        }
        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (_vm.SelectedNodeChild == null)
                return;

            switch (_vm.SelectedNodeChild.Type)
            {
                case NodeType.Directory:
                case NodeType.Drive:
                    OpenFolder(); break;
                case NodeType.File: OpenFile(); break;
            }
        }

        public void OpenFolder()
        {
            var fsHandler = new FSHandler();
            var newListItems = fsHandler.GetNodeEntitiesChildren(_vm.SelectedNodeChild.FullPath);

            while (_vm.NodeHierarchy.Count >= 4)
            {
                _vm.NodeHierarchy.RemoveAt(0);
            }
            _vm.NodeHierarchy.Add(_vm.SelectedNodeChild);


            foreach (var item in newListItems)
            {
                switch (item.Type)
                {
                    case NodeType.Directory:Resources.Localization.LocalizeDirectoryInfo(item); break;
                    case NodeType.Drive:Localization.LocalizeDriveInfo(item); break;
                    case NodeType.File: Localization.LocalizeFileInfo(item); break;
                }
            }

            _vm.NodeChildrenListBoxContent = new ObservableCollection<NodeEntity>(newListItems);
        }

        public void OpenFile()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileName = _vm.SelectedNodeChild.FullPath;

                using (Stream str = openFileDialog.OpenFile())
                {
                    Process.Start(new ProcessStartInfo(_vm.SelectedNodeChild.FullPath) { UseShellExecute = true });
                }
                try
                {
                    MakeDbRecord();
                }
                catch (Exception e) { }
            }
            catch (SecurityException ex) { }
        }

        public void MakeDbRecord()
        {
            _vm.db.Visits.Add(new Visit(_vm.SelectedNodeChild.Name, DateTime.Now));
            _vm.db.SaveChanges();
        }
    }

    public class Search : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private MainWindowViewModel _vm;

        public Search(MainWindowViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            if (_vm.InChildrenNodes)
                new Thread(FindByName).Start();
            else
                SortMainListBoxContent();
        }

        public void FindByName()
        {
            if (_vm.NodeChildrenListBoxContent == null || String.IsNullOrEmpty(_vm.KeyWord))
                return;

            if (_vm.SelectedNodeChild == null)
            {
                var newList = new List<NodeEntity>();
                foreach (var node in _vm.NodeChildrenListBoxContent)
                {
                    newList.AddRange(NodeSeeker.FindByName(node, _vm.KeyWord, SetCheckedFoldersInfo));
                }

                _vm.NodeChildrenListBoxContent = new ObservableCollection<NodeEntity>(newList);
            }
            else
            {
                _vm.NodeChildrenListBoxContent = new ObservableCollection<NodeEntity>(
               NodeSeeker.FindByName(_vm.SelectedNodeChild, _vm.KeyWord, SetCheckedFoldersInfo));
            }

            NodeSeeker.CheckedFolders = 0;
        }

        public int SetCheckedFoldersInfo(int num)
        {
            _vm.FoldersChecked = num;
            return 0;
        }

        public void SortMainListBoxContent()
        {
            var sortedListBoxContent = new List<NodeEntity>();

            foreach (var item in _vm.NodeChildrenListBoxContent)
            {
                if (item.Name.ToUpper().Contains(_vm.KeyWord.ToUpper()))
                {
                    sortedListBoxContent.Add(item);
                }
            }

            _vm.NodeChildrenListBoxContent = new ObservableCollection<NodeEntity>(sortedListBoxContent);
        }
    }

    /// <summary>
    /// Jump to drives
    /// </summary>
    public class ToDrives : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        private MainWindowViewModel _vm;

        public ToDrives(MainWindowViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _vm.GetToDrives();
            _vm.NodeHierarchy = new ObservableCollection<NodeEntity>();
        }
    }
}
