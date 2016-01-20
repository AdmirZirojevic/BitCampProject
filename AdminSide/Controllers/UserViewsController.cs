using BitClassroom.DAL.Models;
using CloudinaryDotNet;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using System.Security.Claims;
using System.Web.Security;
using BitClassroom.MVC.Models.Manage;


namespace BitClassroom.MVC.Controllers
{
    public class UserViewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: UserViews
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortingOrder, string currentFilter, string searchString, int? page)
        {

            var users = db.Users.ToList();
            List<UserViewModel> result = new List<UserViewModel>();

            foreach (var item in users)
            {
                var model = new UserViewModel();
                model.Id = item.Id;
                model.Name = item.Name;
                model.Lastname = item.Lastname;
                model.Email = item.Email;
                model.SkypeName = item.SkypeName;

                var roleId = item.Roles.FirstOrDefault().RoleId;
                model.Role = GetRoleById(roleId).Name;
            
                result.Add(model);
              
            }

           
            ViewBag.CurrentSort = sortingOrder;

            ViewBag.UserNameSort = sortingOrder == "userNameAscending" ? "userNameDescending" : "userNameAscending";
            ViewBag.UserLastnameSort = sortingOrder == "userLastnameAscending" ? "userLastnameDescending" : "userLastnameAscending";
            ViewBag.EmailSort = sortingOrder == "emailAscending" ? "emailDescending" : "emailAscending";
            ViewBag.RoleSort = sortingOrder == "roleAscending" ? "roleDescending" : "roleAscending";


            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var listOfUsers = from x in result select x;

            if (!String.IsNullOrEmpty(searchString) && searchString.Trim()!="")
            {
                listOfUsers = listOfUsers.Where(x => x.Name.ToLower().Contains(searchString.ToLower())
                                       || x.Lastname.ToLower().Contains(searchString.ToLower())
                                       || x.Email.ToLower().Contains(searchString.ToLower())
                                       || x.Role.ToLower().Contains(searchString.ToLower()));
            }

            switch (sortingOrder)
            {
                case "userNameAscending":
                    listOfUsers = listOfUsers.OrderBy(x => x.Name);
                    break;
                case "userNameDescending":
                    listOfUsers = listOfUsers.OrderByDescending(x => x.Lastname);
                    break;
                case "userLastnameAscending":
                    listOfUsers = listOfUsers.OrderBy(x => x.Lastname);
                    break;
                case "userLastnameDescending":
                    listOfUsers = listOfUsers.OrderByDescending(x => x.Lastname);
                    break;
                case "emailAscending":
                    listOfUsers = listOfUsers.OrderBy(x => x.Email);
                    break;
                case "emailDescending":
                    listOfUsers = listOfUsers.OrderByDescending(x => x.Email);
                    break;
                case "roleAscending":
                    listOfUsers = listOfUsers.OrderBy(x => x.Role);
                    break;
                case "roleDescending":
                    listOfUsers = listOfUsers.OrderByDescending(x => x.Role);
                    break;
                default:
                    listOfUsers = listOfUsers.OrderBy(x => x.Id);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);  //means that if page is null set pageNumber to 1
            return View(listOfUsers.ToPagedList(pageNumber, pageSize));                         
           
           // return View("Index", result);
        }

        private IdentityRole GetRoleById(string roleId)
        {
            return db.Roles.Find(roleId);
        }

        public ActionResult Details(string userId)
        {
            var users = db.Users.ToList();

            var user = users.Find(x => x.Id == userId);

            UserViewModel uvmodel = new UserViewModel();
            uvmodel.Id = user.Id;
            uvmodel.Email = user.Email;
            uvmodel.Name = user.Name;
            uvmodel.Lastname = user.Lastname;
            uvmodel.SkypeName = uvmodel.SkypeName;
            uvmodel.Role = GetRoleById(user.Roles.FirstOrDefault().RoleId).Name;

            return View("Details", uvmodel);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(userId);      
            var userRoleName = GetRoleById(user.Roles.FirstOrDefault().RoleId).Name;

            if (userRoleName == "Student")
            {
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Name == "Student"), "Id", "Name");
            }
            if (userRoleName == "Admin")
            {
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Name == "Admin"), "Id", "Name");
            }
            if (userRoleName == "Mentor" || userRoleName=="Teacher")
            {
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Name == "Mentor" || x.Name=="Teacher"), "Id", "Name");
            }
            //else
            //    ViewBag.RoleId = new SelectList(db.Roles, "Id", "Name");

            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,Lastname,Email,UserName,Roles,SkypeName")] ApplicationUser user, string roleId)
        {
            var roles = db.Users.Where(r => r.Id == user.Id).Select(r => r.Roles).ToList();
            var role1 = roles.ElementAt(0);
            var role2 = role1.ElementAt(0);
            var role = role2.RoleId;

            var currentRoleName = db.Roles.Where(r => r.Id == role).SingleOrDefault().Name;
            var newRoleName = db.Roles.Where(r => r.Id == roleId).SingleOrDefault().Name;

            if (currentRoleName == "Student")
            {
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Name == "Student"), "Id", "Name");
            }
            if (currentRoleName == "Admin")
            {
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Name == "Admin"), "Id", "Name");
            }
            if (currentRoleName == "Mentor" || currentRoleName == "Teacher")
            {
                ViewBag.RoleId = new SelectList(db.Roles.Where(x => x.Name == "Mentor" || x.Name == "Teacher"), "Id", "Name");
            }

            if (!CustomUserManager.CanChangeToRole(currentRoleName,newRoleName))
            {
                return View(user);
            }

            if (ModelState.IsValid)
            {
                var toSave = db.Users.Find(user.Id);
                toSave.Name = user.Name;
                toSave.Lastname = user.Lastname;
                toSave.Email = user.Email;
                toSave.SkypeName = user.SkypeName;
                toSave.Roles.Clear();


                //ovu "najbolju validaciju ikad" mijenjat u svakom slucaju, sad za sad neka stoji
                if (String.IsNullOrWhiteSpace(toSave.Email))
                    return Redirect("Edit?userId=" + user.Id);

                if (toSave.Email.Contains(" "))
                    return Redirect("Edit?userId=" + user.Id);

                if(toSave.Email.StartsWith("@bitcamp.ba"))
                    return Redirect("Edit?userId=" + user.Id);

                int count = toSave.Email.Split('@').Length;
                string userID = db.Users.Find(user.Id).ToString();
                if (count > 2 || toSave.Email.EndsWith("@bitcamp.ba") == false || String.IsNullOrWhiteSpace(toSave.Name) || String.IsNullOrWhiteSpace(toSave.Lastname))
                {
                    return Redirect("Edit?userId=" + user.Id);
                }

                IdentityUserRole newRole = new IdentityUserRole();
                newRole.RoleId = roleId;
                newRole.UserId = toSave.Id;
                toSave.Roles.Add(newRole);
              
                db.Entry(toSave).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(user);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string userId)
        {
            if (userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            string adminId = this.User.Identity.GetUserId();
            if (userId == adminId)
            {
                return RedirectToAction("Index");
            }
        
            var adminRole = db.Roles.Where(x => x.Name.Contains("Admin")).FirstOrDefault();
            var userRoleName = GetRoleById(user.Roles.FirstOrDefault().RoleId).Name;

            if (userRoleName == "Admin" && (db.Users.Where(x => x.Roles.Select(y => y.RoleId).Contains(adminRole.Id)).Count()) <= 2)
            {
               
                return RedirectToAction("Index");
            }

            return View("Delete", user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string userId)
        {
            var user = db.Users.Find(userId);
            var role = db.Roles.Select(u => u.Users.Where(x => x.UserId == userId)).ToList();
            

            UserCascadeDeleteManager userManager = new UserCascadeDeleteManager();
            userManager.DeleteUserConnections(userId);

            db.Users.Remove(user);

            try   //ovo ovdje stoji jer ako neko pokusa izbrisat prvog admina sto je napravljen a ima vise od 2 admina padne ovdje, pa nek stoji ovo sad za sad
            {
                db.SaveChanges();
            }
            catch
            {
                return View("Error");
            }


            //this block of code deletes the image from cloudinary, after the course is deleted
            Account acount = new Account("gigantor", "986286566519458", "GT87e1BTMnfLut1_gXhSH0giZPg");
            Cloudinary cloudinary = new Cloudinary(acount);

            if (user.ProfilePicUrl != null && user.ProfilePicUrl.StartsWith("http://res.cloudinary.com/gigantor/image/upload/"))
            {
                //this here is just a string manipulation to get to the ImageID from cloudinary
                string assist = "http://res.cloudinary.com/gigantor/image/upload/";
                string part1 = user.ProfilePicUrl.Remove(user.ProfilePicUrl.IndexOf(assist), assist.Length);
                string part2 = part1.Remove(0, 12);
                string toDelete = part2.Remove(part2.Length - 4);
                cloudinary.DeleteResources(toDelete);  //this finally deletes the image
            }

            return RedirectToAction("Index");
        }

    }
}