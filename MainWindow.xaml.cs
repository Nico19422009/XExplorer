using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace XExplorer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadDrives();
        }

        private void LoadDrives()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    var driveItem = CreateTreeItem(drive.Name, drive.Name);
                    FolderTreeView.Items.Add(driveItem);
                }
            }
        }

        private TreeViewItem CreateTreeItem(string header, string fullPath)
        {
            var item = new TreeViewItem
            {
                Header = header,
                Tag = fullPath
            };
            item.Expanded += Folder_Expanded;
            item.Items.Add(null); // Platzhalter für Lazy Loading
            return item;
        }

        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;
            if (item.Items.Count == 1 && item.Items[0] == null)
            {
                item.Items.Clear();
                try
                {
                    var directories = Directory.GetDirectories((string)item.Tag);
                    foreach (var directory in directories)
                    {
                        var dirItem = CreateTreeItem(System.IO.Path.GetFileName(directory), directory);
                        item.Items.Add(dirItem);
                    }
                }
                catch { }
            }
        }

        private void FolderTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (TreeViewItem)FolderTreeView.SelectedItem;
            if (item != null)
            {
                LoadFiles((string)item.Tag);
            }
        }

        private void LoadFiles(string path)
        {
            var files = new List<FileItem>();
            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    var info = new FileInfo(file);
                    files.Add(new FileItem
                    {
                        Name = info.Name,
                        Extension = info.Extension,
                        Size = (info.Length / 1024).ToString(),
                        LastModified = info.LastWriteTime.ToString()
                    });
                }
            }
            catch { }

            FileListView.ItemsSource = files;
        }
    }

    public class FileItem
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; }
        public string LastModified { get; set; }
    }
}
