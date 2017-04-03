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
                
                foreach (var i in aNames)
                {
                    if (i.Attribute("firstName") != null)
                    {
                        author a = new author
                        {
                            FIRSTNAME = i.Attribute("firstName").Value,
                            LASTNAME = i.Attribute("lastName").Value
                        };
                        String authorname = a.FIRSTNAME.ToString() + a.LASTNAME.ToString();
                        if (!(insertedNames.Contains(authorname)))
                        {
                            db.authors.Add(a);
                            insertedNames.Add(authorname);
                        }
                    }
                    if (i.Attribute("firstName") == null)
                    {
                        author a = new author
                        {
                            FIRSTNAME = "not available",
                            LASTNAME = i.Attribute("lastName").Value
                        };
                        String authorname = a.FIRSTNAME.ToString() + a.LASTNAME.ToString();
                        if (!(insertedNames.Contains(authorname)))
                        {
                            db.authors.Add(a);
                            insertedNames.Add(authorname);
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





                    author auth = db.authors.Where(a => a.FIRSTNAME == anonBook.authorFName && a.LASTNAME == anonBook.authorLName).First();
                    auth.books.Add(book);
                   /* var authors = from a in db.authors select a;
                     var anAuthor = (from a in authors
                                       where a.FIRSTNAME == anonBook.authorFName && a.LASTNAME == anonBook.authorLName
                                       select a);
                   // anAuthor.books.Add(book);*/
                }

                Console.WriteLine(anonymousBooks);

                //db.SaveChanges();
            }
            /*
            foreach(var i in authorNames)
            {
                if (i.Attribute("firstName") != null)
                    Console.Write(i.Attribute("firstName").Value + " ");
                else
                    Console.Write("N/A");
                Console.WriteLine(i.Attribute("lastName").Value);
                
            }
            */
            //Console.Write(authorFNames);
            Console.WriteLine("Execution completed");
            Console.Read();
        }
    }
}
