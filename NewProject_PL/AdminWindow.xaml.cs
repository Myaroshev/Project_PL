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
using System.Windows.Shapes;

namespace NewProject_PL
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow(string name)
        {
            InitializeComponent();
            LoadReadersData();
            AdminLabelPanel(name);
            LoadReadersData();

            //LastNameTextBox.Foreground = Brushes.Gray;
            //FirstNameTextBox.Foreground = Brushes.Gray;
            //BirthDatePicker.Foreground = Brushes.Gray;
            //LibraryCardNumberTextBox.Foreground = Brushes.Gray;
            //ContactsTextBox.Foreground = Brushes.Gray;

            //ReportCardNumberTextBox.Foreground = Brushes.Gray;
            //ReportStartDatePicker.Foreground = Brushes.Gray;
            //ReportEndDatePicker.Foreground = Brushes.Gray;

            //LibraryCardNumberDeleteTextBox.Foreground = Brushes.Gray;
        }

        public void AdminLabelPanel(string name)
        {
            AdminLabel.Content = $"Панель администратора | {name}";
        }

        private void ReadersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReadersDataGrid.SelectedItem is DataRowView selectedRow)
            {
                if (selectedRow["LibraryCardNumber"] != null)
                {
                    string libraryCardNumber = selectedRow["LibraryCardNumber"].ToString();

                    //филл
                    LibraryCardNumberDeleteTextBox.Text = libraryCardNumber;
                    ReportCardNumberTextBox.Text = libraryCardNumber;
                    ReportCardNumberTextBox.Text = libraryCardNumber;
                }
            }
        }

        private void SearchReaderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchReaderTextBox.Text.Trim();
            LoadReadersData(searchText);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                ////подсказки
                //if (textBox.Text == "Фамилия" || textBox.Text == "Имя" ||
                //    textBox.Text == "Выбор даты" || textBox.Text == "Номер билета" || textBox.Text == "Контакты (телефон)"
                //    || textBox.Text == "Название книги" || textBox.Text == "Номер билета (необязательно)")
                //{
                //    textBox.Text = string.Empty;
                //    textBox.Foreground = Brushes.Black;
                //}
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Show();

            this.Close();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox.Name == "LastNameTextBox") textBox.Text = "Фамилия";
                    if (textBox.Name == "FirstNameTextBox") textBox.Text = "Имя";
                    if (textBox.Name == "BirthDatePicker") textBox.Text = "Выбор даты";
                    if (textBox.Name == "LibraryCardNumberTextBox") textBox.Text = "Номер билета";
                    if (textBox.Name == "ContactsTextBox") textBox.Text = "Контакты (телефон)";
                    if (textBox.Name == "LibraryCardNumberDeleteTextBox") textBox.Text = "Номер билета";

                    textBox.Foreground = Brushes.Gray;
                }
            }
        }

        private void LoadReadersData(string search_filter = "")
        {
            DBConnector db_connector = new DBConnector();
            try
            {
                db_connector.OpenConnection();

                //запрос
                string query = "SELECT ID_reader, LastName, FirstName, BirthDate, LibraryCardNumber, Contacts FROM readers";

                //фильтр
                if (!string.IsNullOrEmpty(search_filter))
                {
                    query += " WHERE LastName LIKE @SearchText OR FirstName LIKE @SearchText"; //аналогично как я делали по книгнам
                }

                MySqlCommand load_readers = new MySqlCommand(query, db_connector.GetConnection());

                if (!string.IsNullOrEmpty(search_filter))
                {
                    load_readers.Parameters.AddWithValue("@SearchText", $"%{search_filter}%");
                }

                DataTable readersTable = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(load_readers);
                adapter.Fill(readersTable);

                ReadersDataGrid.ItemsSource = readersTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных о читателях: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                db_connector.CloseConnection();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void LoadReadersData()
        {
            DBConnector db_connector = new DBConnector();
            try
            {
                db_connector.OpenConnection();

                string query = "SELECT ID_reader, LastName, FirstName, BirthDate, LibraryCardNumber, Contacts, Role FROM readers";
                MySqlCommand load_readers = new MySqlCommand(query, db_connector.GetConnection());

                DataTable readers_table = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(load_readers);
                adapter.Fill(readers_table);

                ReadersDataGrid.ItemsSource = readers_table.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            db_connector.CloseConnection();
        }

        private string generateLCN(DBConnector db_connector)
        {
            string lcn;
            Random random = new Random();

            while (true)
            {
                lcn = string.Concat(Enumerable.Range(0, 20).Select(_ => random.Next(0, 10).ToString()));
                //concat объединяет
                //enumarable создает диапазон от 0 до 20
                //random.Next генерирует число от 0 до 9

                // Проверка уникальности в базе данных
                MySqlCommand checkCommand = new MySqlCommand("SELECT COUNT(*) FROM readers WHERE LibraryCardNumber = @lcn", db_connector.GetConnection());
                checkCommand.Parameters.AddWithValue("@lcn", lcn);
                //COUNT(*) подсчитывает все строки независимо от NULL
                // проверяет сколько записей в таблице имеют значение карточки читателя равное уже созданному выше lcn

                long count = (long)checkCommand.ExecuteScalar();
                //возвращает число записей с данным номером
                //если их 0, то прерывается цикл и возвращает это значение(проверка на одинаковые карточки)
                if (count == 0)
                {
                    break;
                }
            }

            return lcn;
        }


        private void AddReaderButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            //данные
            string lastName = LastNameTextBox.Text.Trim(); //фамилия
            string firstName = FirstNameTextBox.Text.Trim();//имя
            string birthDateString = BirthDatePicker.Text.Trim();//дата рождения
            string libraryCardNumberString = LibraryCardNumberTextBox.Text.Trim();//карточка
            string contactsString = ContactsTextBox.Text.Trim();//телефон
            string role = RoleComboBox.Text.Trim();//роль
            string registrationDate = DateTime.Now.ToString("yyyy-MM-dd");//дата регистрации

            //проверка полей
            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(birthDateString) || string.IsNullOrWhiteSpace(libraryCardNumberString) ||
                string.IsNullOrWhiteSpace(contactsString) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //проверка имени и фамилии(буквы и пробелы)
            if (!System.Text.RegularExpressions.Regex.IsMatch(lastName, @"^[А-Яа-яЁёA-Za-z]+$"))
            {
                MessageBox.Show("Введите корректную фамилию(только буквы)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(firstName, @"^[А-Яа-яЁёA-Za-z]+$"))
            {
                MessageBox.Show("Введите корректное имя(только буквы)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //проверка даты рождения
            if (!DateTime.TryParse(birthDateString, out DateTime birthDate))
            {
                MessageBox.Show("Введите корректную дату рождения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //проверка контактов (только цифры)
            if (!System.Text.RegularExpressions.Regex.IsMatch(contactsString, @"^\d{11,}$"))
            {
                MessageBox.Show("Введите корректный номер телефона(только цифры, минимум 11 символов)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            /*
             * ^ - начало строки
             * \d - 1 цифра
             * {11,} - минимум 11
             * s - конец строки
            */

            //проверка роли
            if (role != "Читатель" && role != "Библиотекарь")
            {
                MessageBox.Show("Выберите корректную роль(читатель или библиотекарь)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string lcn = generateLCN(db_connector);

            //добавление
            MySqlCommand create_user = new MySqlCommand(
                "INSERT INTO `readers` (`LastName`, `FirstName`, `BirthDate`, `LibraryCardNumber`, `Contacts`, `RegistrationDate`, `Role`) " +
                "VALUES (@lastName, @firstName, @birthDate, @libraryCardNumber, @contacts, @registrationDate, @role)",
                db_connector.GetConnection()
            );

            create_user.Parameters.AddWithValue("@lastName", lastName);
            create_user.Parameters.AddWithValue("@firstName", firstName);
            create_user.Parameters.AddWithValue("@birthDate", birthDate.ToString("yyyy-MM-dd"));
            create_user.Parameters.AddWithValue("@libraryCardNumber", lcn);
            create_user.Parameters.AddWithValue("@contacts", contactsString);
            create_user.Parameters.AddWithValue("@registrationDate", registrationDate);
            create_user.Parameters.AddWithValue("@role", role);

            try
            {
                create_user.ExecuteNonQuery();
                MessageBox.Show("Пользователь успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Обновление списка пользователей
                LoadReadersData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в создании пользователя: {ex.Message}");
            }

            // Очистка полей
            LastNameTextBox.Clear();
            FirstNameTextBox.Clear();
            BirthDatePicker.SelectedDate = null;
            LibraryCardNumberTextBox.Clear();
            ContactsTextBox.Clear();

            db_connector.CloseConnection();
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            string cardNumber = ReportCardNumberTextBox.Text.Trim(); //номер карты
            DateTime? startDate = ReportStartDatePicker.SelectedDate;
            DateTime? endDate = ReportEndDatePicker.SelectedDate;
            //позволяет хранить как дату, так и null, если дата не выбрана

            List<Report> reportItems = new List<Report>();

            string query = "SELECT r.LastName, r.FirstName, b.ISBN, i.IssuanceDate, re.ReturnDate, re.ExpiredStatus " +//выбираем фамилию и имя читателя, ISBN книги, дату выдачи книги, дату возврата и статус просрочки
                           "FROM returns re " +//возвраты книг с алиасом 're'
                           "JOIN issuances i ON re.ID_issuance = i.ID_issuance " +//соединяем таблицу возвратов с таблицей выдач, по полю ID_issuance
                           "JOIN readers r ON i.ID_reader = r.ID_reader " +//соединяем таблицу выдач с таблицей читателей, по полю ID_reader
                           "JOIN books b ON i.ID_book = b.ID_book ";//соединяем таблицу выдач с таблицей книг, по полю ID_book


            //параметры для запроса
            List<MySqlParameter> parameters = new List<MySqlParameter>();

            //если введен номер читательского билета, то добавляем его в условие WHERE
            if (!string.IsNullOrWhiteSpace(cardNumber))
            {
                query += "WHERE r.LibraryCardNumber = @cardNumber ";//добавляем условие для фильтрации по номеру читательского билета
                parameters.Add(new MySqlParameter("@cardNumber", cardNumber));//добавляем параметр с номером билета
            }

            //если указаны даты начала и окончания, добавляем условие по датам возврата
            if (startDate.HasValue && endDate.HasValue)
            {
                //если уже есть условие WHERE, добавляем "AND" для дополнительной фильтрации по датам
                if (query.Contains("WHERE"))
                {
                    query += "AND re.ReturnDate BETWEEN @startDate AND @endDate ";//добавляем условие по диапазону дат возврата
                }
                else
                {
                    query += "WHERE re.ReturnDate BETWEEN @startDate AND @endDate ";//если еще нет условия WHERE, добавляем его с диапазоном дат
                }

                //добавляем параметры для начала и окончания периода
                parameters.Add(new MySqlParameter("@startDate", startDate.Value.ToString("yyyy-MM-dd")));//параметр для даты начала
                parameters.Add(new MySqlParameter("@endDate", endDate.Value.ToString("yyyy-MM-dd")));//параметр для даты окончания
            }

            //добавляем сортировку результатов по дате возврата(по убыванию)
            query += "ORDER BY re.ReturnDate DESC;";//сортируем результаты по дате возврата в порядке убывания

            try
            {
                MySqlCommand cmd = new MySqlCommand(query, db_connector.GetConnection());

                // Добавляем параметры в команду
                foreach (var param in parameters)
                {
                    cmd.Parameters.Add(param);
                }

                fill_report_data(cmd, reportItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //привязка данных к окну отчетов
            ReportWindow reportWindow = new ReportWindow();
            reportWindow.ReportDataGrid.ItemsSource = reportItems;

            reportWindow.Show();

            db_connector.CloseConnection();
        }
        private void fill_report_data(MySqlCommand cmd, List<Report> reportItems)
        {
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Report item = new Report
                    {
                        LastName = reader.GetString("LastName"),
                        FirstName = reader.GetString("FirstName"),
                        ISBN = reader.GetString("ISBN"),
                        IssuanceDate = reader.GetDateTime("IssuanceDate"),
                        ReturnDate = reader.GetDateTime("ReturnDate"),
                        ExpiredStatus = reader.GetString("ExpiredStatus")
                    };

                    reportItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void delete_readerButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            //номер билета
            string libraryCardNumberString = LibraryCardNumberDeleteTextBox.Text.Trim();

            //проверка на пустое значение
            if (string.IsNullOrWhiteSpace(libraryCardNumberString))
            {
                MessageBox.Show("Введите номер читательского билета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //проверка на корректность формата номера
            if (!System.Text.RegularExpressions.Regex.IsMatch(libraryCardNumberString, @"^\d+$"))
            {
                MessageBox.Show("Введите корректный номер читательского билета(только цифры)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                //проверка существования пользователя
                MySqlCommand readerChecker = new MySqlCommand(
                    "SELECT Role FROM readers WHERE LibraryCardNumber = @libraryCardNumber",
                    db_connector.GetConnection()
                );
                readerChecker.Parameters.AddWithValue("@libraryCardNumber", libraryCardNumberString);

                var role_object = readerChecker.ExecuteScalar();

                if (role_object == null)
                {
                    MessageBox.Show("Пользователь с таким номером читательского билета не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string role = role_object.ToString();

                //проверка на админа
                if (role == "Администратор")
                {
                    MessageBox.Show("Вы не можете удалить Администратора!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //удаление пользователя
                MySqlCommand deleteReader = new MySqlCommand(
                    "DELETE FROM readers WHERE LibraryCardNumber = @libraryCardNumber",
                    db_connector.GetConnection()
                );
                deleteReader.Parameters.AddWithValue("@libraryCardNumber", libraryCardNumberString);

                deleteReader.ExecuteNonQuery();
                MessageBox.Show("Пользователь успешно удалён!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                //обновление данных
                LoadReadersData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                db_connector.CloseConnection();
                LibraryCardNumberDeleteTextBox.Clear();
            }
        }



    }
}
