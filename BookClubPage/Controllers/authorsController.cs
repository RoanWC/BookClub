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
    public class authorsController : Controller
    {
        private Entities db = new Entities();

        // GET: authors
        public ActionResult Index()
        {
            return View(db.authors.ToList());
        }

        // GET: authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            author author = db.authors.Include("books").Where(a => a.AUTHOR_ID == id).Select(a => a).FirstOrDefault();
            //author author = db.authors.Find(id);
            List<book> lBooks= new List<book>();
            lBooks = author.books.ToList();
            ViewBag.authBooks = lBooks;
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }
        [Authorize]
        // GET: authors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FIRSTNAME,LASTNAME")] author author)
        {
            if (ModelState.IsValid)
            {
                if (db.authors.Where(a => a.FIRSTNAME == author.FIRSTNAME && a.LASTNAME == author.LASTNAME).Select(a => a).Count() == 0)
                {
                    db.authors.Add(author);
                    db.SaveChanges();
                    return RedirectToAction("Details", new { id = author.AUTHOR_ID});
                }
                else
                {
                    ViewBag.authorExistsError = "This author already exists";
                }
            }

            return View(author);
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
