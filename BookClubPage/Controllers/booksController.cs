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
                    sortedBooks = db.books.Include("authors.reviews").OrderBy(e => e.TITLE).Select(e => e).ToList();
                    break;
                case "rating":
                    sortedBooks = db.books.Include("authors.reviews").OrderBy(e => e.reviews.Select(r => r.RATING).Average()).Select(e => e).ToList();
                    sortedBooks.Reverse();
                    break;
                case "author":
                    sortedBooks = db.books.Include("authors.reviews").OrderBy(e => e.authors.Select(a => a.LASTNAME).FirstOrDefault()).Select(a => a).ToList();
                    break;
                case "views":
                    sortedBooks = db.books.Include("authors.reviews").OrderBy(b => b.VIEWS).Select(b => b).ToList();
                    sortedBooks.Reverse();
                    break;
                case "recomended":
                    if (User.Identity.IsAuthenticated)
                    {
                        if (db.users.Find(User.Identity.Name).reviews.Count != 0)
                        {
                            user bestUser = null;
                            double bestDotProd = 0;
                            foreach (user u in db.users)
                            {
                                if (u.USERNAME == User.Identity.Name)
                                {
                                    continue;
                                }

                                double currentDotProduct = 0;
                                foreach (book b in db.books)
                                {
                                    currentDotProduct += Convert.ToDouble((b.reviews.Where(r => r.user.USERNAME == User.Identity.Name).Select(r => r.RATING).FirstOrDefault()) ?? 0) *
                                                        Convert.ToDouble((b.reviews.Where(r => r.user.USERNAME == u.USERNAME).Select(r => r.RATING).FirstOrDefault()) ?? 0);
                                }
                                if (bestDotProd < currentDotProduct)
                                {
                                    bestDotProd = currentDotProduct;
                                    bestUser = u;
                                }
                            }
                            List<book> bestUserBooks = bestUser?.reviews.Select(r => r.book).ToList();
                            List<book> usersBooks = db.reviews.Where(r => r.USERNAME == User.Identity.Name).Select(r => r.book).ToList();
                            List<book> recomendedBooks = new List<book>();
                            recomendedBooks = bestUserBooks.Where(b => !usersBooks.Contains(b)).Select(b => b).ToList();
                            sortedBooks = recomendedBooks;
                        }else
                        {
                            ViewBag.recomendedErrorMessage = "Leave some reviews so we can recomend some books to you!";
                        }
                    }else
                    {
                        ViewBag.recomendedErrorMessage = "Log in so you can get awesome recomendations!";
                    }

                    break;
                default:
                    sortedBooks = db.books.Select(e => e).ToList();
                    break;
            }



            List<List<book>> listBookLists = new List<List<book>>(10);

            for (int i = 0; i < sortedBooks.Count; i++)
            {
                listBookLists.Add(new List<book>(sortedBooks.Take(10)));
                if(sortedBooks.Count > 10)
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
        public ActionResult Details(int? id,int? pageNum)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            book book = db.books.Where(b => b.BOOK_ID == id).Include("reviews").Include("authors").Select(b => b).FirstOrDefault();
            if (book == null)
            {
                return HttpNotFound();
            }

            book.VIEWS++;
            db.SaveChanges();

            int PageNum = pageNum ?? 0;

            List<review> reviewList = new List<review>(book.reviews);
            List<List<review>> reviewListList = new List<List<review>>();

            for (int i = 0; i < reviewList.Count; i++)
            {
                reviewListList.Add(new List<review>(reviewList.Take(5)));
                if (reviewList.Count > 5)
                {
                    reviewList.RemoveRange(0, 5);
                }
            }
            reviewListList.Add(new List<review>(reviewList));
            if (PageNum < 0)
            {
                PageNum = reviewListList.Count - 1;
            }

            if (PageNum > reviewListList.Count - 1)
            {
                PageNum = 0;
            }

            ViewBag.PageNumber = PageNum;
            ViewBag.Reviews = reviewListList[PageNum];
            double averageRating = Convert.ToDouble((from r in book.reviews select r.RATING).Average()) +  5;
            ViewBag.Average = averageRating;

            return View(book);
        }
        [Authorize]
        // GET: books/Create
        public ActionResult Create()
        {
            ViewBag.authors = db.authors.Select(a => a).ToList();
            return View();
        }

        // POST: books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BOOK_ID,TITLE,DESCRIPTION,VIEWS")] book book,int authorID)
        {
            if (ModelState.IsValid)
            {
                author a = db.authors.Find(authorID);
                book.authors.Add(a);
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
