using System.Xml;
using System.Xml.Linq;
using BookLibrary.Interfaces;

namespace BookLibrary;

public sealed class BookLibrary : IBookLibrary
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
            var loadedBooks = ParseBooksFromXml(filePath);
            _books.Clear();
            _books.AddRange(loadedBooks);
        }
        catch (Exception ex) when (ex is not FileNotFoundException && 
                                   ex is not InvalidOperationException && 
                                   ex is not InvalidDataException)
        {
            throw new Exception($"An error occurred while loading the library: {ex.Message}", ex);
        }
    }
    
    private static List<Book> ParseBooksFromXml(string filePath)
    {
        List<Book> result = [];

        using var reader = XmlReader.Create(filePath);

        SkipToRootElement(reader);

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Book")
                result.Add(ParseBook(reader));
        }

        return result;
    }

    private static void SkipToRootElement(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Books")
                return;
        }

        throw new InvalidOperationException("Invalid XML format. Root element must be 'Books'.");
    }

    private static Book ParseBook(XmlReader reader)
    {
        string? title = null;
        string? author = null;
        int pages = -1;

        using var subtree = reader.ReadSubtree();
        subtree.Read();

        while (subtree.Read())
        {
            if (subtree.NodeType != XmlNodeType.Element)
                continue;

            switch (subtree.Name)
            {
                case "Title":
                    title = subtree.ReadElementContentAsString();
                    break;
                case "Author":
                    author = subtree.ReadElementContentAsString();
                    break;
                case "Pages":
                    var pagesStr = subtree.ReadElementContentAsString();
                    if (!int.TryParse(pagesStr, out pages) || pages <= 0)
                        throw new InvalidDataException($"Invalid page count '{pagesStr}' for book '{title}'.");
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author))
            throw new InvalidDataException("Book title or author cannot be empty in XML.");

        return new Book(title, author, pages);
    }

    public void AddBook(Book book)
    {
        _books.Add(book ?? throw new ArgumentNullException(nameof(book)));
    }

    public void SaveToXml(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Path cannot be empty.", nameof(filePath));
        
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
