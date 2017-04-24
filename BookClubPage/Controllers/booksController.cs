using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BookClubPage.Models;

namespace BookClubPage.Controllers
{
    public class booksController : Controller
    {
        private Entities db = new Entities();

        // GET: books
        public ActionResult Index(String sorting, int? pageNum)
        {
            int page = pageNum ?? 0;


            List<book> sortedBooks = new List<book>();
            switch (sorting)
            {
                case "title":
                    sortedBooks = db.books.OrderBy(e => e.TITLE).Select(e => e).ToList();
                    break;
                case "rating":
                    sortedBooks = db.books.OrderBy(e => e.reviews.Select(r => r.RATING).Average()).Select(e => e).ToList();
                    break;
                case "author":
                    sortedBooks = db.books.OrderBy(e => e.authors.Select(a => a.LASTNAME).FirstOrDefault()).Select(a => a).ToList();
                    break;
                case "views":
                    sortedBooks = db.books.OrderBy(b => b.VIEWS).Select(b => b).ToList();
                    break;
                default:
                    sortedBooks = db.books.Select(e => e).ToList();
                    break;
            }



            List<List<book>> listBookLists = new List<List<book>>(10);

            for (int i = 0; i < sortedBooks.Count; i++)
            {
                listBookLists.Add(new List<book>(sortedBooks.Take(10)));
                sortedBooks.RemoveRange(0, 10);

            }
            listBookLists.Add(new List<book>(sortedBooks));
            if(page < 0)
            {
                page = listBookLists.Count - 1;
            }
            if(page > listBookLists.Count - 1)
            {
                page = 0;
            }

            ViewBag.PageNumber = page;
            ViewBag.Sorting = sorting;
            return View(listBookLists[page]);
        }

        // GET: books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            int averageRating = Convert.ToInt32((from r in book.reviews select r.RATING).Average());
            ViewBag.Average = averageRating;
            return View(book);
        }
        [Authorize]
        // GET: books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BOOK_ID,TITLE,DESCRIPTION,VIEWS")] book book)
        {
            if (ModelState.IsValid)
            {
                db.books.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }
        [Authorize]
        // GET: books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BOOK_ID,TITLE,DESCRIPTION,VIEWS")] book book)
        {
            if (ModelState.IsValid)
            {
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: books/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: books/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            book book = db.books.Find(id);
            db.books.Remove(book);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
