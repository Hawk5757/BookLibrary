using System.Xml.Linq;
using BookLibrary.Models;

namespace BookLibrary;

public sealed class BookLibrary
{
    private List<Book> _books = new();
    public IReadOnlyList<Book> Books => _books.AsReadOnly();

    public void LoadFromXmlFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Path cannot be empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"The XML file was not found at: {filePath}");

        try
        {
            using var stream = File.OpenRead(filePath);

            var doc = XDocument.Load(stream);
            var root = doc.Root;

            if (root == null || root.Name != "Books")
                throw new InvalidOperationException("Invalid XML format. Root element must be 'Books'.");

            var loadedBooks = new List<Book>();

            foreach (XElement bookElement in root.Elements("Book"))
            {
                string? title = bookElement.Element("Title")?.Value;
                string? author = bookElement.Element("Author")?.Value;
                string? pagesStr = bookElement.Element("Pages")?.Value;

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
                {
                    throw new InvalidDataException("Book title or author cannot be empty in XML.");
                }

                if (!int.TryParse(pagesStr, out int pages) || pages < 0)
                {
                    throw new InvalidDataException($"Invalid page count '{pagesStr}' for book '{title}'.");
                }

                loadedBooks.Add(new Book(title, author, pages));
            }

            _books.Clear();
            _books.AddRange(loadedBooks);
        }
        catch (Exception ex) when (ex is not FileNotFoundException && ex is not InvalidOperationException)
        {
            throw new Exception($"An error occurred while loading the library: {ex.Message}", ex);
        }
    }
    
    public void AddBook(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));
        if (string.IsNullOrWhiteSpace(book.Title))
            throw new ArgumentException("Book title cannot be empty.", nameof(book));
        if (string.IsNullOrWhiteSpace(book.Author))
            throw new ArgumentException("Book author cannot be empty.", nameof(book));
        if (book.Pages <= 0)
            throw new ArgumentException("Book pages must be greater than 0.", nameof(book));

        _books.Add(book);
    }
    
    public void SaveToXml(string filePath)
    {
        XElement root = new("Books");

        foreach (Book book in _books)
        {
            XElement bookElement = new("Book",
                new XElement("Title", book.Title),
                new XElement("Author", book.Author),
                new XElement("Pages", book.Pages)
            );
            root.Add(bookElement);
        }

        XDocument doc = new(root);
        doc.Save(filePath);
    }
    
    public List<Book> SearchByTitle(string searchCondition)
    {
        if (string.IsNullOrWhiteSpace(searchCondition))
            return new List<Book>();

        return _books
            .Where(b => b.Title.Contains(searchCondition, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    public void SortByAuthorThenTitle()
    {
        _books = _books
            .OrderBy(b => b.Author, StringComparer.OrdinalIgnoreCase)
            .ThenBy(b => b.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
