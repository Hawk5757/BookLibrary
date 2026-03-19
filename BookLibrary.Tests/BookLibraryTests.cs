using System.Xml.Linq;

namespace BookLibrary.Tests;

public class BookLibraryTests
{
    private readonly string _testXmlPath = Path.Combine(Path.GetTempPath(), "books_test.xml");
    
    public BookLibraryTests()
    {
        // Cleanup before test
        if (File.Exists(_testXmlPath))
            File.Delete(_testXmlPath);
    }
    
    [Fact]
    public void LoadFromXml_FileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        var library = new BookLibrary();

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => library.LoadFromXmlFile("nonexistent_file.xml"));
    }

    [Fact]
    public void LoadFromXml_InvalidXml_ThrowsInvalidOperationException()
    {
        // Arrange
        File.WriteAllText(_testXmlPath, "<InvalidRoot></InvalidRoot>");
        var library = new BookLibrary();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => library.LoadFromXmlFile(_testXmlPath));
    }
    
    [Fact]
    public void LoadFromXml_ValidXmlFile_LoadsBooks()
    {
        // Arrange
        var root = new XElement("Books",
            new XElement("Book",
                new XElement("Title", "The Hobbit"),
                new XElement("Author", "J.R.R. Tolkien"),
                new XElement("Pages", "310")
            ),
            new XElement("Book",
                new XElement("Title", "1984"),
                new XElement("Author", "George Orwell"),
                new XElement("Pages", "328")
            )
        );
        var doc = new XDocument(root);
        doc.Save(_testXmlPath);

        var library = new BookLibrary();

        // Act
        library.LoadFromXmlFile(_testXmlPath);

        // Assert
        Assert.Equal(2, library.Books.Count);
        Assert.Equal("The Hobbit", library.Books[0].Title);
        Assert.Equal("J.R.R. Tolkien", library.Books[0].Author);
        Assert.Equal(310, library.Books[0].Pages);
    }
    
    [Fact]
    public void AddBook_ValidBook_AddsToList()
    {
        // Arrange
        var library = new BookLibrary();
        var book = new Book("The Hobbit", "J.R.R. Tolkien", 310);

        // Act
        library.AddBook(book);

        // Assert
        Assert.Single(library.Books);
        Assert.Equal(book, library.Books[0]);
    }
    
    [Fact]
    public void AddBook_MultipleBooks_AllAdded()
    {
        // Arrange
        var library = new BookLibrary();
        var book1 = new Book("Book1", "Author1", 100);
        var book2 = new Book("Book2", "Author2", 200);
        var book3 = new Book("Book3", "Author3", 300);

        // Act
        library.AddBook(book1);
        library.AddBook(book2);
        library.AddBook(book3);

        // Assert
        Assert.Equal(3, library.Books.Count);
    }
    
    [Fact]
    public void AddBook_NullBook_ThrowsArgumentNullException()
    {
        // Arrange
        var library = new BookLibrary();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => library.AddBook(null!));
    }

    [Fact]
    public void AddBook_EmptyTitle_ThrowsArgumentException()
    {
        // Arrange
        var library = new BookLibrary();
        var book = new Book("", "Author", 100);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => library.AddBook(book));
    }

    [Fact]
    public void AddBook_EmptyAuthor_ThrowsArgumentException()
    {
        // Arrange
        var library = new BookLibrary();
        var book = new Book("Title", "", 100);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => library.AddBook(book));
    }

    [Fact]
    public void AddBook_NegativePages_ThrowsArgumentException()
    {
        // Arrange
        var library = new BookLibrary();
        var book = new Book("Title", "Author", -50);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => library.AddBook(book));
    }
    
    [Fact]
    public void SaveToXml_ValidBooks_CreatesXmlFile()
    {
        // Arrange
        var library = new BookLibrary();
        library.AddBook(new Book("The Hobbit", "J.R.R. Tolkien", 310));
        library.AddBook(new Book("1984", "George Orwell", 328));

        // Act
        library.SaveToXml(_testXmlPath);

        // Assert
        Assert.True(File.Exists(_testXmlPath));
        var doc = XDocument.Load(_testXmlPath);
        var books = doc.Root?.Elements("Book").ToList();
        Assert.Equal(2, books?.Count);
    }
    
    [Fact]
    public void SearchByTitle_PartialMatch_ReturnsMatchingBooks()
    {
        // Arrange
        var library = new BookLibrary();
        library.AddBook(new Book("The Lord of the Rings", "J.R.R. Tolkien", 1100));
        library.AddBook(new Book("The Hobbit", "J.R.R. Tolkien", 310));
        library.AddBook(new Book("Harry Potter", "J.K. Rowling", 450));

        // Act
        var results = library.SearchByTitle("The");

        // Assert
        Assert.Equal(2, results.Count);
        Assert.Contains(results, b => b.Title == "The Lord of the Rings");
        Assert.Contains(results, b => b.Title == "The Hobbit");
    }

    [Fact]
    public void SearchByTitle_CaseInsensitive_ReturnsMatches()
    {
        // Arrange
        var library = new BookLibrary();
        library.AddBook(new Book("The Hobbit", "J.R.R. Tolkien", 310));

        // Act
        var results = library.SearchByTitle("hobbit");

        // Assert
        Assert.Single(results);
        Assert.Equal("The Hobbit", results[0].Title);
    }

    [Fact]
    public void SearchByTitle_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var library = new BookLibrary();
        library.AddBook(new Book("The Hobbit", "J.R.R. Tolkien", 310));

        // Act
        var results = library.SearchByTitle("NonExistent");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void SearchByTitle_EmptySearch_ReturnsEmptyList()
    {
        // Arrange
        var library = new BookLibrary();
        library.AddBook(new Book("The Hobbit", "J.R.R. Tolkien", 310));

        // Act
        var results = library.SearchByTitle("");

        // Assert
        Assert.Empty(results);
    }

}