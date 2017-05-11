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
    public class reviewsController : Controller
    {
        private Entities db = new Entities();

        // GET: reviews
        public ActionResult Index()
        {
            var reviews = db.reviews.Include("book").Include("user");
            return View(reviews.ToList());
        }

        // GET: reviews/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            review review = db.reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        [Authorize]
        // GET: reviews/Create
        public ActionResult Create(int? book_id)
        {
            ViewBag.BookTitle = db.books.Find(book_id).TITLE;
            ViewBag.BOOK_ID = book_id;
            ViewBag.USERNAME = new SelectList(db.users, "USERNAME", "PASSWORD");
            return View();
        }

        // POST: reviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BOOK_ID,RATING,CONTENT")] review review)
        {
            if (ModelState.IsValid)
            {
                review.USERNAME = User.Identity.Name;
                if(review.CONTENT == null)
                {
                    review.CONTENT = " ";
                }
                review.RATING -= 5;
                db.reviews.Add(review);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BOOK_ID = new SelectList(db.books, "BOOK_ID", "TITLE", review.BOOK_ID);
            ViewBag.USERNAME = new SelectList(db.users, "USERNAME", "PASSWORD", review.USERNAME);
            return View(review);
        }

        // GET: reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            review review = db.reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            ViewBag.BOOK_ID = new SelectList(db.books, "BOOK_ID", "TITLE", review.BOOK_ID);
            ViewBag.USERNAME = new SelectList(db.users, "USERNAME", "PASSWORD", review.USERNAME);
            return View(review);
        }

        // POST: reviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "REVIEW_ID,BOOK_ID,USERNAME,RATING,CONTENT")] review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BOOK_ID = new SelectList(db.books, "BOOK_ID", "TITLE", review.BOOK_ID);
            ViewBag.USERNAME = new SelectList(db.users, "USERNAME", "PASSWORD", review.USERNAME);
            return View(review);
        }

        // GET: reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            review review = db.reviews.Find(id);
            if (review == null)
            {
                return HttpNotFound();
            }
            return View(review);
        }

        // POST: reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            review review = db.reviews.Find(id);
            db.reviews.Remove(review);
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
