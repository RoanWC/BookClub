using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using loadBookClub;
using System.IO;
using System.Reflection;

namespace loadBookClub
{
    class Program
    {
        static void Main(string[] args)
        {

            XElement booksXML = XElement.Load(@"Files/books.xml");
            // var authorFNames = from fname in authornames.Descendants("author") select fname.Attribute("firstName");
            // var authorLNames = from lname in authornames.Descendants("author") select lname;//.Attribute("lastName");




            //create author objects and put them in a list of authors.
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
                        insertedNames.Add(authorname);
                        authorsList.Add(a);
                    }
                }
                if (i.Attribute("firstName") == null)
                {
                    author a = new author
                    {
                        FIRSTNAME = i.Attribute("firstName")?.Value,
                        LASTNAME = i.Attribute("lastName").Value,
                        books = new List<book>()
                    };
                    String authorname = "" + a.LASTNAME.ToString();
                    if (!(insertedNames.Contains(authorname)))
                    {
                        insertedNames.Add(authorname);
                        authorsList.Add(a);
                    }
                }
            }//done creating authors


            //create anonymous objects for the books and use them to create books and add them to the correct author before putting the books in a list
            var XMLbooks = from b in booksXML.Descendants("book") select b;
            List<Object> anonymousBooks = new List<Object>();
            List<book> allBooks = new List<book>();
            foreach (var b in XMLbooks)
            {

                var anonBook = new
                {
                    TITLE = b.Element("title").Value,
                    DESCRIPTION = b.Element("description").Value,
                    authorFName = b.Element("author").Attribute("firstName")?.Value,
                    authorLName = b.Element("author").Attribute("lastName").Value
                };
                anonymousBooks.Add(anonBook);
                book book = new book
                {
                    TITLE = anonBook.TITLE,
                    DESCRIPTION = anonBook.DESCRIPTION
                };
                allBooks.Add(book);

                var auth = authorsList.Where(a => a.FIRSTNAME == anonBook.authorFName && a.LASTNAME == anonBook.authorLName).FirstOrDefault();

                Console.WriteLine("alert");
                auth?.books.Add(book);

                Console.WriteLine(auth?.FIRSTNAME + " " + auth.LASTNAME + " wrote " + auth.books?.First().TITLE);
            }//end foreach 

            XElement ratingsXML = XElement.Load(@"Files/ratings.xml");
            List<user> userList = new List<user>();
            var xmlusers = from u in ratingsXML.Descendants("user") select u;
            foreach(var u in xmlusers)
            {
                user newUser = new user
                {
                    USERNAME = u.Attribute("userId").Value,
                    PASSWORD = u.Attribute("userId").Value,
                    FIRSTNAME = u.Attribute("userId").Value,
                    LASTNAME = u.Attribute("lastName")?.Value ?? "Reader",
                    COUNTRY = "CAN"
                };
                userList.Add(newUser);
                var reviews = u.Elements("review");
                foreach(var r in reviews)
                {
                    review newReview = new review
                    {
                        BOOK_ID = int.Parse(r.Attribute("bookId").Value),
                        book = allBooks[int.Parse(r.Attribute("bookId").Value)],
                        user = newUser,
                        RATING = int.Parse(r.Attribute("rating").Value),
                        CONTENT = ""
                    };
                    newUser.reviews.Add(newReview);
                }
                Console.Write("alert");            
            }
            Console.WriteLine("alert");

            //put the books and authors into the db set and save changes to the database
            using (var db = new BookClubDB())
            {

                 //i do not need to add the books to the dbset because they are all in author objects.                             
                foreach (author a in authorsList)
                {
                    db.authors.Add(a);
                }
                db.SaveChanges();
            }

            Console.WriteLine("Execution completed");
            Console.Read();
        }
    }
}

