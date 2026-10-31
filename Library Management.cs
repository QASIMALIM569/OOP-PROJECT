using System;
using System.Collections.Generic;

namespace LibraryManagementSystem
{
    class Book
    {
        public int Id;
        public string Title;
        public string Author;
        public bool IsIssued;
        public int IssuedToMemberId;
        public DateTime DueDate;

        public Book(int id, string title, string author)
        {
            Id = id;
            Title = title;
            Author = author;
            IsIssued = false;
        }
    }

    class Member
    {
        public int Id;
        public string Name;
        public string Phone;

        public Member(int id, string name, string phone)
        {
            Id = id;
            Name = name;
            Phone = phone;
        }
    }

    class Library
    {
        private List<Book> books = new List<Book>();
        private List<Member> members = new List<Member>();

        private int nextBookId = 1;
        private int nextMemberId = 1;

        private const int LoanDays = 14;
        private const int FinePerDay = 10;

        public void AddBook(string title, string author)
        {
            Book newBook = new Book(nextBookId, title, author);
            books.Add(newBook);
            Console.WriteLine("Book added successfully! Book ID = " + nextBookId);
            nextBookId++;
        }

        public void ViewAllBooks()
        {
            if (books.Count == 0)
            {
                Console.WriteLine("No books in the library yet.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-25} {2,-20} {3,-10}", "ID", "Title", "Author", "Status");
            Console.WriteLine(new string('-', 65));

            foreach (Book b in books)
            {
                string status = b.IsIssued ? "Issued" : "Available";
                Console.WriteLine("{0,-5} {1,-25} {2,-20} {3,-10}", b.Id, b.Title, b.Author, status);
            }
        }

        public void SearchBook(string keyword)
        {
            bool found = false;
            keyword = keyword.ToLower();

            foreach (Book b in books)
            {
                if (b.Title.ToLower().Contains(keyword) || b.Author.ToLower().Contains(keyword))
                {
                    string status = b.IsIssued ? "Issued" : "Available";
                    Console.WriteLine("ID: {0} | {1} by {2} | {3}", b.Id, b.Title, b.Author, status);
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No book found with that keyword.");
            }
        }

        public void DeleteBook(int id)
        {
            Book book = FindBook(id);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
            }
            else if (book.IsIssued)
            {
                Console.WriteLine("This book is currently issued. Return it first.");
            }
            else
            {
                books.Remove(book);
                Console.WriteLine("Book deleted successfully.");
            }
        }

        public void AddMember(string name, string phone)
        {
            Member newMember = new Member(nextMemberId, name, phone);
            members.Add(newMember);
            Console.WriteLine("Member added successfully! Member ID = " + nextMemberId);
            nextMemberId++;
        }

        public void ViewAllMembers()
        {
            if (members.Count == 0)
            {
                Console.WriteLine("No members registered yet.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-25} {2,-15}", "ID", "Name", "Phone");
            Console.WriteLine(new string('-', 47));

            foreach (Member m in members)
            {
                Console.WriteLine("{0,-5} {1,-25} {2,-15}", m.Id, m.Name, m.Phone);
            }
        }

        public void IssueBook(int bookId, int memberId)
        {
            Book book = FindBook(bookId);
            Member member = FindMember(memberId);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
            }
            else if (member == null)
            {
                Console.WriteLine("Member not found.");
            }
            else if (book.IsIssued)
            {
                Console.WriteLine("Sorry, this book is already issued to someone else.");
            }
            else
            {
                book.IsIssued = true;
                book.IssuedToMemberId = memberId;
                book.DueDate = DateTime.Now.AddDays(LoanDays);

                Console.WriteLine("'{0}' issued to {1}.", book.Title, member.Name);
                Console.WriteLine("Please return before: " + book.DueDate.ToString("dd-MMM-yyyy"));
            }
        }

        public void ReturnBook(int bookId)
        {
            Book book = FindBook(bookId);

            if (book == null)
            {
                Console.WriteLine("Book not found.");
            }
            else if (!book.IsIssued)
            {
                Console.WriteLine("This book was not issued.");
            }
            else
            {
                int lateDays = (DateTime.Now.Date - book.DueDate.Date).Days;

                if (lateDays > 0)
                {
                    int fine = lateDays * FinePerDay;
                    Console.WriteLine("Book is late by {0} day(s). Fine = Rs. {1}", lateDays, fine);
                }
                else
                {
                    Console.WriteLine("Book returned on time. No fine.");
                }

                book.IsIssued = false;
                book.IssuedToMemberId = 0;
                Console.WriteLine("'{0}' returned successfully.", book.Title);
            }
        }

        public void ViewIssuedBooks()
        {
            bool found = false;

            foreach (Book b in books)
            {
                if (b.IsIssued)
                {
                    Member m = FindMember(b.IssuedToMemberId);
                    string memberName = (m != null) ? m.Name : "Unknown";

                    Console.WriteLine("Book: {0} | Issued to: {1} | Due: {2}",
                        b.Title, memberName, b.DueDate.ToString("dd-MMM-yyyy"));
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("No books are currently issued.");
            }
        }

        private Book FindBook(int id)
        {
            foreach (Book b in books)
            {
                if (b.Id == id) return b;
            }
            return null;
        }

        private Member FindMember(int id)
        {
            foreach (Member m in members)
            {
                if (m.Id == id) return m;
            }
            return null;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Library library = new Library();
            bool running = true;

            while (running)
            {
                ShowMenu();
                int choice = ReadInt("Enter your choice: ");
                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        string title = ReadText("Enter book title: ");
                        string author = ReadText("Enter author name: ");
                        library.AddBook(title, author);
                        break;

                    case 2:
                        library.ViewAllBooks();
                        break;

                    case 3:
                        string keyword = ReadText("Enter title or author to search: ");
                        library.SearchBook(keyword);
                        break;

                    case 4:
                        int deleteId = ReadInt("Enter book ID to delete: ");
                        library.DeleteBook(deleteId);
                        break;

                    case 5:
                        string name = ReadText("Enter member name: ");
                        string phone = ReadText("Enter phone number: ");
                        library.AddMember(name, phone);
                        break;

                    case 6:
                        library.ViewAllMembers();
                        break;

                    case 7:
                        int bookId = ReadInt("Enter book ID: ");
                        int memberId = ReadInt("Enter member ID: ");
                        library.IssueBook(bookId, memberId);
                        break;

                    case 8:
                        int returnId = ReadInt("Enter book ID to return: ");
                        library.ReturnBook(returnId);
                        break;

                    case 9:
                        library.ViewIssuedBooks();
                        break;

                    case 0:
                        running = false;
                        Console.WriteLine("Thank you for using the Library System. Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                if (running)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=====================================");
            Console.WriteLine("     LIBRARY MANAGEMENT SYSTEM");
            Console.WriteLine("=====================================");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. View All Books");
            Console.WriteLine("3. Search Book");
            Console.WriteLine("4. Delete Book");
            Console.WriteLine("5. Add Member");
            Console.WriteLine("6. View All Members");
            Console.WriteLine("7. Issue Book");
            Console.WriteLine("8. Return Book");
            Console.WriteLine("9. View Issued Books");
            Console.WriteLine("0. Exit");
            Console.WriteLine("-------------------------------------");
        }

        static int ReadInt(string message)
        {
            int number;
            Console.Write(message);

            while (!int.TryParse(Console.ReadLine(), out number))
            {
                Console.Write("Invalid input. Please enter a number: ");
            }

            return number;
        }

        static string ReadText(string message)
        {
            Console.Write(message);
            string text = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(text))
            {
                Console.Write("This cannot be empty. Try again: ");
                text = Console.ReadLine();
            }

            return text.Trim();
        }
    }
}