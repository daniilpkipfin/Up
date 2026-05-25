using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Models
{
    public class CatalogViewModel : BaseViewModel
    {
        private ObservableCollection<BookExtended> _allBooks;
        public ObservableCollection<BookExtended> FilteredBooks { get; set; }
        private readonly Users _user;
        public ObservableCollection<BookExtended> BooksInList { get; set; }
        public BookExtended SelectedBook { get; set; }
        public CatalogViewModel()
        {
            LoadBooks();
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
        private void LoadBooks()
        {
            var rawBooks = Core.Context.Books
                .Include("Users")
                .Include("Reviews")
                .Include("Genres")
                .ToList();

            _allBooks = new ObservableCollection<BookExtended>(
                rawBooks.Select(b => new BookExtended(b))
            );

            FilteredBooks = new ObservableCollection<BookExtended>(_allBooks);
        }
        public void ApplyFilters(string searchText, string genreFilter, int sortIndex)
        {
            var query = _allBooks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();
                query = query.Where(b =>
    (b.Title != null &&
     b.Title.ToLower().Contains(searchText)) ||

    (b.DisplayName != null &&
     b.DisplayName.ToLower().Contains(searchText)));
            }
            if (!string.IsNullOrWhiteSpace(genreFilter) && genreFilter != "Все жанры")
            {
                query = query.Where(b =>
                    b.Book.Genres.Any(g => g.Name == genreFilter));
            }
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

            FilteredBooks = new ObservableCollection<BookExtended>(query.ToList());
            OnPropertyChanged(nameof(FilteredBooks));
        }
    }
}
