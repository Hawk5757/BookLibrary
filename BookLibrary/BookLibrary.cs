using BookLibrary.Models;
using System.Xml;
using System.Xml.Linq;

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

        var loadedBooks = new List<Book>();

        using (var reader = XmlReader.Create(filePath))
        {
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Books")
                        break;
                }

                if (reader.EOF)
                    throw new InvalidOperationException("Invalid XML format. Root element must be 'Books'.");

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "Book")
                    {
                        string? title = null;
                        string? author = null;
                        int pages = -1;

                        using (var bookSubtree = reader.ReadSubtree())
                        {
                            bookSubtree.Read();
                            while (bookSubtree.Read())
                            {
                                if (bookSubtree.NodeType == XmlNodeType.Element)
                                {
                                    switch (bookSubtree.Name)
                                    {
                                        case "Title":
                                            title = bookSubtree.ReadElementContentAsString();
                                            break;
                                        case "Author":
                                            author = bookSubtree.ReadElementContentAsString();
                                            break;
                                        case "Pages":
                                            var pagesStr = bookSubtree.ReadElementContentAsString();
                                            if (!int.TryParse(pagesStr, out pages) || pages <= 0)
                                                throw new InvalidDataException($"Invalid page count '{pagesStr}' for book '{title}'.");
                                            break;
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
                            throw new InvalidDataException("Book title or author cannot be empty in XML.");

                        loadedBooks.Add(new Book(title, author, pages));
                    }
                }
            }
            catch (Exception ex) when (ex is not FileNotFoundException &&
                                       ex is not InvalidOperationException &&
                                       ex is not InvalidDataException)
            {
                throw new Exception($"An error occurred while loading the library: {ex.Message}", ex);
            }
        }

        _books.Clear();
        _books.AddRange(loadedBooks);
    }

    public void AddBook(Book book)
    {
        _books.Add(book ?? throw new ArgumentNullException(nameof(book)));
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

    public IReadOnlyList<Book> SearchByTitle(string searchCondition)
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
