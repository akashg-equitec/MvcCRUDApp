using CrudApp.Models;
using CrudApp.Repo;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CrudApp.Controllers
{
    public class StudentsController : Controller
    {

        DapperCrud dapObj = new DapperCrud();

    


        
        public ActionResult Show(int page = 1)
        {
            int pageSize = 5;
            int count = (page - 1) * pageSize + 1;
            ViewBag.c = count;
            var emp = dapObj.ShowData();
            var Employees = emp.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalCount = emp.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(Employees);
        }





      

   
        public ActionResult Create()
        {
            using (var con = new SqlConnection(dapObj.conn))
            {
                con.Open();
                string sql = "SELECT * FROM Departments";
                var temp = con.Query<StudentModel>(sql);
                ViewBag.dept = temp;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(StudentModel std)
        {
            try
            {
                dapObj.InsertStudent(std);
                TempData["SuccessMessage"] = "Student added successfully!";
                return RedirectToAction("Show");
            }
            catch (Exception ex)
            { 
                TempData["ErrorMessage"] = ex.Message;
                using (var con = new SqlConnection(dapObj.conn))
                {
                    con.Open();
                    string sql = "SELECT * FROM Departments";
                    var temp = con.Query<StudentModel>(sql);
                    ViewBag.dept = temp;
                }
                return View(std);
            }
        }







        // update code
        public ActionResult Update(int? StudentId)
        {
            if (!StudentId.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "StudentId is required.");
            }

            using (var con = new SqlConnection(dapObj.conn))
            {
                con.Open();
                string departmentSql = @"SELECT DepartmentId, DepartmentName FROM Departments";
                var departments = con.Query<StudentModel>(departmentSql); 
                ViewBag.dept = departments;


                string studentSql = @"SELECT s.StudentId, s.Name, s.RollNo, s.DateOfBirth, s.Gender, 
                             s.Address, s.PhoneNumber, s.DepartmentId 
                             FROM Students s 
                             WHERE s.StudentId = @StudentId";
                var student = con.QueryFirstOrDefault<StudentModel>(studentSql, new { StudentId });

                if (student == null)
                {
                    return HttpNotFound();
                }
                return View(student);
            }
        }

        [HttpPost]
        public ActionResult Update(StudentModel model)
        {
            if (!ModelState.IsValid)
            {
                using (var con = new SqlConnection(dapObj.conn))
                {
                    con.Open();

                    string updateSql = @"UPDATE Students
                                     SET Name = @Name, RollNo = @RollNo, DateOfBirth = @DateOfBirth, 
                                         Gender = @Gender, Address = @Address, PhoneNumber = @PhoneNumber, 
                                         DepartmentId = @DepartmentId
                                     WHERE StudentId = @StudentId";

                    string departmentSql = @"SELECT DepartmentId, DepartmentName FROM Departments";
                    var departments = con.Query<StudentModel>(departmentSql);
                    ViewBag.dept = departments;
                    con.Execute(updateSql, model);
                }
                return View(model);
            }
            try
            {
                dapObj.updateData(model);
                TempData["SuccessMessage"] = "Data updated successfully!";
                return RedirectToAction("Show"); 
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred while updating the data: {ex.Message}";
                return RedirectToAction("Show");
            }
        }






        //delete data
        public ActionResult Delete(int id)
        {
            bool isDeleted = dapObj.DeleteStudent(id); 

            if (isDeleted)
            {
                TempData["SuccessMessage"] = "Student deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete the student. Please try again.";
            }

            return RedirectToAction("Show"); 
        }





        //deleted data 
        public ActionResult DeletedView(int page = 1)
        {
            int pageSize = 5;
            int count = (page - 1) * pageSize + 1;
            ViewBag.c = count;
            var emp = dapObj.DeletedShow();
            var Employees = emp.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            int totalCount = emp.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(Employees);
        }



        //restored data
        public ActionResult Restore(int id)
        {
            bool isRestored = dapObj.RestoreData(id); 

            if (isRestored)
            {
                TempData["SuccessMessage"] = "Student restored successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to restore the student. The student may not exist in the backup.";
            }

            return RedirectToAction("Show"); 
        }







        //details of one data
        public ActionResult Details(int id)
        {
            return View(dapObj.ViewData(id));
        }







        // details of one deleted data
        public ActionResult DeletedDetails(int id)
        {
            return View(dapObj.ViewDeletedData(id));
        }






        //permanent delete 
        public ActionResult PermanentDelete(int id)
        {
            
            dapObj.DeletePermData(id);
            return RedirectToAction("DeletedView");
        }
  

    }
}