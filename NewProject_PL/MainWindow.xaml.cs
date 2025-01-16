using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewProject_PL
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox.Text == "Введите имя" || textBox.Text == "Введите карту читателя")
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "LoginTextBox")
                {
                    textBox.Text = "Введите имя";
                }
                else if (textBox.Name == "PasswordTextBox")
                {
                    textBox.Text = "Введите карту читателя";
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();

            db_connector.OpenConnection();

            String loginUser = LoginTextBox.Text;
            String reader_card = PasswordTextBox.Text;


            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand(
                "SELECT Role " +
                "FROM readers " +
                "WHERE FirstName = @uL AND LibraryCardNumber = @uP " +
                "ORDER BY Role DESC " +//сорт по роли в убывающем порядке
                "LIMIT 1",//1 запись
                db_connector.GetConnection()
            );

            //параметры
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = loginUser;
            command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = reader_card;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                string user_role = table.Rows[0]["Role"].ToString();

                switch (user_role)
                {
                    case "Библиотекарь":
                        MessageBox.Show("Вы вошли как Библиотекарь");
                        this.Hide();

                        AddData librarian = new AddData(loginUser);
                        librarian.Show();
                        break;

                    case "Читатель":
                        MessageBox.Show("Вы вошли как Читатель");
                        this.Hide();

                        ReaderPlace reader_form = new ReaderPlace(loginUser, reader_card);
                        reader_form.Show();
                        break;

                    case "Администратор":
                        MessageBox.Show("Вы вошли как Администратор");
                        this.Hide();

                        AdminWindow administration_form = new AdminWindow(loginUser);
                        administration_form.Show();
                        break;

                    default:
                        MessageBox.Show("Системная ошибка. Обратитесь к администратору");
                        break;
                }


            }
            else
            {
                MessageBox.Show("Проверьте корректность вводимых данных. Имя/карточку читателя");
            }

            db_connector.CloseConnection();
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }
}
