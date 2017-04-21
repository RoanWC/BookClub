﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BookClubPage.Models;
using System.Data.Entity;


namespace BookClubPage.Controllers
{
    public class AccountController : Controller
    {
        private Entities db = new Entities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(String returnURL)
        {
            ViewBag.ReturnURL = returnURL;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName, Password")] user userIn, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                user user = (from u in db.users
                             where u.USERNAME.Equals(userIn.USERNAME)
                             select u).FirstOrDefault<user>();
                if (user != null)
                {
                    if (user.PASSWORD.Equals(userIn.PASSWORD))
                    {
                        FormsAuthentication.RedirectFromLoginPage(userIn.USERNAME, false);
                    }
                }
            }
                ViewBag.ReturnURL = ReturnUrl;
                ModelState.AddModelError("", "Invalid user name or password");
                return View();
            
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