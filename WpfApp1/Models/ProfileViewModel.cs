using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Models
{
    public class ProfileViewModel : BaseViewModel
    {
        public Users User { get; set; }

        public ObservableCollection<Reviews> UserReviews { get; set; }

        public string FreezeReason =>
            User?.IsFrozen == true
                ? "Ваш аккаунт временно заморожен администрацией."
                : "";

        public Visibility FrozenVisibility =>
            User?.IsFrozen == true
                ? Visibility.Visible
                : Visibility.Collapsed;

        public Visibility AuthorRequestVisibility
        {
            get
            {
                if (User?.Roles == null)
                    return Visibility.Visible;

                // Автор или админ — скрываем кнопку
                if (User.Roles.RoleID == 2 ||
                    User.Roles.RoleID == 3)
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }

        public ProfileViewModel()
        {
            UserReviews = new ObservableCollection<Reviews>();
        }

        public ProfileViewModel(Users user)
        {
            User = user;

            UserReviews = new ObservableCollection<Reviews>(
                Core.Context.Reviews
                    .Include("Books")
                    .Where(r => r.UserID == user.UserID)
                    .ToList());
        }
    }
}
