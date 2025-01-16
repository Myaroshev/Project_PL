using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewProject_PL
{
    /// <summary>
    /// Логика взаимодействия для ReaderPlace.xaml
    /// </summary>
    public partial class ReaderPlace : Window
    {
        public ReaderPlace(string name, string library_card)
        {
            InitializeComponent();
            ReaderPanelText(name);
            LoadBooksData();
            LibraryCard_Text(library_card);
            LoadReaderBooks();

            //ISBNTextBox.Foreground = Brushes.Gray;
            //ReservationDatePicker.Foreground = Brushes.Gray;
            //ReturnDatePicker.Foreground = Brushes.Gray;
        }

        public void ReaderPanelText(string name)
        {
            ReaderPanel.Content = $"Панель читателя | {name}";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string serach = SearchTextBox.Text.Trim(); //текст
            LoadBooksData(serach); //обновление таблицы
        }
        private void LibraryCard_Text(string card)
        {
            LibraryCardNumberTextBox.Text = $"{card}";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                //подсказки
                if (textBox.Text == "ISBN книги" || textBox.Text == "Выбор даты")

                {
                    textBox.Text = string.Empty;
                    textBox.Foreground = Brushes.Black;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                //if (string.IsNullOrWhiteSpace(textBox.Text))
                //{
                //    if (textBox.Name == "ISBNTextBox") textBox.Text = "ISBN книги";
                //    if (textBox.Name == "ReservationDatePicker") textBox.Text = "Выбор даты";
                //    if (textBox.Name == "ReturnDatePicker") textBox.Text = "Выбор даты";

                //    textBox.Foreground = Brushes.Gray;
                //}
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow main_window = new MainWindow();
            main_window.Show();

            this.Close();
        }

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //выборка строки
            if (BooksDataGrid.SelectedItem is DataRowView selected_row)
            {
                if (selected_row["ISBN"] != null) //значение ISBN
                {
                    string isbn = selected_row["ISBN"].ToString();
                    ISBNTextBox.Text = isbn;
                }
            }
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
        private void LoadMyBooksData(int readerId)
        {
            DBConnector db_connector = new DBConnector();
            try
            {
                db_connector.OpenConnection();

                //запрос
                string query = "SELECT b.ID_book, b.title, b.author, b.ISBN, b.genre, b.Status " +
                               "FROM books b " +
                               "JOIN issuances i ON b.ID_book = i.ID_book ";
                MySqlCommand load_books = new MySqlCommand(query, db_connector.GetConnection());
                load_books.Parameters.AddWithValue("@readerId", readerId);

                DataTable booksTable = new DataTable();
                MySqlDataAdapter adapter = new MySqlDataAdapter(load_books);
                adapter.Fill(booksTable);

                MyBooksDataGrid.ItemsSource = booksTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных о моих книгах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            db_connector.CloseConnection();
        }
        private void LoadReaderBooks()
        {
            string libraryCardNumber = LibraryCardNumberTextBox.Text.Trim(); // номер читательского билета

            DBConnector db_connector = new DBConnector();
            try
            {
                db_connector.OpenConnection();

                // Получаем ID_reader по номеру читательского билета
                MySqlCommand get_reader_id = new MySqlCommand(
                    "SELECT ID_reader FROM readers WHERE LibraryCardNumber = @libraryCardNumber",
                    db_connector.GetConnection()
                );
                get_reader_id.Parameters.AddWithValue("@libraryCardNumber", libraryCardNumber);

                object reader_id_result = get_reader_id.ExecuteScalar();
                if (reader_id_result == null)
                {
                    MessageBox.Show("Читатель с таким номером билета не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int readerId = Convert.ToInt32(reader_id_result);

                // Загрузить книги для текущего читателя
                LoadMyBooksData(readerId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных о моих книгах: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            db_connector.CloseConnection();
        }









        private void ReserveBookButton_Click(object sender, RoutedEventArgs e)
        {
            DBConnector db_connector = new DBConnector();
            db_connector.OpenConnection();

            string libraryCardNumber = LibraryCardNumberTextBox.Text.Trim();//номер читательского билета
            string isbn = ISBNTextBox.Text.Trim();//ISBN книги
            DateTime currentDate = DateTime.Now;//текущая дата
            DateTime? returnDate = ReturnDatePicker.SelectedDate;//дата возврата

            //ID_reader по читательскому билету
            MySqlCommand get_reader_id = new MySqlCommand(
                "SELECT ID_reader FROM readers WHERE LibraryCardNumber = @libraryCardNumber",
                db_connector.GetConnection()
            );
            get_reader_id.Parameters.AddWithValue("@libraryCardNumber", libraryCardNumber);

            var reader_id_result = get_reader_id.ExecuteScalar();
            if (reader_id_result == null)
            {
                MessageBox.Show("Читатель с таким номером билета не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int readerId = Convert.ToInt32(reader_id_result);

            //ID_book по ISBN
            MySqlCommand get_book_id = new MySqlCommand(
                "SELECT ID_book FROM books WHERE ISBN = @isbn",
                db_connector.GetConnection()
            );
            get_book_id.Parameters.AddWithValue("@isbn", isbn);

            var book_id_result = get_book_id.ExecuteScalar();
            if (book_id_result == null)
            {
                MessageBox.Show("Книга с таким ISBN не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int bookId = Convert.ToInt32(book_id_result);

            //проверка даты
            if (!returnDate.HasValue || returnDate.Value <= currentDate || returnDate.Value > currentDate.AddDays(14))
            {
                MessageBox.Show("Дата возврата должна быть позже текущей даты, но не более чем через 2 недели!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // проверка статуса книги
            MySqlCommand check_book_status_command = new MySqlCommand(
                "SELECT Status FROM books WHERE ID_book = @bookId",
                db_connector.GetConnection()
            );
            check_book_status_command.Parameters.AddWithValue("@bookId", bookId);

            var status_result = check_book_status_command.ExecuteScalar();
            if (status_result != null && status_result.ToString() == "выдана")
            {
                MessageBox.Show("Эту книгу уже невозможно получить, она была выдана!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Прерывание выполнения метода, если книга уже выдана
            }

            //дата выдачи
            DateTime issuanceDate = currentDate;//текущая дата(сегодня взяли)

            MySqlCommand insert_command = new MySqlCommand(
                "INSERT INTO issuances (ID_reader, ID_book, IssuanceDate, ReturnPeriod, CurrentCondition) " +
                "VALUES (@readerId, @bookId, @issuanceDate, @returnDate, @currentCondition)",
                db_connector.GetConnection()
            );
            insert_command.Parameters.AddWithValue("@readerId", readerId);
            insert_command.Parameters.AddWithValue("@bookId", bookId);
            insert_command.Parameters.AddWithValue("@issuanceDate", issuanceDate.ToString("yyyy-MM-dd HH:mm:ss"));
            insert_command.Parameters.AddWithValue("@returnDate", returnDate.Value.ToString("yyyy-MM-dd"));
            insert_command.Parameters.AddWithValue("@currentCondition", "выдана");

            try
            {
                insert_command.ExecuteNonQuery();

                MySqlCommand update_book_status = new MySqlCommand(
                    "UPDATE books SET Status = 'выдана' WHERE ID_book = @bookId",
                    db_connector.GetConnection()
                );
                update_book_status.Parameters.AddWithValue("@bookId", bookId);
                update_book_status.ExecuteNonQuery();

                LoadReaderBooks();
                LoadBooksData();

                MessageBox.Show("Книга успешно зарезервирована!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении книги в таблицу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            db_connector.CloseConnection();
        }
    }
}
