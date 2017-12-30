using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sales_Taxes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Item> itemList = new List<Item>(); // List to hold items added
        private List<Item> totalList = new List<Item>(); // List to hold items without duplicates and number of each item
        Regex regex = new Regex(@"^([+-]?\d{1,3}(?:,?\d{3})*)?(?:[.]\d+)?$");

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds items to the lblResults
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Item item = new Item();
            item.Name = txtName.Text;
            if (!String.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.ClearValue(Border.BorderBrushProperty);
            }
            else
            {
                txtName.BorderBrush = Brushes.Red;
            }
            if (regex.IsMatch(txtPrice.Text) && !String.IsNullOrEmpty(txtPrice.Text))
            {
                item.Price = Convert.ToDouble(txtPrice.Text);
                txtPrice.ClearValue(Border.BorderBrushProperty);
            }
            else
            {
                item.Valid = false;
                txtPrice.BorderBrush = Brushes.Red;
            }

            item.Type = cmbType.Text;
            item.Imported = cmbImported.Text;

            if (item.Valid)
            {
                item.Quantity++;
                itemList.Add(new Item { Name = item.Name, Quantity = item.Quantity, Price = item.Price, Type = item.Type, Imported = item.Imported });
                totalList.Add(new Item { Name = item.Name, Quantity = item.Quantity, Price = item.Price, Type = item.Type, Imported = item.Imported });
                txtResults.Text = "";

                // Check for and remove duplicates from totalList
                if (totalList.GroupBy(x => new { x.Name, x.Price, x.Type, x.Imported })
                   .Where(x => x.Skip(1).Any()).ToArray().Any())
                {
                    item.Quantity++;
                    // Increasing quantity of item
                    foreach (var s in totalList.Where(w => w.Name == item.Name && w.Price == item.Price && w.Type == item.Type && w.Imported == item.Imported))
                    {
                        s.Quantity++;
                    }
                    totalList.RemoveAt(totalList.Count - 1); // Remove current duplicate item
                }

                foreach (var s in itemList)
                {
                    txtResults.Text += String.Concat("1 ", s.Imported == "Yes" ? "imported " : null, s.Name, " at ", s.Price.ToString(), "\n");
                }
            }
        }

        /// <summary>
        /// Calculates total for all items, tax, and displays receipt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculateTotal_Click(object sender, RoutedEventArgs e)
        {
            double total = 0;
            double tax = 0;
            double totalTax = 0;
            double priceWithTax = 0;
            const double TAXRATE = .1;
            const double IMPORTEDTAX = .05;
            txtResults.Text = "";

            if (!String.IsNullOrWhiteSpace(txtName.Text) && !String.IsNullOrWhiteSpace(txtPrice.Text))
            {
                foreach (var s in totalList)
                {
                    tax = 0;

                    if (s.Imported == "Yes")
                    {
                        if (s.Type == "Other")
                        {
                            tax += Math.Round((s.Price * (TAXRATE + IMPORTEDTAX)), 2);
                        }
                        else
                        {
                            tax += Math.Round((s.Price * (IMPORTEDTAX)), 2);
                        }
                    }
                    else
                    {
                        if (s.Type == "Other")
                        {
                            tax += Math.Round((s.Price * TAXRATE), 2);
                        }
                    }
                    tax = Math.Ceiling(tax / .05) * .05;

                    totalTax += tax * s.Quantity;
                    priceWithTax = (s.Price + tax) * s.Quantity;
                    total += priceWithTax;
                    s.Name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.Name); // Pascal case the name of the item
                    txtResults.Text += String.Concat(s.Imported == "Yes" ? "Imported " : null, s.Name, ": ", s.Quantity > 1 ? String.Concat(priceWithTax.ToString("0.00"), " (", s.Quantity.ToString(), " @ ", (s.Price + tax).ToString("0.00"), ") ") : priceWithTax.ToString("0.00"), "\n");
                }

                txtResults.Text += String.Concat("Sales Tax: ", totalTax.ToString("0.00"), "\n");
                txtResults.Text += String.Concat("Total: ", total.ToString("0.00"));
            }
        }

        /// <summary>
        /// Clears all textboxes, resets combobox values/borderbrush color, clears list, and sets focus to name textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = "";
            txtPrice.Text = "";
            txtResults.Text = "";
            cmbType.Text = "Other";
            cmbImported.Text = "No";
            txtName.ClearValue(Border.BorderBrushProperty);
            txtPrice.ClearValue(Border.BorderBrushProperty);
            itemList.Clear();
            totalList.Clear();
            txtName.Focus();
        }

        /// <summary>
        /// Loops through list to display items in the list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string displayList(List<String> list)
        {
            foreach (String s in list)
            {
                return s.ToString();
            }
            return null;
        }
    }
}
