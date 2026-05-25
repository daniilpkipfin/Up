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
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        private readonly AdminViewModel _vm;

        public AdminView()
        {
            InitializeComponent();
            _vm = new AdminViewModel();
            DataContext = _vm;
        }
        private void ApproveComplaint_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is Complaints c)
            {
                switch (c.TargetType)
                {
                    case "Book":

                        var book = Core.Context.Books.Find(c.TargetID);

                        if (book != null)
                            book.IsFrozen = true;

                        break;

                    case "Author":

                        var user = Core.Context.Users.Find(c.TargetID);

                        if (user != null)
                            user.IsFrozen = true;

                        break;

                    case "Review":

                        var review = Core.Context.Reviews.Find(c.TargetID);

                        if (review != null)
                            review.IsFrozen = true;

                        break;
                }

                Core.Context.Complaints.Remove(c);

                Core.Context.SaveChanges();

                MessageBox.Show(
                    "Жалоба принята.",
                    "Успех");

                _vm.RefreshData();
            }
        }

        private void RejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is Complaints c)
            {
                Core.Context.Complaints.Remove(c);

                Core.Context.SaveChanges();

                MessageBox.Show(
                    "Жалоба отклонена.",
                    "Готово");

                _vm.RefreshData();
            }
        }
        private void ApproveRoleRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is RoleRequests req)
            {
                var user = Core.Context.Users.Find(req.UserID);
                if (user != null)
                {
                    user.RoleID = req.RequestedRoleID;
                    Core.Context.RoleRequests.Remove(req);
                    Core.Context.SaveChanges();
                    MessageBox.Show($"Роль Автора выдана {user.DisplayName}");
                    _vm.RefreshData();
                }
            }
        }

        private void RejectRoleRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is RoleRequests req)
            {
                Core.Context.RoleRequests.Remove(req);
                Core.Context.SaveChanges();
                MessageBox.Show("Заявка отклонена.");
                _vm.RefreshData();
            }
        }
        private void ApproveUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn &&
                btn.Tag is UnfreezeRequests req)
            {
                if (req.UserID.HasValue)
                {
                    var user = Core.Context.Users.Find(req.UserID.Value);

                    if (user != null)
                        user.IsFrozen = false;
                }

                if (req.BookID.HasValue)
                {
                    var book = Core.Context.Books.Find(req.BookID.Value);

                    if (book != null)
                        book.IsFrozen = false;
                }

                Core.Context.UnfreezeRequests.Remove(req);

                Core.Context.SaveChanges();

                MessageBox.Show(
                    "Объект разморожен.",
                    "Успех");

                _vm.RefreshData();
            }
        }

        private void RejectUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is UnfreezeRequests req)
            {
                Core.Context.UnfreezeRequests.Remove(req);
                Core.Context.SaveChanges();
                MessageBox.Show("Заявка отклонена.");
                _vm.RefreshData();
            }
        }
        private void ChangeUserRole_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Users user)
            {
                // Получаем все роли
                var allRoles = Core.Context.Roles.ToList();
                if (allRoles.Count == 0)
                {
                    MessageBox.Show("Роли не найдены в базе!", "Ошибка");
                    return;
                }

                var roleWindow = new Window
                {
                    Title = $"Смена роли для: {user.DisplayName}",
                    Width = 340,
                    Height = 200,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };

                var stack = new StackPanel { Margin = new Thickness(20) };

                stack.Children.Add(new TextBlock
                {
                    Text = "Выберите новую роль:",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                var comboBox = new ComboBox { Margin = new Thickness(0, 0, 0, 15) };
                foreach (var role in allRoles)
                {
                    comboBox.Items.Add(role.RoleName);
                }
                comboBox.SelectedItem = user.Roles?.RoleName ?? "Читатель";

                stack.Children.Add(comboBox);

                var btnSave = new Button
                {
                    Content = "Сменить роль",
                    Height = 38,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                btnSave.Click += (s, args) =>
                {
                    string selectedRoleName = comboBox.SelectedItem?.ToString();
                    if (string.IsNullOrEmpty(selectedRoleName))
                    {
                        roleWindow.Close();
                        return;
                    }

                    var newRole = allRoles.FirstOrDefault(r => r.RoleName == selectedRoleName);
                    if (newRole != null)
                    {
                        user.RoleID = newRole.RoleID;
                        Core.Context.SaveChanges();

                        MessageBox.Show($"Роль пользователя **{user.DisplayName}**\nуспешно изменена на:\n**{selectedRoleName}**",
                                        "Роль изменена", MessageBoxButton.OK, MessageBoxImage.Information);

                        _vm.RefreshData();
                    }

                    roleWindow.Close();
                };

                stack.Children.Add(btnSave);
                roleWindow.Content = stack;
                roleWindow.ShowDialog();
            }
        }
        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Users user)
            {
                var passwordWindow = new Window
                {
                    Title = $"Смена пароля: {user.DisplayName}",
                    Width = 350,
                    Height = 220,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    ResizeMode = ResizeMode.NoResize
                };

                var panel = new StackPanel
                {
                    Margin = new Thickness(20)
                };

                panel.Children.Add(new TextBlock
                {
                    Text = "Введите новый пароль:",
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 10)
                });

                var passwordBox = new PasswordBox
                {
                    Height = 35,
                    Margin = new Thickness(0, 0, 0, 20)
                };

                panel.Children.Add(passwordBox);

                var saveButton = new Button
                {
                    Content = "Сохранить",
                    Height = 38
                };

                saveButton.Click += (s, args) =>
                {
                    string newPassword = passwordBox.Password;

                    if (string.IsNullOrWhiteSpace(newPassword))
                    {
                        MessageBox.Show("Пароль не может быть пустым.");
                        return;
                    }

                    user.Password = newPassword;

                    Core.Context.SaveChanges();

                    MessageBox.Show(
                        "Пароль успешно изменён!",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    passwordWindow.Close();
                };

                panel.Children.Add(saveButton);

                passwordWindow.Content = panel;
                passwordWindow.ShowDialog();
            }
        }
        private void ToggleFreezeUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Users user)
            {
                user.IsFrozen = !user.IsFrozen;
                Core.Context.SaveChanges();
                MessageBox.Show(user.IsFrozen ? "Пользователь заморожен." : "Пользователь разморожен.");
                _vm.RefreshData();
            }
        }
    }
}
