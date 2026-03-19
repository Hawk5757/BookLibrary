namespace BookLibrary.Interfaces;

public interface IBookLibrary
{
    IReadOnlyList<Book> Books { get; }
    void LoadFromXmlFile(string filePath);
    void AddBook(Book book);
    void SaveToXml(string filePath);
    IReadOnlyList<Book> SearchByTitle(string searchCondition);
    void SortByAuthorThenTitle();
}