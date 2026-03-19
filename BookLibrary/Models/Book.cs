using System.Xml.Serialization;

namespace BookLibrary.Models;

[XmlType("Book")]
public sealed class Book
{
    public Book() { }
    
    public Book(string title, string author, int pages)
    {
        Title = title;
        Author = author;
        Pages = pages;
    }
    
    [XmlElement("Title")]
    public string Title { get; set; } = string.Empty;

    [XmlElement("Author")]
    public string Author { get; set; } = string.Empty;

    [XmlElement("Pages")]
    public int Pages { get; set; }
}