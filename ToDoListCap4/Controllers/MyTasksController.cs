using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDoListCap4.Models;

namespace ToDoListCap4.Controllers
{
    public class MyTasksController : Controller
    {
        private readonly MyTasksDbContext _myTasksDB;

        public MyTasksController(MyTasksDbContext myTasksContext)
        {
            _myTasksDB = myTasksContext;
        }
        // check startup.cs configure services method for addition of authorization

        public IActionResult Index(string sortOrder, string searchString)
        {
            // switch between two values to sort table, default sorts by date oldest first
            ViewData["completionSorting"] = sortOrder == "Completion" ? "comepletion_desc" : "Completion";
            ViewData["DateSorting"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            // search string to view certain records
            ViewData["DescriptionSearch"] = searchString;

            // Only show Logged User's tasks
            List<ToDos> allTasks = _myTasksDB.ToDos.ToList();
            string user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<ToDos> userTasks = allTasks.Where(x => x.UserId == user).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                userTasks = userTasks.Where(x=> x.ItemDesciption.ToLower().Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "date_desc":
                    userTasks.Sort((x, y) => DateTime.Compare(y.DueDate, x.DueDate));
                    break;
                case "Completion":
                    userTasks = (from task in userTasks
                                orderby task.IsComplete
                                descending
                                select task).ToList();
                    break;
                case "comepletion_desc":
                    userTasks = (from task in userTasks
                                 orderby task.IsComplete
                                 select task).ToList();
                    break;
                default:
                    userTasks.Sort((x, y) => DateTime.Compare(x.DueDate, y.DueDate));
                    break;
            }
            return View(userTasks);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(ToDos task)
        {
            task.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                _myTasksDB.ToDos.Add(task);
                _myTasksDB.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public IActionResult Delete(int id)
        {
            //Cat cat = new Cat();
            ToDos td = _myTasksDB.ToDos.Find(id);

            return View(td);
        }

        [HttpPost]
        public IActionResult Delete(ToDos td)
        {
            if (ModelState.IsValid)
            {
                _myTasksDB.ToDos.Remove(td);
                _myTasksDB.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            ToDos td = _myTasksDB.ToDos.Find(id);
            return View(td);
        }

        [HttpPost]
        public IActionResult Update(ToDos td)
        {
            if (ModelState.IsValid)
            {
                //updating user record
                string user = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                td.UserId = user;
                td.IsComplete = true;
                _myTasksDB.ToDos.Update(td);
                _myTasksDB.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
