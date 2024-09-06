using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                    ItemDataGrid.ItemsSource = items;
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
            try
            {
                XDocument xaml = XDocument.Load(filePath);
                items = xaml.Descendants("Item")
                    .Select(x => new Item
                    {
                        ItemID = x.Element("ItemID")?.Value,
                        ItemName = x.Element("ItemName")?.Value,
                        CategoryName = x.Element("CategoryName")?.Value,
                        ColorName = x.Element("ColorName")?.Value,
                        Qty = int.Parse(x.Element("Qty")?.Value ?? "0")
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("A fájl formátuma nem megfelelő.");
            }
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            string itemIdFilter = ItemIdFilter.Text.ToLower();
            string itemNameFilter = ItemNameFilter.Text.ToLower();
            string categoryNameFilter = CategoryNameFilter.Text.ToLower();

            var filteredItems = items.Where(x =>
                (string.IsNullOrEmpty(itemIdFilter) || x.ItemID.ToLower().Contains(itemIdFilter)) &&
                (string.IsNullOrEmpty(itemNameFilter) || x.ItemName.ToLower().Contains(itemNameFilter)) &&
                (string.IsNullOrEmpty(categoryNameFilter) || x.CategoryName.ToLower().Contains(categoryNameFilter))
            ).ToList();

            ItemDataGrid.ItemsSource = filteredItems;
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