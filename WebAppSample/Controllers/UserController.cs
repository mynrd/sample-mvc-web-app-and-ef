using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebAppSample.DataContextObject;
using WebAppSample.Models;

namespace WebAppSample.Controllers
{
    // to detect if you are logged
    //if no attribute "Authorize" to Controller or to ActionResult on top, anyone can view the page like the HomeController
    [Authorize]
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new MyDbContext())
            {
                var model = context.Users
                    .OrderBy(x => x.Username)
                    .Select(x => new UserModel()
                    {
                        Id = x.Id,
                        Username = x.Username
                    }).ToList();

                return View(model);
            }
        }

        #region User CRUD
        public ActionResult Edit(Guid id)
        {
            using (var context = new MyDbContext())
            {
                var model = context.Users
                    .Where(x => x.Id == id)
                    .Select(x => new UserModel()
                    {
                        Id = x.Id,
                        Username = x.Username
                    }).FirstOrDefault();

                return View("CreateEdit", model);
            }
        }

        [HttpPost]
        public ActionResult Edit(Guid id, UserModel model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new MyDbContext())
                {
                    var data = context.Users.Where(x => x.Id == id).FirstOrDefault();
                    data.Username = model.Username;
                    context.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View("CreateEdit", model);
        }

        public ActionResult Delete(Guid id)
        {
            using (var context = new MyDbContext())
            {
                var data = context.Users.Where(x => x.Id == id).FirstOrDefault();
                context.Entry(data).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var model = new UserModel();
            return View("CreateEdit", model);
        }

        [HttpPost]
        public ActionResult Create(UserModel model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new MyDbContext())
                {
                    context.Users.Add(new User()
                    {
                        Username = model.Username
                    });
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View("CreateEdit", model);
        }

        #endregion User CRUD

        #region Login and Logout

        [AllowAnonymous] //this will override the Authorize to allow any user to view the page
        public ActionResult Login(string ReturnUrl)
        {
            var model = new UserLogin();
            return View(model);
        }

        [AllowAnonymous] //this will override the Authorize to allow any user to view the page
        [HttpPost]
        public ActionResult Login(UserLogin model, string ReturnUrl)
        {
            if (ModelState.IsValid) //this will check if UserLogin model data is valid
            {
                using (var context = new MyDbContext())
                {
                    var checkIfUserExists = context.Users.Any(x => x.Username == model.Username);
                    if (checkIfUserExists)
                    {
                        FormsAuthentication.SetAuthCookie(model.Username, true);
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("Username", "Username doesnt exists");
                    }
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("~/");
        }

        #endregion Login and Logout
    }
}