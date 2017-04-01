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
            XElement authornames = XElement.Load("books.xml");
           // var authorFNames = from fname in authornames.Descendants("author") select fname.Attribute("firstName");
           // var authorLNames = from lname in authornames.Descendants("author") select lname;//.Attribute("lastName");
            var authorNames = from a in authornames.Descendants("author") select a;
            using(var db = new BookClubDB())
            {
                foreach(var i in authorNames)
                {
                    if (i.Attribute("firstName") != null)
                    {
                        author a = new author
                        {
                            FIRSTNAME = i.Attribute("firstName").Value,
                            LASTNAME = i.Attribute("lastName").Value
                        };
                        int[] authAttributes = { a.AUTHOR_ID};
                        if (db.authors.Find(a.AUTHOR_ID) == null)
                        {
                            db.authors.Add(a);
                        }

                    } 
                    if(i.Attribute("firstName") == null)
                    {
                        author a = new author
                        {
                            FIRSTNAME = "not available",
                            LASTNAME = i.Attribute("lastName").Value
                        };
                        int[] authAttributes = { a.AUTHOR_ID};
                        if (db.authors.Find(a.AUTHOR_ID) == null)
                        {
                            db.authors.Add(a);
                        }

                    }
                }
                db.SaveChanges();
            }
            foreach(var i in authorNames)
            {
                Console.WriteLine(i.Attribute("lastName").Value);
                
            }

            //Console.Write(authorFNames);
            Console.Read();
        }
    }
}
