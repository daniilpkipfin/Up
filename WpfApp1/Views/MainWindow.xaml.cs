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
using WpfApp1.Views;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Users CurrentUser { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(Users user) : this()
        {
            CurrentUser = user;

            bool isFrozen = user?.IsFrozen == true;

            string roleName = user?.Roles?.RoleName ?? "";

            if (isFrozen)
            {
                btnAuthor.Visibility = Visibility.Collapsed;
                btnAdmin.Visibility = Visibility.Collapsed;

                btnCatalog.Visibility = Visibility.Collapsed;
                btnLists.Visibility = Visibility.Collapsed;

                ContentArea.Content = new ProfileView(CurrentUser);

                MessageBox.Show(
                    "Ваш аккаунт заморожен.\nДоступна только страница профиля.",
                    "Аккаунт заморожен",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            btnAuthor.Visibility =
                (roleName == "Автор" || roleName == "Администратор")
                ? Visibility.Visible
                : Visibility.Collapsed;

            btnAdmin.Visibility =
                (roleName == "Администратор")
                ? Visibility.Visible
                : Visibility.Collapsed;

            ContentArea.Content = new CatalogView();
        }

        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser?.IsFrozen == true)
                return;

            ContentArea.Content = new CatalogView();
        }
        private void BtnLists_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser?.IsFrozen == true)
                return;

            ContentArea.Content = new ReadingLists(CurrentUser);
        }
        private void BtnAuthor_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser?.IsFrozen == true)
                return;

            if (CurrentUser?.Roles?.RoleName == "Автор" ||
                CurrentUser?.Roles?.RoleName == "Администратор")
            {
                ContentArea.Content = new AuthorView(CurrentUser);
            }
            else
            {
                MessageBox.Show(
                    "Эта страница доступна только авторам!",
                    "Доступ запрещён");
            }
        }
        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser?.IsFrozen == true)
                return;

            ContentArea.Content = new AdminView();
        }
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => ContentArea.Content = new ProfileView(CurrentUser);
    }
}
