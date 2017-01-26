using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TodoList.Entities;
using TodoList.Models;

namespace TodoList.Controllers
{
    [Authorize]
    public class ToDoTasksController : Controller
    {
        //Database context
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: ToDoTasks
        public ActionResult Index()
        {
            //Get user`s info and projects
            string userId = User.Identity.GetUserId();
            UpdateViewBagListOfProjects(userId);

            return View();
        }

        public ActionResult BuildToDoTasksTable(string description)
        {
            //Get user`s info
            string userId = User.Identity.GetUserId();
            ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);

            //Initialize Mapper
            Mapper.Initialize(cfg => cfg.CreateMap<ToDoTask, ToDoTaskViewModel>());

            //Check for search task
            if (description != null)
            {
                //Get searched tasks
                IEnumerable<ToDoTask> searchTasks =
                    _db.ToDoTasks.ToList()
                        .Where(task => task.User == currentUser && task.Description.Contains(description));

                //Map to IEnumerable<ToDoTaskViewModel>
                var searchTaskViewModels = Mapper.Map<IEnumerable<ToDoTask>, IEnumerable<ToDoTaskViewModel>>(searchTasks);

                //Show updated table
                return PartialView("_ToDoTaskTable", searchTaskViewModels);
            }

            //Get all user`s tasks
            IEnumerable<ToDoTask> tasks = _db.ToDoTasks.ToList().Where(task => task.User == currentUser);

            //Map to IEnumerable<ToDoTaskViewModel>
            var taskViewModels = Mapper.Map<IEnumerable<ToDoTask>, IEnumerable<ToDoTaskViewModel>>(tasks);

            //Show table with data
            return PartialView("_ToDoTaskTable", taskViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProject(ProjectViewModel projectViewModel)
        {
            if (ModelState.IsValid)
            {
                //Initialize Mapper
                Mapper.Initialize(cfg => cfg.CreateMap<ProjectViewModel, Project>());
                var project = Mapper.Map<ProjectViewModel, Project>(projectViewModel);

                //Get user`s info
                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
                project.User = currentUser;

                //Insert project into database and save
                _db.Projects.Add(project);
                _db.SaveChanges();

                UpdateViewBagListOfProjects(userId);
            }

            return RedirectToAction("Index");
        }

        //Update dropdown list with new projects
        private void UpdateViewBagListOfProjects(string userId)
        {
            //Get all projects for dropdown list
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
            //Check for valid model and check for selected project
            if (ModelState.IsValid && toDoTaskViewModel.ProjectName != "Project name")
            {
                //Initialize Mapper
                Mapper.Initialize(cfg => cfg.CreateMap<ToDoTaskViewModel, ToDoTask>());

                //Map to ToDoTask
                var toDoTask = Mapper.Map<ToDoTaskViewModel, ToDoTask>(toDoTaskViewModel);

                //Get user`s info
                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);
                Project currentProject = _db.Projects.FirstOrDefault(pr => pr.Name == toDoTaskViewModel.ProjectName);

                //Initialize ToDoTask with Project and User
                toDoTask.Project = currentProject;
                toDoTask.User = currentUser;

                //Insert new task into database and save
                _db.ToDoTasks.Add(toDoTask);
                _db.SaveChanges();
            }
           
            return RedirectToAction("BuildToDoTasksTable");
        }

        // GET: ToDoTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            ToDoTask toDoTask = _db.ToDoTasks.Find(id);

            if (toDoTask == null)
                return HttpNotFound();

            //Initialize Mapper
            Mapper.Initialize(cfg => cfg.CreateMap<ToDoTask, ToDoTaskViewModel>());

            //Map to ToDoTaskViewModel
            var toDoTaskViewModel = Mapper.Map<ToDoTask, ToDoTaskViewModel>(toDoTask);

            toDoTaskViewModel.ProjectName = toDoTask.Project.Name;
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

                //Map to TodoTask
                var toDoTask = Mapper.Map<ToDoTaskViewModel, ToDoTask>(toDoTaskViewModel);

                //Get user`s info
                string userId = User.Identity.GetUserId();
                ApplicationUser currentUser = _db.Users.FirstOrDefault(user => user.Id == userId);

                //Initialize ToDoTask with Project and User
                toDoTask.Project = _db.Projects.FirstOrDefault(pr => pr.Name == toDoTaskViewModel.ProjectName);
                if (toDoTask.Project != null) toDoTask.ProjectId = toDoTask.Project.Id;
                toDoTask.User = currentUser;
                toDoTask.UserId = userId;

                //Save ToDoTask to database
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

        //Save data form database to file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveToFile()
        {
            //Get current user info
            string userId = User.Identity.GetUserId();

            //Get user`s tasks
            List<ToDoTask> tasks = _db.ToDoTasks.Where(task => task.UserId == userId).ToList();

            // Serialize list to JSON
            string jsonData = JsonConvert.SerializeObject(tasks, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            //Write text to file in directory App_Data
            using (StreamWriter writer = System.IO.File.CreateText(Server.MapPath("~/App_Data/ToDoTasks.json")))
            {
                await writer.WriteAsync(jsonData);
            }

            return new JavaScriptResult() {Script = "alert('Data successfully saved to file!');"};
          }

        //Get Json data from file
        [HttpGet]
        [AllowAnonymous]
        public async Task<JArray> GetJsonData()
        {
            string jsonData;

            //Create or open file in directory App_Data and read
            using (FileStream fileStream = new FileStream(Server.MapPath("~/App_Data/ToDoTasks.json"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                StreamReader reader = new StreamReader(fileStream);
                jsonData = await reader.ReadToEndAsync();
            }

            // Deserialize text 
            var tasks = (JArray)JsonConvert.DeserializeObject(jsonData, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return tasks;
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