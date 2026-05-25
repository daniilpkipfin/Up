using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Models
{
    public class ReadingListsModel : BaseViewModel
    {
        private readonly Users _user;
        public ObservableCollection<BookExtended> BooksInList { get; set; }

        public ReadingListsModel(Users user)
        {
            _user = user;
            BooksInList = new ObservableCollection<BookExtended>();
        }

        public void LoadBooksByStatus(string status)
        {
            if (_user == null) return;

            var books = Core.Context.UserBooks
                .Include("Book.User")
                .Where(ub => ub.UserID == _user.UserID && ub.Status == status)
                .Select(ub => ub.Books)
                .ToList();

            BooksInList = new ObservableCollection<BookExtended>(
                books.Select(b => new BookExtended(b))
            );

            OnPropertyChanged(nameof(BooksInList));
        }
        public void ApplyFilters(
    string status,
    string searchText,
    string genreFilter,
    int sortIndex)
        {
            if (_user == null)
                return;

            var books = Core.Context.UserBooks
                .Include("Books.Users")
                .Include("Books.Reviews")
                .Include("Books.Genres")
                .Where(ub =>
                    ub.UserID == _user.UserID &&
                    ub.Status == status)
                .Select(ub => ub.Books)
                .ToList();

            var query = books
                .Select(b => new BookExtended(b))
                .AsQueryable();

            // === Поиск ===
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();

                query = query.Where(b =>
                    (b.Title != null &&
                     b.Title.ToLower().Contains(searchText))

                    ||

                    (b.DisplayName != null &&
                     b.DisplayName.ToLower().Contains(searchText)));
            }

            // === Жанры ===
            if (!string.IsNullOrWhiteSpace(genreFilter) &&
                genreFilter != "Все жанры")
            {
                query = query.Where(b =>
                    b.Book.Genres.Any(g =>
                        g.Name == genreFilter));
            }

            // === Сортировка ===
            switch (sortIndex)
            {
                case 0:
                    query = query.OrderBy(b => b.Title);
                    break;

                case 1:
                    query = query.OrderByDescending(b => b.Title);
                    break;

                case 2:
                    query = query.OrderByDescending(b => b.AvgRating);
                    break;

                case 3:
                    query = query.OrderBy(b => b.AvgRating);
                    break;
            }

            BooksInList =
                new ObservableCollection<BookExtended>(
                    query.ToList());

            OnPropertyChanged(nameof(BooksInList));
        }
    }
}
