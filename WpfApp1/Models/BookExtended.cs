using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UPWPF.Models
{
    public class BookExtended
    {
        public Books Book { get; private set; }
        public double AvgRating { get; private set; }

        // Свойства для привязки в XAML
        public string Title => Book?.Title;
        public string DisplayName => Book?.Users?.DisplayName ?? Book?.Users?.DisplayName ?? "Неизвестный автор";

        public DateTime CreatedAt => Book?.CreatedAt ?? DateTime.MinValue;
        public bool IsFrozen => Book?.IsFrozen ?? false;
        public string GenresString => string.Join(", ",
            Book.Genres.Select(bg => bg?.Name) ?? Enumerable.Empty<string>());
        // Для кнопки "Оспорить заморозку"
        public Visibility ShowUnfreezeButton => IsFrozen ? Visibility.Visible : Visibility.Collapsed;

        public BookExtended(Books book)
        {
            Book = book;
            AvgRating = book.Reviews?.Any() == true
                ? book.Reviews.Average(r => r.Rating)
                : 0;
        }
    }
}
