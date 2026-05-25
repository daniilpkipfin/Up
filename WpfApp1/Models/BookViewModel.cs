    using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.Models
{
    public class BookViewModel : BaseViewModel
    {
        public Books Book { get; set; }
        public ObservableCollection<Reviews> Reviews { get; set; }
        public string GenresString { get; set; }
        public string AuthorName { get; set; }
        public bool IsAdmin { get; set; }
        public Visibility AdminVisibility
        {
            get
            {
                if (IsAdmin == true)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public BookViewModel(Books book)
        {
            Book = book;
            AuthorName = book.Users?.DisplayName ?? book.Users?.DisplayName ?? "Неизвестный автор";

            IsAdmin = (Application.Current.MainWindow as MainWindow)?
    .CurrentUser?
    .Roles?
    .RoleName == "Администратор";

            RefreshReviews();

            GenresString = string.Join(", ",
                book.Genres.Select(bg => bg.Name) ?? Enumerable.Empty<string>());
        }

        public void RefreshReviews()
        {
            Reviews = new ObservableCollection<Reviews>(
                Reviews = new ObservableCollection<Reviews>(
    Core.Context.Reviews
            .Where(r =>
            r.BookID == Book.BookID &&
            !r.IsFrozen)
            .ToList()));
            OnPropertyChanged(nameof(AdminVisibility));
        }

        public void RefreshData()
        {
            RefreshReviews();
            OnPropertyChanged(nameof(Book));
        }
    }
}
