using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace loadBookClub
{
    class Program
    {
        static void Main(string[] args)
        {
            XElement booksXML = XElement.Load("books.xml");
            // var authorFNames = from fname in authornames.Descendants("author") select fname.Attribute("firstName");
            // var authorLNames = from lname in authornames.Descendants("author") select lname;//.Attribute("lastName");
            
            


            using (var db = new BookClubDB())
            {
                

                //create all of the author objects
                var authorNames = from a in booksXML.Descendants("author") select a;
                List<String> insertedNames = new List<string>();
                var aNames = from a in authorNames select a;
                List<author> authorsList = new List<author>();

                foreach (var i in aNames)
                {
                    if (i.Attribute("firstName") != null)
                    {
                        author a = new author
                        {
                            FIRSTNAME = i.Attribute("firstName").Value,
                            LASTNAME = i.Attribute("lastName").Value,
                            books = new List<book>()
                        };
                        String authorname = a.FIRSTNAME.ToString() + a.LASTNAME.ToString();
                        if (!(insertedNames.Contains(authorname)))
                        {
                            db.authors.Add(a);
                            insertedNames.Add(authorname);
                            authorsList.Add(a);
                        }
                    }
                    if (i.Attribute("firstName") == null)
                    {
                        author a = new author
                        {
                            FIRSTNAME = "not available",
                            LASTNAME = i.Attribute("lastName").Value,
                            books = new List<book>()
                        };
                        String authorname = a.FIRSTNAME.ToString() + a.LASTNAME.ToString();
                        if (!(insertedNames.Contains(authorname)))
                        {
                            db.authors.Add(a);
                            insertedNames.Add(authorname);
                            authorsList.Add(a);
                        }
                    }          
                }//done creating authors
                                              
                //create anonymous objects for the books
                var XMLbooks = from b in booksXML.Descendants("book") select b;
                List<Object> anonymousBooks = new List<Object>();
                List<book> allBooks = new List<book>();
                foreach(var b in XMLbooks)
                {
                    var anonBook = new
                    {
                        TITLE = b.Element("title").Value,
                        DESCRIPTION = b.Element("description").Value,
                        authorFName = b.Element("author").Attribute("firstName")?.Value ,
                        authorLName = b.Element("author").Attribute("lastName").Value
                    };
                    anonymousBooks.Add(anonBook);
                    book book = new book
                    {
                        TITLE = anonBook.TITLE,
                        DESCRIPTION = anonBook.DESCRIPTION
                    };
                    allBooks.Add(book);
                    db.books.Add(book);

                    var auth = db.authors.Where(a => a.FIRSTNAME == anonBook.authorFName && a.LASTNAME == anonBook.authorLName).FirstOrDefault();
                    if(auth != null)
                    auth.books.Add(book);
                }
              
                foreach (author author in db.authors)
                {
                    Console.WriteLine(author.FIRSTNAME + " " + author.LASTNAME + " wrote" + author.books.First());
                }
                db.SaveChanges();
            }
            
            Console.WriteLine("Execution completed");
            Console.Read();
        }
    }
}
