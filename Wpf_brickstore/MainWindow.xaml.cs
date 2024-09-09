using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Xml.Linq;

namespace Wpf_brickstore
{
    public partial class MainWindow : Window
    {
        private List<Item> items = new List<Item>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "BSX Files (*.bsx)|*.bsx|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    LoadBSXFile(openFileDialog.FileName);
                    Caller();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a fájl beolvasásakor: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("A fájl nem található, vagy nem lett kiválasztva.");
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
                    Qty = int.Parse(x.Element("Qty")?.Value ?? "0")
                })
                .ToList();
        }
        private void Caller()
        {
            string itemIdFilter = ItemIdFilter.Text.ToLower();
            string itemNameFilter = ItemNameFilter.Text.ToLower();
            string categoryNameFilter = CategoryNameFilter.SelectedItem?.ToString().ToLower() ?? "";
            var filteredItems = items.Where(x =>
                (string.IsNullOrEmpty(itemIdFilter) || x.ItemID.ToLower().Contains(itemIdFilter)) &&
                (string.IsNullOrEmpty(itemNameFilter) || x.ItemName.ToLower().Contains(itemNameFilter)) &&
                (string.IsNullOrEmpty(categoryNameFilter) || x.CategoryName.ToLower().Contains(categoryNameFilter))
            ).ToList();
            var uniqueCategories = filteredItems
                .Select(x => x.CategoryName)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            uniqueCategories.Insert(0, "Összes Kategória");
            CategoryNameFilter.ItemsSource = uniqueCategories;
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
