namespace BookLibrary.Models;

public sealed class Book
{
    public Book(string title, string author, int pages)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Book title cannot be empty.", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Book author cannot be empty.", nameof(author));
        if (pages <= 0)
            throw new ArgumentException("Book pages must be greater than 0.", nameof(pages));

        Title = title;
        Author = author;
        Pages = pages;
    }

    public string Title { get; }
    public string Author { get; }
    public int Pages { get; }
}