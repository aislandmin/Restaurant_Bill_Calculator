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

namespace RestaurantBillCalculator
{
    public partial class MainWindow : Window
    {
        private List<MenuItem> selectedItems = new List<MenuItem>();
        private List<MenuItem> menuItems = new List<MenuItem>();

        public MainWindow()
        {
            InitializeComponent();
            //Prepare Menu items for ComboBox options
            LoadMenuItemData(); 
            PopulateCategoryComboBoxes();
        }

        private void LoadMenuItemData()
        {
            menuItems = new List<MenuItem>
            {
                //Beverage
                new MenuItem { Name = "Soda", Category = "Beverage", Price = 1.95 },
                new MenuItem { Name = "Tea", Category = "Beverage", Price = 1.50 },
                new MenuItem { Name = "Coffee", Category = "Beverage", Price = 1.25 },
                new MenuItem { Name = "Mineral Water", Category = "Beverage", Price = 2.95 },
                new MenuItem { Name = "Juice", Category = "Beverage", Price = 2.50 },
                new MenuItem { Name = "Milk", Category = "Beverage", Price = 1.50 },

                // Appetizers
                new MenuItem { Name = "Buffalo Wings", Category = "Appetizer", Price = 5.95 },
                new MenuItem { Name = "Buffalo Fingers", Category = "Appetizer", Price = 6.95 },
                new MenuItem { Name = "Potato Skins", Category = "Appetizer", Price = 8.95 },
                new MenuItem { Name = "Nachos", Category = "Appetizer", Price = 8.95 },
                new MenuItem { Name = "Mushroom Caps", Category = "Appetizer", Price = 10.95 },
                new MenuItem { Name = "Shrimp Cocktail", Category = "Appetizer", Price = 12.95 },
                new MenuItem { Name = "Chips and Salsa", Category = "Appetizer", Price = 6.95 },

                // Main Course
                new MenuItem { Name = "Seafood Alfredo", Category = "Main Course", Price = 15.95 },
                new MenuItem { Name = "Chicken Alfredo", Category = "Main Course", Price = 13.95 },
                new MenuItem { Name = "Chicken Piccata", Category = "Main Course", Price = 13.95 },
                new MenuItem { Name = "Turkey Club", Category = "Main Course", Price = 11.95 },
                new MenuItem { Name = "Lobster Pie", Category = "Main Course", Price = 19.95 },
                new MenuItem { Name = "Prime Rib", Category = "Main Course", Price = 20.95 },
                new MenuItem { Name = "Shrimp Scampi", Category = "Main Course", Price = 18.95 },
                new MenuItem { Name = "Turkey Dinner", Category = "Main Course", Price = 13.95 },
                new MenuItem { Name = "Stuffed Chicken", Category = "Main Course", Price = 14.95 },

                // Desserts
                new MenuItem { Name = "Apple Pie", Category = "Dessert", Price = 5.95 },
                new MenuItem { Name = "Sundae", Category = "Dessert", Price = 3.95 },
                new MenuItem { Name = "Carrot Cake", Category = "Dessert", Price = 5.95 },
                new MenuItem { Name = "Mud Pie", Category = "Dessert", Price = 4.95 },
                new MenuItem { Name = "Apple Crisp", Category = "Dessert", Price = 5.95 }
            };
        }

        private void PopulateCategoryComboBoxes()
        {
            cmbBeverage.ItemsSource = menuItems.Where(item => item.Category == "Beverage").ToList();
            cmbBeverage.DisplayMemberPath = "Name";

            cmbAppetizer.ItemsSource = menuItems.Where(item => item.Category == "Appetizer").ToList();
            cmbAppetizer.DisplayMemberPath = "Name";

            cmbMainCourse.ItemsSource = menuItems.Where(item => item.Category == "Main Course").ToList();
            cmbMainCourse.DisplayMemberPath = "Name";

            cmbDessert.ItemsSource = menuItems.Where(item => item.Category == "Dessert").ToList();
            cmbDessert.DisplayMemberPath = "Name";
        }

        private void SelectItem(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.SelectedItem != null)
            {
                MenuItem selectedItem = comboBox.SelectedItem as MenuItem;

                var searchItem = selectedItems.FirstOrDefault(item => item?.Name == selectedItem?.Name);

                if (searchItem != null) // already existed in the datagrid, only need to add quantity
                {
                    searchItem.Quantity++;
                } 
                else //add the new selectedItem to datagrid
                {
                    selectedItems.Add(new MenuItem
                    {
                        Name = selectedItem!.Name,
                        Category = selectedItem.Category,
                        Price = selectedItem.Price,
                        Quantity = 1
                    });
                }

                //refresh
                dgMenuItems.ItemsSource = null;
                dgMenuItems.ItemsSource = selectedItems;

                UpdateBill();

                // Reset selection
                comboBox.SelectedItem = null;
            } 
        }

        private void UpdateBill()
        {
            double subtotal = selectedItems.Sum(item => item.Price * item.Quantity);
            double tax = subtotal * 0.13;
            double total = subtotal + tax;

            txtSubtotal.Text = subtotal.ToString("C");
            txtTax.Text = tax.ToString("C");
            txtTotal.Text = total.ToString("C");

            // Enable or disable the Remove Item button
            btnRemoveItem.IsEnabled = selectedItems.Count > 0;
        }

        //handle Clear Bill button
        private void ClearBill_Click(object sender, RoutedEventArgs e)
        {
            selectedItems.Clear();
            dgMenuItems.ItemsSource = null;

            cmbBeverage.SelectedItem = null;
            cmbAppetizer.SelectedItem = null;
            cmbMainCourse.SelectedItem = null;
            cmbDessert.SelectedItem = null;

            UpdateBill();
        }

        //handle quantity updated directly in datagrid
        private void dgMenuItems_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Quantity")
            {
                var item = e.Row.Item as MenuItem;
                if (item != null)
                {
                    // Try to parse the new quantity
                    if (e.EditingElement is TextBox textBox && int.TryParse(textBox.Text, out int newQty))
                    {
                        item.Quantity = Math.Max(1, newQty); // Prevent 0 or negative
                    }
                    else
                    {
                        item.Quantity = 1;
                    }

                    //refresh
                    dgMenuItems.ItemsSource = null;
                    dgMenuItems.ItemsSource = selectedItems;

                    UpdateBill();
                }
            }
        }

        //handle delete one row by selecting a row and pressing Delete
        private void dgMenuItems_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgMenuItems.SelectedItem is MenuItem selected)
            {
                selectedItems.Remove(selected);

                //refresh
                dgMenuItems.ItemsSource = null;
                dgMenuItems.ItemsSource = selectedItems;

                UpdateBill();
            }
        }

        //handle Remove Item button
        private void RemoveSelectedItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgMenuItems.SelectedItem as MenuItem;

            if (selectedItem != null)
            {
                if (MessageBox.Show($"Remove '{selectedItem.Name}' from the bill?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    selectedItems.Remove(selectedItem);

                    dgMenuItems.ItemsSource = null;
                    dgMenuItems.ItemsSource = selectedItems;

                    UpdateBill();
                }
            }
            else
            {
                MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}