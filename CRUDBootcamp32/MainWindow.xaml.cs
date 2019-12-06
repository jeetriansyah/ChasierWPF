using CRUDBootcamp32.Context;
using CRUDBootcamp32.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Outlook = Microsoft.Office.Interop.Outlook;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace CRUDBootcamp32
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyContext myContext = new MyContext();

        int supplierId, transId, roleId;
        int itemId;
        int totalPrice = 0, totalHarga = 0, pay;
        string struk = "ID\t" + "Item Name\t" + "Price\t" + "Quantity\t" + "Sub Total" + "\n";
        List<TransactionDetail> cart = new List<TransactionDetail>();
        //List<TransactionDetail> Cart = new List<TransactionDetail>();

        public MainWindow()
        {
            InitializeComponent();
            ShowData();
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;

            BtnDeleteItem.IsEnabled = false;
            BtnEditItem.IsEnabled = false;

            TransDate.Text = DateTimeOffset.Now.DateTime.ToString("dd MMMM yyyy hh:mm:ss");

            Clear();

            // Add Item to ComboBox
            CmbRoleUser.ItemsSource = myContext.Roles.ToList();
            CmbSupplier.ItemsSource = myContext.Suppliers.ToList();
            CmbSupplier.DisplayMemberPath = "Name";
            CmbSupplier.SelectedValuePath = "Id";

            CmbNameItem.ItemsSource = myContext.Items.ToList();
            CmbNameItem.DisplayMemberPath = "NameItem";
            CmbNameItem.SelectedValuePath = "ID";
        }

        #region Supplier
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (TxtName.Text == "")
            {
                MessageBox.Show("Name cannot be empty!", "Caution", MessageBoxButton.OK);
                TxtName.Focus();
            }
            else if (TxtEmail.Text == "")
            {
                MessageBox.Show("Email cannot be empty!", "Caution", MessageBoxButton.OK);
                TxtEmail.Focus();
            }
            else
            {
                var checkEmail = myContext.Suppliers.Where(email => email.Email == TxtEmail.Text).FirstOrDefault(); // mencari email yg ada
                if (checkEmail == null)
                {
                    var push = new Supplier(TxtName.Text, TxtEmail.Text);
                    myContext.Suppliers.Add(push);
                    var result = myContext.SaveChanges();
                    if (result > 0)
                    {
                        MessageBox.Show(result + " row has been inserted!");
                    }
                    ShowData();
                    CmbSupplier.ItemsSource = myContext.Suppliers.ToList();
                    try
                    {
                        //Outlook._Application _app = new Outlook.Application();
                        //Outlook.MailItem mail = (Outlook.MailItem)_app.CreateItem(Outlook.OlItemType.olMailItem);
                        //mail.To = TxtEmail.Text;
                        //mail.Subject = "Notifikasi email ditambahkan.";
                        //mail.Body = "Hai, " + TxtName.Text + "! Email Anda (" + TxtEmail.Text + ") sudah terdaftar ke dalam sistem kami.";
                        //mail.Importance = Outlook.OlImportance.olImportanceNormal;
                        //((Outlook._MailItem)mail).Send();
                        MessageBox.Show("Your email has been sent!", "Message", MessageBoxButton.OK);
                        TxtID.Text = "";
                        TxtEmail.Text = "";
                        TxtName.Text = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("This email has been used!");
                }

            }
        }

        private void GridSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var data = GridSupplier.SelectedItem;
                string id = (GridSupplier.SelectedCells[0].Column.GetCellContent(data) as TextBlock).Text;
                TxtID.Text = id;
                string name = (GridSupplier.SelectedCells[1].Column.GetCellContent(data) as TextBlock).Text;
                TxtName.Text = name;
                string email = (GridSupplier.SelectedCells[2].Column.GetCellContent(data) as TextBlock).Text;
                TxtEmail.Text = email;
                BtnSubmit.IsEnabled = false;
                BtnDelete.IsEnabled = true;
                BtnEdit.IsEnabled = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void TxtEmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^A-Za-z0-9.@]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int id = Convert.ToInt32(TxtID.Text);
            var uRow = myContext.Suppliers.FirstOrDefault(s => s.Id == id);
            uRow.Name = TxtName.Text;
            uRow.Email = TxtEmail.Text;
            myContext.SaveChanges();
            ShowData();
            MessageBox.Show("Update successed!");
            BtnSubmit.IsEnabled = true;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
            TxtID.Text = "";
            TxtName.Text = "";
            TxtEmail.Text = "";
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do You Want Delete this Record ?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int id = Convert.ToInt32(TxtID.Text);
                    var delRow = myContext.Suppliers.Where(s => s.Id == id).FirstOrDefault();
                    myContext.Suppliers.Remove(delRow);
                    myContext.SaveChanges();
                    TxtID.Text = "";
                    TxtName.Text = "";
                    TxtEmail.Text = "";
                    MessageBox.Show("Record has been deleted!");
                    BtnSubmit.IsEnabled = true;
                    BtnEdit.IsEnabled = false;
                    BtnDelete.IsEnabled = false;
                    ShowData();
                }
            }
            catch (Exception)
            {

            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TxtID.Text = "";
            TxtName.Text = "";
            TxtEmail.Text = "";
            BtnSubmit.IsEnabled = true;
            BtnEdit.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }
        #endregion

        #region Item
        private void TxtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TxtStock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnRefreshItem_Click(object sender, RoutedEventArgs e)
        {
            BtnSave.IsEnabled = true;
            BtnEditItem.IsEnabled = false;
            BtnDeleteItem.IsEnabled = false;
            TxtItemID.Text = "";
            TxtItemName.Text = "";
            TxtStock.Text = "";
            TxtPrice.Text = "";
            //CmbSupplier.ItemsSource = myContext.Suppliers.ToList();
            //CmbSupplier.Text = "- Choose Supplier -";
            //supplierId = 0;
        }

        private void TxtItemName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^A-Za-z!]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((TxtItemName.Text == "") || (TxtStock.Text == "") || (TxtPrice.Text == ""))
                {
                    if (TxtItemName.Text == "")
                    {
                        MessageBox.Show("Name Item Is Required", "Caution", MessageBoxButton.OK);
                        TxtItemName.Focus();
                    }
                    else if (TxtStock.Text == "")
                    {
                        MessageBox.Show("Stock Item Is Required", "Caution", MessageBoxButton.OK);
                        TxtStock.Focus();
                    }
                    else if (TxtPrice.Text == "")
                    {
                        MessageBox.Show("Price Item  is Required", "Caution", MessageBoxButton.OK);
                        TxtPrice.Focus();
                    }
                }
                else
                {
                    if (TxtItemName.Text != null)
                    {
                        int Stock = Convert.ToInt32(TxtStock.Text);
                        int Price = Convert.ToInt32(TxtPrice.Text);

                        var supplier = myContext.Suppliers.Where(s => s.Id == supplierId).FirstOrDefault();
                        var itemname = myContext.Items.Where(i => i.NameItem == TxtItemName.Text).FirstOrDefault();
                        //var priceitem = myContext.Items.Where(i => i.Price == TxtPriceItem.Text).FirstOrDefault();



                        if (itemname != null)
                        {
                            var stockrecent = itemname.Stock;
                            var pricerecent = itemname.Price;
                            var supplierrecent = itemname.Supplier.ToString();


                            if (TxtPrice.Text == pricerecent.ToString())
                            {
                                int updStock = Stock + stockrecent;
                                itemname.Stock = Convert.ToInt32(updStock);
                                var result2 = myContext.SaveChanges();

                                if (result2 > 0)
                                {
                                    MessageBox.Show("Stock Has Been Updated");
                                }
                                else
                                {
                                    MessageBox.Show("Stock Cant be Updated");

                                }
                                GridItem.ItemsSource = myContext.Items.ToList();
                            }
                            else
                            {
                                int Stock2 = Convert.ToInt32(TxtStock.Text);
                                int Price2 = Convert.ToInt32(TxtPrice.Text);

                                var supplier2 = myContext.Suppliers.Where(s => s.Id == supplierId).FirstOrDefault();
                                var pushStock = new Items(TxtItemName.Text, Stock2, Price2, supplier2);
                                myContext.Items.Add(pushStock);
                                var result = myContext.SaveChanges();
                                if (result > 0)
                                {
                                    MessageBox.Show("New Item has been inserted");
                                }
                                else
                                {
                                    MessageBox.Show("New item cant be inserted");
                                }
                                GridItem.ItemsSource = myContext.Items.ToList();
                            }
                        }
                        else
                        {
                            int Stock2 = Convert.ToInt32(TxtStock.Text);
                            int Price2 = Convert.ToInt32(TxtPrice.Text);

                            var supplier2 = myContext.Suppliers.Where(s => s.Id == supplierId).FirstOrDefault();
                            var pushStock = new Items(TxtItemName.Text, Stock2, Price2, supplier2);
                            myContext.Items.Add(pushStock);
                            var result = myContext.SaveChanges();
                            if (result > 0)
                            {
                                MessageBox.Show("New Item has been inserted");
                                TxtItemName.Text = "";
                                TxtStock.Text = "";
                                TxtPrice.Text = "";
                            }
                            else
                            {
                                MessageBox.Show("New item cant be inserted");
                            }
                            GridItem.ItemsSource = myContext.Items.ToList();
                        }
                    }

                }
                CmbNameItem.ItemsSource = myContext.Items.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK);
            }
        }

        private void CmbSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            supplierId = Convert.ToInt32(CmbSupplier.SelectedValue.ToString());
        }

        private void GridItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var data = GridItem.SelectedItem;
                string id = (GridItem.SelectedCells[0].Column.GetCellContent(data) as TextBlock).Text;
                TxtItemID.Text = id;
                string name = (GridItem.SelectedCells[1].Column.GetCellContent(data) as TextBlock).Text;
                TxtItemName.Text = name;
                string stock = (GridItem.SelectedCells[2].Column.GetCellContent(data) as TextBlock).Text;
                TxtStock.Text = stock;
                string price = (GridItem.SelectedCells[3].Column.GetCellContent(data) as TextBlock).Text;
                TxtPrice.Text = price;
                string supplier = (GridItem.SelectedCells[4].Column.GetCellContent(data) as TextBlock).Text;
                CmbSupplier.Text = supplier;
                //BtnSave.IsEnabled = false;
                BtnDeleteItem.IsEnabled = true;
                BtnEditItem.IsEnabled = true;
            }
            catch (Exception)
            {
            }
        }

        private void BtnEditItem_Click(object sender, RoutedEventArgs e)
        {
            var supplier = myContext.Suppliers.Where(s => s.Id == supplierId).FirstOrDefault();
            int id = Convert.ToInt32(TxtItemID.Text);
            var uRow = myContext.Items.FirstOrDefault(s => s.ID == id);
            uRow.NameItem = TxtItemName.Text;
            uRow.Stock = Convert.ToInt32(TxtStock.Text);
            uRow.Price = Convert.ToInt32(TxtPrice.Text);
            uRow.Supplier = supplier;
            myContext.SaveChanges();
            ShowData();
            CmbNameItem.ItemsSource = myContext.Items.ToList();
            MessageBox.Show("Update successed!");
            BtnSave.IsEnabled = true;
            BtnEditItem.IsEnabled = false;
            BtnDeleteItem.IsEnabled = false;
            TxtItemID.Text = "";
            TxtItemName.Text = "";
            TxtStock.Text = "";
            TxtPrice.Text = "";
            //CmbSupplier.ItemsSource = myContext.Suppliers.ToList();
            //CmbSupplier.Text = "- Choose Supplier -";
            //supplierId = 0;
        }

        private void BtnDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do You Want Delete this Record ?", "Delete Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    int id = Convert.ToInt32(TxtItemID.Text);
                    var delRow = myContext.Items.Where(i => i.ID == id).FirstOrDefault();
                    myContext.Items.Remove(delRow);
                    myContext.SaveChanges();
                    MessageBox.Show("Record has been deleted!");
                    TxtItemID.Text = "";
                    TxtItemName.Text = "";
                    TxtStock.Text = "";
                    TxtPrice.Text = "";
                    BtnSave.IsEnabled = true;
                    BtnEditItem.IsEnabled = false;
                    BtnDeleteItem.IsEnabled = false;
                    ShowData();
                    //CmbSupplier.ItemsSource = myContext.Suppliers.ToList();
                    //CmbSupplier.Text = "- Choose Supplier -";
                    //supplierId = 0;
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        public void ShowData()
        {
            GridSupplier.ItemsSource = myContext.Suppliers.ToList();
            GridItem.ItemsSource = myContext.Items.ToList();
            GridUser.ItemsSource = myContext.Users.ToList();
            GridRole.ItemsSource = myContext.Roles.ToList();
            //GridTransItem.ItemsSource = myContext.TransactionDetail.ToList();
        }

        #region Transaction
        private void CmbNameItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            itemId = Convert.ToInt32(CmbNameItem.SelectedValue.ToString());
            //var data = GridItem.SelectedItem;
            var item = myContext.Items.Where(i => i.ID == itemId).FirstOrDefault();
            TxtPriceItem.Text = item.Price.ToString();
            TxtStockItem.Text = item.Stock.ToString();
        }

        private void BtnAddItem_Click(object sender, RoutedEventArgs e)
        {
            if (CmbNameItem.Text == "")
            {
                MessageBox.Show("Select An Item!", "Warning!", MessageBoxButton.OK);
            }
            else if (TxtPriceItem.Text == "")
            {
                MessageBox.Show("Entry Price Item!", "Warning!", MessageBoxButton.OK);
            }
            else if (TxtQty.Text == "" || TxtQty.Text == "0")
            {
                MessageBox.Show("Quantity is Required!", "Warning!", MessageBoxButton.OK);
            }
            else
            {
                int qty = Convert.ToInt32(TxtQty.Text);
                int price = Convert.ToInt32(TxtPriceItem.Text);
                int stock = Convert.ToInt32(TxtStockItem.Text);
                int subtot = qty * price;
                int updateStock = stock - qty;
                TxtSubTotal.Text = subtot.ToString();

                totalPrice += subtot;

                transId = Convert.ToInt32(TransID.Text);
                var trans = myContext.Transactions.Where(t => t.ID == transId).FirstOrDefault();
                var item = myContext.Items.Where(i => i.ID == itemId).FirstOrDefault();

                //Update Stock
                item.Stock = updateStock;
                myContext.SaveChanges();
                ShowData();

                cart.Add(new TransactionDetail { Transactions = trans, Items = item, Quantity = qty, SubTotal = subtot });
 
                GridTransItem.Items.Add(new { Name = CmbNameItem.Text, Price = TxtPriceItem.Text, Qty = TxtQty.Text, SubTotal = subtot.ToString() });

                TxtTotalPrice.Text = "Rp. " + totalPrice.ToString("n0") + ",-";
                TxtTotPrice.Text = totalPrice.ToString();

                TxtPriceItem.Text = "";
                TxtQty.Text = "";
                TxtStockItem.Text = "";
            }
        }

        private void btnDeleteOneItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var data = GridTransItem.SelectedItem;
                string itemName = (GridTransItem.SelectedCells[0].Column.GetCellContent(data) as TextBlock).Text;
                string quantityItem = (GridTransItem.SelectedCells[2].Column.GetCellContent(data) as TextBlock).Text;
                string subTotal = (GridTransItem.SelectedCells[3].Column.GetCellContent(data) as TextBlock).Text;
                int subtotInt = Convert.ToInt32(subTotal);
                //int priceTotal = Convert.ToInt32(TxtTotPrice.Text);
                int total = Convert.ToInt32(TxtTotPrice.Text);

                if (GridTransItem.SelectedItem != null)
                {
                    //int currentStock = Convert.ToInt32(TxtStockItem.Text); //stock yang ada sekarang
                    int qtyCart = Convert.ToInt32(quantityItem);
                    int subtotCart = Convert.ToInt32(subTotal);

                    var item = myContext.Items.Where(i => i.NameItem == itemName).FirstOrDefault();
                    int stockNow = item.Stock;
                    int realStock = Convert.ToInt32(quantityItem) + stockNow;
                    int realTotal = total - subtotCart;
                    //totalPrice -= subtotCart;

                    item.Stock = realStock;
                    myContext.SaveChanges();

                    TxtStockItem.Text = realStock.ToString();
                    TxtTotPrice.Text = realTotal.ToString();
                    GridTransItem.Items.RemoveAt(GridTransItem.SelectedIndex);
                    GridItem.ItemsSource = myContext.Items.ToList();

                    //int totalAfter = total - subtotInt;
                    //totalPrice -= subtotInt;
                    //TxtTotPrice.Text = totalAfter.ToString();

                }
                else
                {
                    //TxtTotPrice.Text = totalPrice.ToString();
                    TxtTotPrice.Clear();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

        }
        #endregion

        private void TxtQty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidasiInputNum(e);
        }

        private void TxtPay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidasiInputNum(e);
        }

        public void ValidasiInputNum(TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TxtPay_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                totalHarga = Convert.ToInt32(TxtTotPrice.Text);
                pay = Convert.ToInt32(TxtPay.Text);
                TxtChange.Text = "Rp. " + (pay - totalHarga).ToString("n0") + ",-";
                BtnSubmitTrans.IsEnabled = true;
            }
            catch (Exception)
            {

            }
        }

        private void BtnSubmitTrans_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                totalHarga = Convert.ToInt32(TxtTotPrice.Text);
                pay = Convert.ToInt32(TxtPay.Text);
                if (TxtPay.Text == "")
                {
                    MessageBox.Show("Payment is Required!", "Caution");
                    TxtPay.Focus();
                }
                else if (totalHarga <= pay)
                {
                    int transId = Convert.ToInt32(TransID.Text);
                    var item = myContext.TransactionDetail.FirstOrDefault(i => i.Transactions.ID == transId);
                    var trans = myContext.Transactions.FirstOrDefault(t => t.ID == transId);
                    int totPrice = Convert.ToInt32(TxtTotPrice.Text);
                    trans.TotalPrice = totPrice;
                    foreach (var transItem in cart)
                    {
                        myContext.TransactionDetail.Add(transItem);
                        myContext.SaveChanges();
                        struk += transItem.Items.ID.ToString() + "\t" + transItem.Items.NameItem + "\t" + transItem.Items.Price + "\t" + transItem.Quantity + "\t" + transItem.SubTotal + "\n";
                    }
                    totalPrice = 0;
                    TransID.Text = "";
                    MessageBox.Show("Your change is : Rp. " + (pay - totalHarga).ToString("n0") + "\nThank You! :)", "Notification", MessageBoxButton.OK);
                    using (PdfDocument document = new PdfDocument())
                    {
                        //Add a page to the document
                        PdfPage page = document.Pages.Add();

                        //Create PDF graphics for the page
                        PdfGraphics graphics = page.Graphics;

                        //Set the standard font
                        PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);

                        //Draw the text
                        graphics.DrawString(struk, font, PdfBrushes.Black, new PointF(0, 0));

                        //Save the document
                        document.Save("Output.pdf");

                        #region View the Workbook
                        //Message box confirmation to view the created document.
                        if (MessageBox.Show("Do you want to view the PDF?", "PDF has been created",
                            MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            try
                            {
                                //Launching the Excel file using the default Application.[MS Excel Or Free ExcelViewer]
                                System.Diagnostics.Process.Start("Output.pdf");

                                //Exit
                                //Close();
                            }
                            catch (Win32Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }
                        //Close();
                        #endregion
                    }
                    AddTransaction.IsEnabled = true;
                    Clear();
                    BtnSubmitTrans.IsEnabled = false;
                    BtnAddItem.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Your Payment is Invalid!", "Caution", MessageBoxButton.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (TxtUsername.Text == "")
            {
                MessageBox.Show("Name cannot be empty!", "Caution", MessageBoxButton.OK);
                TxtUsername.Focus();
            }
            else if (TxtEmailUser.Text == "")
            {
                MessageBox.Show("Email cannot be empty!", "Caution", MessageBoxButton.OK);
                TxtEmailUser.Focus();
            }
            else if (CmbRoleUser.Text == "")
            {
                MessageBox.Show("Select a Role!", "Caution", MessageBoxButton.OK);
                CmbRoleUser.Focus();
            }
            else
            {
                var checkEmail = myContext.Users.Where(email => email.Email == TxtEmailUser.Text).FirstOrDefault(); // mencari email yg ada
                var pass = Guid.NewGuid().ToString();
                if (checkEmail == null)
                {
                    var role = myContext.Roles.Where(r => r.ID == roleId).FirstOrDefault();
                    var push = new User(TxtUsername.Text, TxtEmailUser.Text, pass, role);
                    myContext.Users.Add(push);
                    myContext.SaveChanges();
                    MessageBox.Show("1 row has been inserted!");
                    ShowData();
                    try
                    {
                        Outlook._Application _app = new Outlook.Application();
                        Outlook.MailItem mail = (Outlook.MailItem)_app.CreateItem(Outlook.OlItemType.olMailItem);
                        mail.To = TxtEmailUser.Text;
                        mail.Subject = "Register Notification.";
                        mail.Body = "Hai, " + TxtUsername.Text + "! Email Anda (" + TxtEmailUser.Text + ") sudah terdaftar ke dalam sistem kami.\nSilahkan login dengan password : " + pass;
                        mail.Importance = Outlook.OlImportance.olImportanceNormal;
                        ((Outlook._MailItem)mail).Send();
                        MessageBox.Show("Your email has been sent!", "Message", MessageBoxButton.OK);
                        TxtUsername.Text = "";
                        TxtEmailUser.Text = "";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK);
                    }
                }
                else
                {
                    MessageBox.Show("This email has been used!");
                }

            }
        }

        private void GridUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var data = GridUser.SelectedItem;
                TxtUsername.Text = (GridUser.SelectedCells[1].Column.GetCellContent(data) as TextBlock).Text;
                TxtEmailUser.Text = (GridUser.SelectedCells[2].Column.GetCellContent(data) as TextBlock).Text;
                CmbRoleUser.Text = (GridUser.SelectedCells[3].Column.GetCellContent(data) as TextBlock).Text;
                BtnRegister.IsEnabled = false;
            }
            catch (Exception)
            {
            }
        }

        private void BtnCancelRegister_Click(object sender, RoutedEventArgs e)
        {
            TxtUsername.Text = "";
            TxtEmailUser.Text = "";
            BtnRegister.IsEnabled = true;
        }

        private void TxtEmailUser_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^A-Za-z0-9.@]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TxtUsername_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^A-Za-z]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TabItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("You'r Loging Out!", "Logout Succes", MessageBoxButton.OK);
            Login_Form login = new Login_Form();
            login.Show();
            this.Close();
        }

        private void CmbRoleUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            roleId = Convert.ToInt32(CmbRoleUser.SelectedValue.ToString());
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            var push = new Transaction();
            myContext.Transactions.Add(push);
            myContext.SaveChanges();
            TransID.Text = Convert.ToString(push.ID);
            AddTransaction.IsEnabled = false;
            BtnAddItem.IsEnabled = true;
            GridUser.ItemsSource = myContext.Users.ToList();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            cart.Clear();
        }

        public void Clear()
        {
            GridTransItem.Items.Clear();
            TxtPriceItem.Text = "";
            TxtTotalPrice.Text = "Rp. 0,-";
            TxtTotPrice.Text = "-";
            TxtPay.Text = "";
            TxtChange.Text = "Rp. 0,-";
            TxtQty.Text = "";
        }
    }
}