using CRUDBootcamp32.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace CRUDBootcamp32
{
    /// <summary>
    /// Interaction logic for ForgottPassword.xaml
    /// </summary>
    public partial class ForgottPassword : Window
    {
        MyContext myContext = new MyContext();
        public ForgottPassword()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login_Form formLogin = new Login_Form();
            formLogin.Show();
            this.Close();
        }

        private void BtnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TxtEmailUser.Text == "")
                {
                    MessageBox.Show("Email is Required!", "Caution", MessageBoxButton.OK);
                    TxtEmailUser.Focus();
                }
                else
                {
                    var checkEmail = myContext.Users.FirstOrDefault(u => u.Email == TxtEmailUser.Text);
                    if (checkEmail != null)
                    {
                        var email = checkEmail.Email;
                        if (TxtEmailUser.Text == email)
                        {
                            var newPass = Guid.NewGuid().ToString();
                            checkEmail.Password = newPass;
                            myContext.SaveChanges();
                            MessageBox.Show("Password has been updated!");
                            try
                            {
                                Outlook._Application _app = new Outlook.Application();
                                Outlook.MailItem mail = (Outlook.MailItem)_app.CreateItem(Outlook.OlItemType.olMailItem);
                                mail.To = TxtEmailUser.Text;
                                mail.Subject = "Password Changed.";
                                mail.Body = "Hai, " + checkEmail.Username + "!\nPassword Anda sekarang adalah  : " + newPass;
                                mail.Importance = Outlook.OlImportance.olImportanceNormal;
                                ((Outlook._MailItem)mail).Send();
                                MessageBox.Show("Your email has been sent!", "Message", MessageBoxButton.OK);
                                TxtEmailUser.Text = "";
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Your Email Did Not Find!", "Caution", MessageBoxButton.OK);
                        TxtEmailUser.Text = "";
                        TxtEmailUser.Focus();
                    }
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
