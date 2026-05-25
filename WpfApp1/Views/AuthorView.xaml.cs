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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    /// <summary>
    /// Логика взаимодействия для AuthorView.xaml
    /// </summary>
    public partial class AuthorView : UserControl
    {
        private readonly AuthorViewModel _vm;
        private readonly Users _currentUser;

        public AuthorView(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            _vm = new AuthorViewModel(user);
            DataContext = _vm;
        }

        private void AddNewBook_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddBookWindow(_currentUser);
            if (addWindow.ShowDialog() == true)
                _vm.RefreshBooks();
        }

        private void EditBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Books book)
            {
                var editWindow = new AddBookWindow(_currentUser, book);
                if (editWindow.ShowDialog() == true)
                    _vm.RefreshBooks();
            }
        }
        private void FilterBooks_Changed(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (_vm == null)
                return;

            string search = txtSearch?.Text ?? "";

            bool onlyFrozen =
                cbOnlyFrozen?.IsChecked == true;

            _vm.ApplyFilters(search, onlyFrozen);
        }
        private void UnfreezeBook_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is Books book)
            {
                bool exists = Core.Context.UnfreezeRequests
                    .Any(r =>
                        r.BookID == book.BookID &&
                        r.Status == "Pending");

                if (exists)
                {
                    MessageBox.Show(
                        "Заявка уже отправлена.",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }

                var result = MessageBox.Show(
                    "Отправить заявку на разморозку книги?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;

                Core.Context.UnfreezeRequests.Add(
                    new UnfreezeRequests
                    {
                        BookID = book.BookID,
                        Status = "Pending",
                        Reason = "Автор оспаривает заморозку книги.",
                        CreatedAt = DateTime.Now
                    });

                Core.Context.SaveChanges();

                MessageBox.Show(
                    "Заявка отправлена.",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
