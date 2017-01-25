using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security;
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
            string userId = User.Identity.GetUserId();
            var names = new List<string> {"Project name"};
            foreach (var project in _db.Projects.Where(pr => pr.User.Id == userId))
                names.Add(project.Name);
            ViewBag.ProjectNames = new SelectList(names);

            return View();
        }

        public ActionResult BuildToDoTasksTable(string description = null)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);

            if (description != null)
            {
                IEnumerable<ToDoTask> tasks = _db.ToDoTasks.ToList().Where(task => task.User == currentUser && task.Description.Contains(description));
                return PartialView("_ToDoTaskTable", tasks);
            }
           
            return PartialView("_ToDoTaskTable", _db.ToDoTasks.ToList().Where(task => task.User == currentUser));
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProject(Project project)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
                project.User = currentUser;
                _db.Projects.Add(project);
                _db.SaveChanges();
                return PartialView("_CreateToDoTask");
            }

            return View("Index");
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateToDoTask(ToDoTask toDoTask, string projectName)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
                Project currentProject = _db.Projects.FirstOrDefault(pr => pr.Name == projectName);
                toDoTask.Project = currentProject;
                toDoTask.User = currentUser;
                _db.ToDoTasks.Add(toDoTask);
                _db.SaveChanges();
            }

            return RedirectToAction("BuildToDoTasksTable");
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

        // POST: ToDoTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            ToDoTask toDoTask = _db.ToDoTasks.Find(id);
            if (toDoTask != null) _db.ToDoTasks.Remove(toDoTask);
            _db.SaveChanges();
            return RedirectToAction("BuildToDoTasksTable");
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
