using CRUDBootcamp32.Context;
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
using System.Windows.Shapes;

namespace CRUDBootcamp32
{
    /// <summary>
    /// Interaction logic for Login_Form.xaml
    /// </summary>
    public partial class Login_Form : Window
    {
        MyContext myContext = new MyContext();
        public Login_Form()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var email = myContext.Users.Where(u => u.Email == TxtEmailLogin.Text).FirstOrDefault();

                if ((TxtEmailLogin.Text == "") || (TxtPasswordLogin.Password == ""))
                {
                    if (TxtEmailLogin.Text == "")
                    {
                        MessageBox.Show("Email is Required!", "Caution", MessageBoxButton.OK);
                        TxtEmailLogin.Focus();
                    }
                    else if (TxtPasswordLogin.Password == "")
                    {
                        MessageBox.Show("Password is Required!", "Caution", MessageBoxButton.OK);
                        TxtPasswordLogin.Focus();
                    }
                }
                else
                {
                    if (email != null)
                    {
                        var psw = email.Password;
                        if (TxtPasswordLogin.Password == psw)
                        {
                            MessageBox.Show("Login Successfully!", "Login Succes", MessageBoxButton.OK);
                            MainWindow dashboard = new MainWindow();
                            dashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Email and Password are wrong!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Email and Password is invalid");
                    }

                }
            }
            catch (Exception)
            {

            }
        }

        private void TxtEmailLogin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^A-Za-z0-9.@]+$");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnForgotPass_Click(object sender, RoutedEventArgs e)
        {
            ForgottPassword forgotPass = new ForgottPassword();
            forgotPass.Show();
            this.Close();
        }
    }
}
