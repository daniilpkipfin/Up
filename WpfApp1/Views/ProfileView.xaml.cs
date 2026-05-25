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
    /// Логика взаимодействия для ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl
    {
        public ProfileView(Users user)
        {
            InitializeComponent();
            DataContext = new ProfileViewModel(user);   // Передаём пользователя
        }

        // Конструктор без параметров для дизайнера
        public ProfileView()
        {
            InitializeComponent();
            DataContext = new ProfileViewModel(); // Пустой для дизайнера
        }
        private void RequestAuthor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заявка на получение роли «Автор» успешно отправлена!\n\nОжидайте рассмотрения администратором.",
                            "Заявка отправлена",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
        private void Unregistration_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Window.GetWindow(this)?.Close();
        }
        private void AppealFreeze_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProfileViewModel vm &&
                vm.User != null)
            {
                // Проверяем существующую заявку
                bool exists = Core.Context.UnfreezeRequests.Any(r =>
                    r.UserID == vm.User.UserID);

                if (exists)
                {
                    MessageBox.Show(
                        "Заявка уже отправлена.",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }

                Core.Context.UnfreezeRequests.Add(new UnfreezeRequests
                {
                    UserID = vm.User.UserID,
                    Reason = "Пользователь оспаривает заморозку аккаунта.",
                    CreatedAt = DateTime.Now
                });

                Core.Context.SaveChanges();

                MessageBox.Show(
                    "Заявка на разморозку отправлена.",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}
