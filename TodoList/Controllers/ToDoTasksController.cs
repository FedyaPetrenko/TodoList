using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
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
            UpdateViewBagListOfProjects(userId);

            return View();
        }

        public ActionResult BuildToDoTasksTable(string description = null)
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
            
            //Initialize Mapper
            Mapper.Initialize(cfg => cfg.CreateMap<ToDoTask, ToDoTaskViewModel>());

            if (description != null)
            {
                IEnumerable<ToDoTask> searchTasks = _db.ToDoTasks.ToList().Where(task => task.User == currentUser && task.Description.Contains(description));
                var searchTaskViewModels = Mapper.Map<IEnumerable<ToDoTask>, IEnumerable<ToDoTaskViewModel>>(searchTasks);
                return PartialView("_ToDoTaskTable", searchTaskViewModels);
            }

            IEnumerable<ToDoTask> tasks = _db.ToDoTasks.ToList().Where(task => task.User == currentUser);
            var taskViewModels = Mapper.Map<IEnumerable<ToDoTask>, IEnumerable<ToDoTaskViewModel>>(tasks);
            return PartialView("_ToDoTaskTable", taskViewModels);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProject(ProjectViewModel projectViewModel)
        {
            if (ModelState.IsValid)
            {
                //Initialize Mapper
                Mapper.Initialize(cfg => cfg.CreateMap<ProjectViewModel, Project>());
                var project = Mapper.Map<ProjectViewModel, Project>(projectViewModel);

                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
                project.User = currentUser;
                _db.Projects.Add(project);
                _db.SaveChanges();

                UpdateViewBagListOfProjects(userId);
            }

            return RedirectToAction("Index");
        }

        private void UpdateViewBagListOfProjects(string userId)
        {
            var names = new List<string> {"Project name"};
            foreach (var proj in _db.Projects.Where(pr => pr.User.Id == userId))
                names.Add(proj.Name);
            ViewBag.ProjectNames = new SelectList(names);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateToDoTask(ToDoTaskViewModel toDoTaskViewModel)
        {
            if (ModelState.IsValid && toDoTaskViewModel.ProjectName != "Project name")
            {
                //Initialize Mapper
                Mapper.Initialize(cfg => cfg.CreateMap<ToDoTaskViewModel, ToDoTask>());
                var toDoTask = Mapper.Map<ToDoTaskViewModel, ToDoTask>(toDoTaskViewModel);

                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
                Project currentProject = _db.Projects.FirstOrDefault(pr => pr.Name == toDoTaskViewModel.ProjectName);
                toDoTask.Project = currentProject;
                toDoTask.User = currentUser;
                _db.ToDoTasks.Add(toDoTask);
                _db.SaveChanges();
            }
            //ModelState.AddModelError("ProjectName", "The project name is required.");
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
            //Initialize Mapper
            Mapper.Initialize(cfg => cfg.CreateMap<ToDoTask, ToDoTaskViewModel>());
            var toDoTaskViewModel = Mapper.Map<ToDoTask, ToDoTaskViewModel>(toDoTask);
            return View(toDoTaskViewModel);
        }

        // POST: ToDoTasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ToDoTaskViewModel toDoTaskViewModel)
        {
            if (ModelState.IsValid)
            {
                //Initialize Mapper
                Mapper.Initialize(cfg => cfg.CreateMap<ToDoTaskViewModel, ToDoTask>());
                var toDoTask = Mapper.Map<ToDoTaskViewModel, ToDoTask>(toDoTaskViewModel);

                _db.Entry(toDoTask).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDoTaskViewModel);
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
