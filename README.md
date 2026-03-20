# Book Library (C#)

## Overview
This project is a C# class library that provides functionality to manage a collection of books. It is designed to be used as a backend component by other software modules and does not include any user interface.

## Features

The library supports the following operations:

- Load a list of books from an XML file
- Add a new book to the collection
- Sort books:
  - First by author (alphabetically)
  - Then by title (alphabetically within each author)
- Search books by part of the title (simple substring match, not fuzzy search)
- Save the list of books to an XML file

## Book Model

Each book contains the following properties:

- **Title**
- **Author**
- **Number of Pages**

## Sorting Example

Books are sorted as follows:

1. By author name (A → Z)
2. For each author, by book title (A → Z)

Example:
- Andersen
  - The Little Mermaid
  - The Ugly Duckling
- King
  - It
  - The Shining

## Technical Requirements

- Language: C#
- Platform: .NET (recommended: .NET 6 or higher)
- Project type: Class Library
- No UI required

## Testing

The library should be covered with unit tests use xUnit.

