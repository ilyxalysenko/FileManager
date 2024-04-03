using FileSystemHelper;
using RootCat.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RootCat.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private ObservableCollection<NodeEntity> nodeChildrenListBoxContent;  // main Listbox content
        private NodeEntity selectedNodeChild;                                 // its selected item
        private ObservableCollection<NodeEntity> nodeHierarchy;

        private string keyWord; // for searching

        // Commands:
        private JumpToNodeChildren jumpToNodeChildren;
        private Search search;

        private bool inChildrenNodes;
        private int foldersChecked;

        public DataBaseHelper.AppContext db;

        // Localisation:
        private string localToDrives;
        private string localInChildNodes;
        private string localSearch;
        private string localFoldersChecked;

        public string LocalFolderPath { get; set; }

        public MainWindowViewModel()
        {
            Start();
        }

        private void Start()
        {
            LocalizeView();
            db = new DataBaseHelper.AppContext();
            db.Database.EnsureCreated();
            GetToDrives();
            NodeHierarchy = new ObservableCollection<NodeEntity>();
        }

        private void LocalizeView()
        {
            LocalInChildNodes = Resources.Localization.InChildNodes;
            LocalSearch = Resources.Localization.Search;
            LocalFoldersChecked = Resources.Localization.FoldersChecked;
        }

        public void GetToDrives()
        {
            var fsHandler = new FSHandler();
            var rootEntities = fsHandler.GetRootNodeEntities();
            Resources.Localization.LocalizeDriveInfos(rootEntities);
            NodeChildrenListBoxContent = new ObservableCollection<NodeEntity>(rootEntities);
        }

        public ObservableCollection<NodeEntity> NodeChildrenListBoxContent
        {
            get { return nodeChildrenListBoxContent; }
            set
            {
                nodeChildrenListBoxContent = value;
                OnPropertyChanged("NodeChildrenListBoxContent");
            }
        }

        public ObservableCollection<NodeEntity> NodeHierarchy
        {
            get { return nodeHierarchy; }
            set
            {
                nodeHierarchy = value;
                OnPropertyChanged("NodeHierarchy");
            }
        }

        public NodeEntity SelectedNodeChild
        {
            get { return selectedNodeChild; }
            set
            {
                selectedNodeChild = value;
                OnPropertyChanged("SelectedNodeChild");
            }
        }

        public string KeyWord
        {
            get { return keyWord; }
            set
            {
                keyWord = value;
                OnPropertyChanged("KeyWord");
            }
        }

        public int FoldersChecked
        {
            get { return foldersChecked; }
            set
            {
                foldersChecked = value;
                OnPropertyChanged("FoldersChecked");
            }
        }

        public bool InChildrenNodes
        {
            get { return inChildrenNodes; }
            set
            {
                inChildrenNodes = value;
                OnPropertyChanged("InChildrenNodes");
            }
        }

        public string LocalToDrives
        {
            get { return localToDrives; }
            set
            {
                localToDrives = value;
                OnPropertyChanged("LocalToDrives");
            }
        }

        public string LocalInChildNodes
        {
            get { return localInChildNodes; }
            set
            {
                localInChildNodes = value;
                OnPropertyChanged("LocalInChildNodes");
            }
        }

        public string LocalSearch
        {
            get { return localSearch; }
            set
            {
                localSearch = value;
                OnPropertyChanged("LocalSearch");
            }
        }

        public string LocalFoldersChecked
        {
            get { return localFoldersChecked; }
            set
            {
                localFoldersChecked = value;
                OnPropertyChanged("LocalFoldersChecked");
            }
        }

        public Search Search
        {
            get { return search ?? (search = new Search(this)); }
        }

        public JumpToNodeChildren JumpToNodeChildren
        {
            get { return jumpToNodeChildren ?? (jumpToNodeChildren = new JumpToNodeChildren(this)); }
        }
    }
}
