using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPWPF.Models
{
    public class AuthorViewModel : BaseViewModel
    {
        private readonly Users _currentUser;
        public ObservableCollection<BookExtended> MyBooks { get; set; }

        public AuthorViewModel(Users user)
        {
            _currentUser = user;
            RefreshBooks();
        }

        public void RefreshBooks()
        {
            ApplyFilters("", false);
        }

        public void ApplyFilters(string search, bool onlyFrozen)
        {
            if (_currentUser == null)
                return;

            var query = Core.Context.Books
                .Where(b => b.AuthorID == _currentUser.UserID);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(b =>
                    b.Title.ToLower().Contains(search));
            }

            if (onlyFrozen)
            {
                query = query.Where(b => b.IsFrozen);
            }

            var books = query
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            MyBooks = new ObservableCollection<BookExtended>(
                books.Select(b => new BookExtended(b)));

            OnPropertyChanged(nameof(MyBooks));
        }
    }
}
