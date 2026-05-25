using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class AdminViewModel : BaseViewModel
    {
        public ObservableCollection<Complaints> Complaints { get; set; }
        public ObservableCollection<RoleRequests> RoleRequests { get; set; }
        public ObservableCollection<UnfreezeRequests> UnfreezeRequests { get; set; }
        public ObservableCollection<Users> Users { get; set; }
        public ObservableCollection<FrozenItem> FrozenItems { get; set; }

        public AdminViewModel()
        {
            RefreshData();
        }

        public void RefreshData()
        {

            Complaints = new ObservableCollection<Complaints>(
                Core.Context.Complaints
                    .Include("Users")
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList());

            RoleRequests = new ObservableCollection<RoleRequests>(
                Core.Context.RoleRequests
                    .Include("Users")
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList());

            UnfreezeRequests = new ObservableCollection<UnfreezeRequests>(
                Core.Context.UnfreezeRequests
                    .Include("Users")
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList());

            Users = new ObservableCollection<Users>(
                Core.Context.Users
                    .Include("Roles")
                    .OrderBy(u => u.DisplayName)
                    .ToList());

            var frozenBooks = Core.Context.Books
                .Where(b => b.IsFrozen)
                .ToList()
                .Select(b => new FrozenItem
                {
                    Type = "Книга",
                    Name = b.Title,
                    FrozenDate = b.CreatedAt
                });
            
            var frozenUsers = Core.Context.Users
                .Where(u => u.IsFrozen)
                .ToList()
                .Select(u => new FrozenItem
                {
                    Type = "Пользователь",
                    Name = u.DisplayName,
                    FrozenDate = u.CreatedAt
                });

            var frozenReviews = Core.Context.Reviews
                .Where(r => r.IsFrozen)
                .ToList()
                .Select(r => new FrozenItem
                {
                    Type = "Отзыв",
                    Name = r.ReviewText,
                    FrozenDate = r.CreatedAt
                });

            FrozenItems = new ObservableCollection<FrozenItem>(
                frozenBooks
                .Concat(frozenUsers)
                .Concat(frozenReviews));

            OnPropertyChanged(nameof(Complaints));
            OnPropertyChanged(nameof(RoleRequests));
            OnPropertyChanged(nameof(UnfreezeRequests));
            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(FrozenItems));
        }
    }
}
