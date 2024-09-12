using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;

namespace Wpf_brickstore
{
    public partial class MainWindow : Window
    {
        private List<Item> items = new List<Item>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Mappák|*.*",
                CheckFileExists = false,
                CheckPathExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = Path.GetDirectoryName(dialog.FileName);
                var bsxFiles = Directory.GetFiles(selectedPath, "*.bsx");

                if (bsxFiles.Length > 0)
                {
                    FileListBox.ItemsSource = bsxFiles;
                }
                else
                {
                    System.Windows.MessageBox.Show("Nincsenek BSX fájlok a kiválasztott mappában.");
                }
            }
        }
        private void FileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileListBox.SelectedItem != null)
            {
                string selectedFile = FileListBox.SelectedItem.ToString();
                try
                {
                    LoadBSXFile(selectedFile);
                    Caller();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Hiba a fájl beolvasásakor: {ex.Message}");
                }
            }
        }

        private void LoadBSXFile(string filePath)
        {
            XDocument xaml = XDocument.Load(filePath);
            items = xaml.Descendants("Item")
                .Select(x => new Item
                {
                    ItemID = x.Element("ItemID").Value,
                    ItemName = x.Element("ItemName").Value,
                    CategoryName = x.Element("CategoryName").Value,
                    ColorName = x.Element("ColorName").Value,
                    Qty = int.Parse(x.Element("Qty").Value ?? "0")
                })
                .ToList();
        }

        private void Caller()
        {
            string itemIdFilter = ItemIdFilter.Text.ToLower();
            string itemNameFilter = ItemNameFilter.Text.ToLower();
            string categoryNameFilter = CategoryNameFilter.SelectedItem?.ToString().ToLower() ?? "összes kategória";

            var filteredItems = items.Where(x =>
                (string.IsNullOrEmpty(itemIdFilter) || x.ItemID.ToLower().Contains(itemIdFilter)) &&
                (string.IsNullOrEmpty(itemNameFilter) || x.ItemName.ToLower().Contains(itemNameFilter)) &&
                (categoryNameFilter == "összes kategória" || x.CategoryName.ToLower().Contains(categoryNameFilter))
            ).ToList();

            var uniqueCategories = filteredItems
                .Select(x => x.CategoryName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            uniqueCategories.Insert(0, "Összes kategória");
            CategoryNameFilter.ItemsSource = uniqueCategories;

            if (CategoryNameFilter.SelectedIndex == -1)
            {
                CategoryNameFilter.SelectedIndex = 0;
            }
            ItemDataGrid.ItemsSource = filteredItems;
        }

        private void ItemIdFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            Caller();
        }

        private void ItemNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            Caller();
        }

        private void CategoryNameFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Caller();
        }
    }

    public class Item
    {
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string CategoryName { get; set; }
        public string ColorName { get; set; }
        public int Qty { get; set; }
    }
}
