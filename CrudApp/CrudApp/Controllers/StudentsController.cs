using CrudApp.Models;
using CrudApp.Repo;
using Dapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CrudApp.Controllers
{
    public class StudentsController : Controller
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        DapperCrud dapObj = new DapperCrud();

        //public StudentsController()
        //{
        //    logger.Debug("NLog is initialized and working.");
        //}



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
                logger.Info("Successfully loaded the Create page.");
                return View();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading Create page.");
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
                    logger.Error(ex, "Error loading departments during Create.");
                    TempData["ErrorMessage"] = $"Error loading departments: {ex.Message}";
                }
                return View(std);
            }

            try
            {
                dapObj.InsertStudent(std);
                logger.Info("Successfully added student with ID {0}.", std.StudentId);
                TempData["SuccessMessage"] = "Student added successfully!";
                return RedirectToAction("Show");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error adding student.");
                TempData["ErrorMessage"] = $"Error adding student: {ex.Message}";
                return View(std);
            }
        }





        // Update (GET)
        public ActionResult Update(int? StudentId)
        {
            if (!StudentId.HasValue)
            {
                logger.Warn("StudentId not provided for Update.");
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
                        logger.Warn("Student with ID {0} not found.", StudentId);
                        TempData["ErrorMessage"] = "Student not found.";
                        return RedirectToAction("Show");
                    }
                    logger.Info("Loaded Update page for student with ID {0}.", StudentId);
                    return View(student);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading Update page.");
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
                logger.Warn("Model state is invalid for updating student.");
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
                    logger.Error(ex, "Error loading departments during Update.");
                    TempData["ErrorMessage"] = $"Error loading departments: {ex.Message}";
                }
                return View(model);
            }

            try
            {
                // Validate DateOfBirth
                if (model.DateOfBirth == null || model.DateOfBirth < new DateTime(1753, 1, 1))
                {
                    logger.Error("Invalid DateOfBirth for student with ID {0}.", model.StudentId);
                    throw new ArgumentException("Invalid Date of Birth.");
                }

                dapObj.updateData(model);
                logger.Info("Successfully updated student with ID {0}.", model.StudentId);
                TempData["SuccessMessage"] = "Data updated successfully!";
                return RedirectToAction("Show");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error updating student.");
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
                    logger.Warn("Student with ID {0} not found for Details.", id);
                    TempData["ErrorMessage"] = "Student not found.";
                    return RedirectToAction("Show");
                }
                logger.Info("Loaded details for student with ID {0}.", id);
                return View(student);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading details for student with ID {0}.", id);
                TempData["ErrorMessage"] = $"Error loading details: {ex.Message}";
                return RedirectToAction("Show");
            }
        }




        // Show (GET)
        //public ActionResult Show(int page = 1)
        //{
        //    try
        //    {
        //        int pageSize = 10;
        //        var students = dapObj.GetActiveStudents();
        //        ViewBag.CurrentPage = page;
        //        ViewBag.TotalPages = Math.Ceiling((double)students.Count / pageSize);
        //        ViewBag.c = ((page - 1) * pageSize) + 1;

        //        logger.Info("Successfully loaded student list for page {0}.", page);
        //        return View(students.Skip((page - 1) * pageSize).Take(pageSize));
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex, "Error loading student list for page {0}.", page);
        //        TempData["ErrorMessage"] = $"Error loading student list: {ex.Message}";
        //        return RedirectToAction("Show");
        //    }
        //}

        public ActionResult Show(int page = 1, string searchQuery = "", string selectedDepartment = "")
        {
            try
            {
                int pageSize = 10;

                // Fetch all students
                var students = dapObj.GetActiveStudents();

                // Apply search query filter
                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    students = students
                        .Where(s => s.Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                }

                // Apply department filter
                if (!string.IsNullOrWhiteSpace(selectedDepartment))
                {
                    students = students
                        .Where(s => s.DepartmentName.Equals(selectedDepartment, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // Calculate pagination details
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = Math.Ceiling((double)students.Count / pageSize);
                ViewBag.c = ((page - 1) * pageSize) + 1;

                // Pass filtered students for the current page
                var paginatedStudents = students
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Pass department list for dropdown
                var departments = dapObj.GetDepartments();
                ViewBag.Dept = departments;

                logger.Info("Successfully loaded student list for page {0} with filters.", page);
                return View(paginatedStudents);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading student list for page {0}.", page);
                TempData["ErrorMessage"] = $"Error loading student list: {ex.Message}";
                return RedirectToAction("Show");
            }
        }








        // Soft Delete (GET)
        public ActionResult SoftDelete(int id)
        {
            try
            {
                dapObj.SoftDeleteStudent(id);
                logger.Info("Successfully soft-deleted student with ID {0}.", id);
                TempData["SuccessMessage"] = "Student soft-deleted successfully.";
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error soft-deleting student with ID {0}.", id);
                TempData["ErrorMessage"] = $"Error soft-deleting student: {ex.Message}";
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

                logger.Info("Successfully loaded deleted student list for page {0}.", page);
                return View(students.Skip((page - 1) * pageSize).Take(pageSize));
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error loading deleted student list for page {0}.", page);
                TempData["ErrorMessage"] = $"Error loading deleted students: {ex.Message}";
                return RedirectToAction("Show");
            }
        }


        // Restore (GET)
        public ActionResult Restore(int id)
        {
            try
            {
                dapObj.RestoreStudent(id);
                logger.Info("Successfully restored student with ID {0}.", id);
                TempData["SuccessMessage"] = "Student restored successfully.";
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error restoring student with ID {0}.", id);
                TempData["ErrorMessage"] = $"Error restoring student: {ex.Message}";
            }
            return RedirectToAction("DeletedView");
        }


    }
}