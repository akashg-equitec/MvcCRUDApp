using CrudApp.Models;
using CrudApp.Repo;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static CrudApp.Repo.DapperCrud;

namespace CrudApp.Controllers
{
    public class StudentsController : Controller
    {

        DapperCrud dapObj = new DapperCrud();



        // Create (GET)
        public ActionResult Create()
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error loading Create page: {ex.Message}";
                return RedirectToAction("Show");
            }
        }

        // Create (POST)
        [HttpPost]
        public ActionResult Create(StudentModel std)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(dapObj.conn))
                    {
                        con.Open();
                        string sql = "SELECT * FROM Departments";
                        var temp = con.Query<StudentModel>(sql);
                        ViewBag.dept = temp;
                    }
                }
                catch (Exception ex)
                {
                    // Log the error to a text file
                    ErrorLogger.LogError(ex);
                    TempData["ErrorMessage"] = $"Error loading departments: {ex.Message}";
                }
                return View(std);
            }

            try
            {
                dapObj.InsertStudent(std);
                TempData["SuccessMessage"] = "Student added successfully!";
                return RedirectToAction("Show");
            }
            catch (Exception ex)
            {
                // Log the error to a text file
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error adding student: {ex.Message}";
                return View(std);
            }
        }





        // Update (GET)
        public ActionResult Update(int? StudentId)
        {
            if (!StudentId.HasValue)
            {
                TempData["ErrorMessage"] = "StudentId is required.";
                return RedirectToAction("Show");
            }

            try
            {
                using (var con = new SqlConnection(dapObj.conn))
                {
                    con.Open();
                    string departmentSql = "GetDepartments";
                    var departments = con.Query<StudentModel>(departmentSql, commandType: CommandType.StoredProcedure);
                    ViewBag.dept = departments;

                    string studentSql = "GetStudentById";
                    var student = con.QueryFirstOrDefault<StudentModel>(studentSql, new { StudentId }, commandType: CommandType.StoredProcedure);

                    if (student == null)
                    {
                        TempData["ErrorMessage"] = "Student not found.";
                        return RedirectToAction("Show");
                    }
                    return View(student);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error loading Update page: {ex.Message}";
                return RedirectToAction("Show");
            }
        }

        // Update (POST)
        [HttpPost]
        public ActionResult Update(StudentModel model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    using (var con = new SqlConnection(dapObj.conn))
                    {
                        con.Open();
                        string departmentSql = "GetDepartments";
                        var departments = con.Query<StudentModel>(departmentSql, commandType: CommandType.StoredProcedure);
                        ViewBag.dept = departments;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error loading departments: {ex.Message}";
                }
                return View(model);
            }

            try
            {
                // Ensure DateOfBirth is valid
                if (model.DateOfBirth == null || model.DateOfBirth < new DateTime(1753, 1, 1))
                {
                    throw new ArgumentException("Invalid Date of Birth.");
                }

                dapObj.updateData(model);
                TempData["SuccessMessage"] = "Data updated successfully!";
                return RedirectToAction("Show");
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error updating data: {ex.Message}";
                return View(model);
            }
        }





        // Details (GET)
        public ActionResult Details(int id)
        {
            try
            {
                var student = dapObj.ViewData(id);
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Student not found.";
                    return RedirectToAction("Show");
                }
                return View(student);
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error loading details: {ex.Message}";
                return RedirectToAction("Show");
            }
        }




        // Show (GET)
        public ActionResult Show(int page = 1)
        {
            try
            {
                int pageSize = 10;
                var students = dapObj.GetActiveStudents();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = Math.Ceiling((double)students.Count / pageSize);
                ViewBag.c = ((page - 1) * pageSize) + 1;

                return View(students.Skip((page - 1) * pageSize).Take(pageSize));
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error loading student list: {ex.Message}";
                return RedirectToAction("Error");
            }
        }

        // Permanent Delete (GET)
        public ActionResult Delete(int id)
        {
            try
            {
                dapObj.DeletePermData(id);
                TempData["SuccessMessage"] = "Student deleted successfully.";
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error deleting student: {ex.Message}";
                return RedirectToAction("Error");
            }
            return RedirectToAction("DeletedView");
        }


        // Soft Delete (GET)
        public ActionResult SoftDelete(int id)
        {
            try
            {
                dapObj.SoftDeleteStudent(id);
                TempData["SuccessMessage"] = "Student soft-deleted successfully.";
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error soft-deleting student: {ex.Message}";
                return RedirectToAction("Error");
            }
            return RedirectToAction("Show");
        }

        // Deleted View (GET)
        public ActionResult DeletedView(int page = 1)
        {
            try
            {
                int pageSize = 10;
                var students = dapObj.GetDeletedStudents();
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = Math.Ceiling((double)students.Count / pageSize);
                ViewBag.c = ((page - 1) * pageSize) + 1;

                return View(students.Skip((page - 1) * pageSize).Take(pageSize));
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error loading deleted students: {ex.Message}";
                return RedirectToAction("Error");
            }
        }

        // Restore (GET)
        public ActionResult Restore(int id)
        {
            try
            {
                dapObj.RestoreStudent(id);
                TempData["SuccessMessage"] = "Student restored successfully.";
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                TempData["ErrorMessage"] = $"Error restoring student: {ex.Message}";
                return RedirectToAction("Error");
            }
            return RedirectToAction("DeletedView");
        }

    }
}