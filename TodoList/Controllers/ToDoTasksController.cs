using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using TodoList.Entities;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Authorize]
    public class ToDoTasksController : Controller
    {
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: ToDoTasks
        public ActionResult Index()
        {
            var names = new List<string> {"Project name"};
            string userId = User.Identity.GetUserId();
            foreach (var project in _db.Projects.Where(pr => pr.User.Id == userId))
                names.Add(project.Name);
            ViewBag.ProjectNames = new SelectList(names);

            return View();
        }

        public ActionResult BuildToDoTasksTable()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
            return PartialView("_ToDoTaskTable", _db.ToDoTasks.ToList().Where(task => task.User == currentUser));
        }

        public PartialViewResult CreateToDoTask()
        {
            return PartialView("_CreateToDoTask");
        }

        public PartialViewResult CreateProject()
        {
            return PartialView("_CreateProject");
        }

        public PartialViewResult SearchToDoTask()
        {
            return PartialView("_SearchToDoTask");
        }

        // GET: ToDoTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoTask toDoTask = _db.ToDoTasks.Find(id);
            if (toDoTask == null)
            {
                return HttpNotFound();
            }
            return View(toDoTask);
        }

        // GET: ToDoTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ToDoTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Description")] ToDoTask toDoTask)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
                toDoTask.User = currentUser;
                _db.ToDoTasks.Add(toDoTask);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toDoTask);
        }

        // GET: ToDoTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoTask toDoTask = _db.ToDoTasks.Find(id);
            if (toDoTask == null)
            {
                return HttpNotFound();
            }
            return View(toDoTask);
        }

        // POST: ToDoTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,IsDone")] ToDoTask toDoTask)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(toDoTask).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDoTask);
        }

        // GET: ToDoTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoTask toDoTask = _db.ToDoTasks.Find(id);
            if (toDoTask == null)
            {
                return HttpNotFound();
            }
            return View(toDoTask);
        }

        // POST: ToDoTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToDoTask toDoTask = _db.ToDoTasks.Find(id);
            _db.ToDoTasks.Remove(toDoTask);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
