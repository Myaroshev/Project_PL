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
    /// Логика взаимодействия для AddData.xaml
    /// </summary>
    public partial class AddData : Window
    {
        public AddData(string name)
        {
            InitializeComponent();
            LoadBooksData();
            LoadReadersData();
            LibPanelText(name);
            AllBooks();

            //TitleTextBox.Foreground = Brushes.Gray;
            //AuthorTextBox.Foreground = Brushes.Gray;
            //ISBNTextBox.Foreground = Brushes.Gray;
            //GenreTextBox.Foreground = Brushes.Gray;

            //CardNumberTextBox.Foreground = Brushes.Gray;
            //BookTitleTextBox.Foreground = Brushes.Gray;
            //DatePicker.Foreground = Brushes.Gray;
            //ReturnDate.Foreground = Brushes.Gray;

            //ReturnCardNumberTextBox.Foreground = Brushes.Gray;
            //ReturnBookTitleTextBox.Foreground = Brushes.Gray;
            //ReturnDatePicker.Foreground = Brushes.Gray;

            //ReportCardNumberTextBox.Foreground = Brushes.Gray;
            //ReportStartDatePicker.Foreground = Brushes.Gray;
            //ReportEndDatePicker.Foreground = Brushes.Gray;
        }

        public void LibPanelText(string name)
        {
            LibPanel.Content = $"Панель библиотекаря | {name}";
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string serach = SearchTextBox.Text.Trim(); //текст
            LoadBooksData(serach); //обновление таблицы
        }


        //книги
        private void LoadBooksData(string search_filter = "")
        {
            DBConnector db_connector = new DBConnector();
            try
            {
                db_connector.OpenConnection();

                //сам запрос
                string query = "SELECT ID_book, title, author, ISBN, genre, status FROM books";

                //фильтрация
                if (!string.IsNullOrEmpty(search_filter))
                {
                    query += " WHERE title LIKE @SearchText"; //оператор LIKE для сравнения строк
                }

                MySqlCommand load_books = new MySqlCommand(query, db_connector.GetConnection());

                if (!string.IsNullOrEmpty(search_filter))
                {
                    load_books.Parameters.AddWithValue("@SearchText", $"%{search_filter}%");
                }

                DataTable booksTable = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(load_books);
                adapter.Fill(booksTable);

                BooksDataGrid.ItemsSource = booksTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных о книгах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                db_connector.CloseConnection();
            }
        }

        //читатели
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
                //if (textBox.Text == "Название книги" || textBox.Text == "Автор" ||
                //    textBox.Text == "ISBN" || textBox.Text == "Жанр" || textBox.Text == "Номер билета"
                //    || textBox.Text == "Название книги" || textBox.Text == "Выбор даты(когда взяли)" || textBox.Text == "Выбор даты(когда вернуть)"
                //    || textBox.Text == "Номер билета (необязательно)")

                //{
                //    textBox.Text = string.Empty;
                //    textBox.Foreground = Brushes.Black;
                //}
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    //if (textBox.Name == "TitleTextBox") textBox.Text = "Название книги";
                    //if (textBox.Name == "AuthorTextBox") textBox.Text = "Автор";
                    //if (textBox.Name == "ISBNTextBox") textBox.Text = "ISBN";
                    //if (textBox.Name == "GenreTextBox") textBox.Text = "Жанр";
                    //if (textBox.Name == "CardNumberTextBox") textBox.Text = "Номер билета";
                    //if (textBox.Name == "BookTitleTextBox") textBox.Text = "ISBN";
                    //if (textBox.Name == "DatePicker") textBox.Text = "Выбор даты(когда взяли)";
                    //if (textBox.Name == "ReturnDate") textBox.Text = "Выбор даты(когда вернуть)";
                    //if (textBox.Name == "ReturnCardNumberTextBox") textBox.Text = "Номер билета";
                    //if (textBox.Name == "ReturnBookTitleTextBox") textBox.Text = "ISBN";

                    //textBox.Foreground = Brushes.Gray;
                }
            }
        }

        private string generateISBN(DBConnector db_connector)
        {
            string isbn;
            Random random = new Random();

            while (true)
            {
                isbn = string.Concat(Enumerable.Range(0, 13).Select(_ => random.Next(0, 10).ToString()));
                //concat объединяет
                //enumarable создает диапазон от 0 до 13
                //random.Next генерирует число от 0 до 9

                // Проверка уникальности в базе данных
                MySqlCommand checkCommand = new MySqlCommand("SELECT COUNT(*) FROM books WHERE ISBN = @isbn", db_connector.GetConnection());
                checkCommand.Parameters.AddWithValue("@isbn", isbn);
                //COUNT(*) подсчитывает все строки независимо от NULL
                // проверяет сколько записей в таблице имеют значение карточки читателя равное уже созданному выше isbn

                long count = (long)checkCommand.ExecuteScalar();

                if (count == 0)
                {
                    break;
                }
            }

            return isbn;
        }


        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            //данные
            string title = TitleTextBox.Text.Trim();
            string author = AuthorTextBox.Text.Trim();
            string genre = GenreTextBox.Text.Trim();

            // Проверка полей
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(genre) || title == "Название книги" || author == "Автор" || genre == "Жанр")
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(author, @"^[А-Яа-яЁёA-Za-z]+\s[А-Яа-яЁёA-Za-z]+$"))
            {
                MessageBox.Show("Введите имя и фамилию автора(буквы, пробел между именем и фамилией)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(genre, @"^[А-Яа-яЁёA-Za-z\s]+$"))
            {
                MessageBox.Show("Введите корректный жанр(буквы и пробелы, без спецсимволов)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //[А-Яа-яЁёA-Za-z]+ - совпадение с символов 1+ из набора
            //А-Яа-я - русские
            //A-Za-z - латинские
            // /s - пробел

            string isbn = generateISBN(db_connector);

            MySqlCommand create_book = new MySqlCommand(
                "INSERT INTO `books` (`title`, `author`, `ISBN`, `genre`, `status`) VALUES (@title, @author, @isbn, @genre, @status)",
                db_connector.GetConnection()
            );

            create_book.Parameters.AddWithValue("@title", title);
            create_book.Parameters.AddWithValue("@author", author);
            create_book.Parameters.AddWithValue("@isbn", isbn);
            create_book.Parameters.AddWithValue("@genre", genre);
            create_book.Parameters.AddWithValue("@status", "доступна");

            try
            {
                create_book.ExecuteNonQuery();
                MessageBox.Show($"Книга успешно добавлена! ISBN: {isbn}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadBooksData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в создании книги: {ex.Message}");
            }

            TitleTextBox.Clear();
            AuthorTextBox.Clear();
            GenreTextBox.Clear();
            ISBNTextBox.Clear();

            db_connector.CloseConnection();


        }

        private void IssueBookButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            //данные
            string cardNumber = CardNumberTextBox.Text.Trim();
            string isbn = BookTitleTextBox.Text.Trim();

            //проверка даты
            string iss_date;
            if (DatePicker.SelectedDate.HasValue)
            {
                iss_date = DatePicker.SelectedDate.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string return_date;
            if (ReturnDate.SelectedDate.HasValue)
            {
                return_date = ReturnDate.SelectedDate.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //проверки
            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(isbn) || iss_date == null || return_date == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //поиск ID читателя
            MySqlCommand reader_id = new MySqlCommand(
                "SELECT ID_reader FROM readers WHERE LibraryCardNumber = @cardNumber",
                db_connector.GetConnection()
            );
            reader_id.Parameters.AddWithValue("@cardNumber", cardNumber);



            var result = reader_id.ExecuteScalar();
            if (result == null)
            {
                MessageBox.Show("Читатель с таким номером карточки не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int readerId = Convert.ToInt32(result);

            //поиск книги по ISBN 
            MySqlCommand book_id = new MySqlCommand(
                "SELECT ID_book FROM books WHERE ISBN = @isbn",
                db_connector.GetConnection()
            );
            book_id.Parameters.AddWithValue("@isbn", isbn);

            var book_id_temp = book_id.ExecuteScalar();
            if (book_id_temp == null)
            {
                MessageBox.Show("Книга с таким ISBN не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int bookId = Convert.ToInt32(book_id_temp);



            //вставка
            MySqlCommand create_issue = new MySqlCommand(
                "INSERT INTO issuances (ID_reader, ID_book, IssuanceDate, ReturnPeriod, CurrentCondition) " +
                "VALUES (@readerId, @bookId, @iss_date, @return_date, @currentCondition)",
                db_connector.GetConnection()
            );
            create_issue.Parameters.AddWithValue("@readerId", readerId);//айди читателя
            create_issue.Parameters.AddWithValue("@bookId", bookId);//айди книги
            create_issue.Parameters.AddWithValue("@iss_date", iss_date);//дата выдачи
            create_issue.Parameters.AddWithValue("@return_date", return_date);//дата возврата
            create_issue.Parameters.AddWithValue("@currentCondition", "Хорошее");//состояние книги

            try
            {
                create_issue.ExecuteNonQuery();
                MessageBox.Show($"Книга успешно выдана!\nДата возврата: до {ReturnDate} включительно", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                CardNumberTextBox.Clear();
                BookTitleTextBox.Clear();
                DatePicker.SelectedDate = null;
                ReturnDate.SelectedDate = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            db_connector.CloseConnection();



        }



        private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            //данные 
            string cardNumber = ReturnCardNumberTextBox.Text.Trim();
            string isbn = ReturnBookTitleTextBox.Text.Trim();

            //проверка ввода
            if (string.IsNullOrWhiteSpace(cardNumber))
            {
                MessageBox.Show("Введите номер билета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("Введите ISBN книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ReturnDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату возврата!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime returnDate = ReturnDatePicker.SelectedDate.Value;

            //состояние книги
            string bookCondition = string.Empty;
            if (BookConditionComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                bookCondition = selectedItem.Content.ToString();
            }
            else
            {
                MessageBox.Show("Выберите состояние книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //ID читателя
            MySqlCommand reader_id = new MySqlCommand(
                "SELECT ID_reader FROM readers WHERE LibraryCardNumber = @cardNumber",
                db_connector.GetConnection()
            );
            reader_id.Parameters.AddWithValue("@cardNumber", cardNumber);

            var reader_id_obj = reader_id.ExecuteScalar();
            if (reader_id_obj == null)
            {
                MessageBox.Show("Читатель с таким номером карточки не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int readerId = Convert.ToInt32(reader_id_obj);

            //ID книги по ISBN
            MySqlCommand book_id = new MySqlCommand(
                "SELECT ID_book FROM books WHERE ISBN = @isbn",
                db_connector.GetConnection()
            );
            book_id.Parameters.AddWithValue("@isbn", isbn);

            var book_id_obj = book_id.ExecuteScalar();
            if (book_id_obj == null)
            {
                MessageBox.Show("Книга с таким ISBN не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int bookId = Convert.ToInt32(book_id_obj);


            MySqlCommand issuance_id = new MySqlCommand(
                "SELECT ID_issuance, ReturnPeriod FROM issuances WHERE ID_reader = @readerId AND ID_book = @bookId",
                db_connector.GetConnection()
            );
            issuance_id.Parameters.AddWithValue("@readerId", readerId);
            issuance_id.Parameters.AddWithValue("@bookId", bookId);

            using (var reader = issuance_id.ExecuteReader())
            {
                if (!reader.Read())
                {
                    MessageBox.Show("Выдача для этой книги и читателя не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int issuance_ID = reader.GetInt32(0);
                DateTime expected_returnDate = reader.GetDateTime(1);

                string expiredStatus = returnDate > expected_returnDate ? "Просрочено" : "Не просрочено";

                reader.Close();

                MySqlCommand create_return = new MySqlCommand(
                    "INSERT INTO returns (ID_issuance, ReturnDate, BookCondition, ExpiredStatus) " +
                    "VALUES (@issuance_ID, @returnDate, @bookCondition, @expiredStatus)",
                    db_connector.GetConnection()
                );

                create_return.Parameters.AddWithValue("@issuance_ID", issuance_ID);
                create_return.Parameters.AddWithValue("@returnDate", returnDate.ToString("yyyy-MM-dd HH:mm:ss"));
                create_return.Parameters.AddWithValue("@bookCondition", bookCondition);
                create_return.Parameters.AddWithValue("@expiredStatus", expiredStatus);

                try
                {
                    create_return.ExecuteNonQuery();
                    AllBooks();
                    MessageBox.Show($"Книга успешно возвращена!\nСтатус возврата: {expiredStatus}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    ReturnCardNumberTextBox.Clear();
                    ReturnBookTitleTextBox.Clear();
                    ReturnDatePicker.SelectedDate = null;
                    BookConditionComboBox.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            db_connector.CloseConnection();
        }








        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            string cardNumber = ReportCardNumberTextBox.Text.Trim();
            DateTime? startDate = ReportStartDatePicker.SelectedDate;//
            DateTime? endDate = ReportEndDatePicker.SelectedDate;//аналогично описанию как в панели администратора

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

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Show();

            this.Close();
        }


        private void AllBooks()
        {
            DBConnector db_connector = new DBConnector();
            try
            {
                db_connector.OpenConnection();

                //запрос
                string query = "SELECT i.ID_issuance, i.ID_reader, i.ID_book, i.IssuanceDate, i.ReturnPeriod, i.CurrentCondition, " +
                               "b.title, b.author, b.ISBN, b.genre " +
                               "FROM issuances i " +
                               "JOIN books b ON i.ID_book = b.ID_book";

                MySqlCommand load_books = new MySqlCommand(query, db_connector.GetConnection());

                DataTable booksTable = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(load_books);
                adapter.Fill(booksTable);

                AllDataBooks.ItemsSource = booksTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных о книгах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            db_connector.CloseConnection();
        }










        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //выборка строки
            if (BooksDataGrid.SelectedItem is DataRowView selected_row)
            {
                if (selected_row["ISBN"] != null) //значение ISBN
                {
                    string isbn = selected_row["ISBN"].ToString();
                    BookTitleTextBox.Text = isbn;//филл поля
                    ReturnBookTitleTextBox.Text = isbn;
                }
            }
        }

        private void ReadersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReadersDataGrid.SelectedItem is DataRowView selectedRow)
            {
                if (selectedRow["LibraryCardNumber"] != null)
                {
                    string libraryCardNumber = selectedRow["LibraryCardNumber"].ToString();

                    //филл
                    CardNumberTextBox.Text = libraryCardNumber;
                    ReturnCardNumberTextBox.Text = libraryCardNumber;
                    ReportCardNumberTextBox.Text = libraryCardNumber;
                }
            }
        }

        private void AllDataBooks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
