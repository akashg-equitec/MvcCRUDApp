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
    

        


        //Insert new data
        public ActionResult Create()
        {
            using (var con = new SqlConnection(dapObj.conn))
            {
                con.Open();
                string sql = "select * from Departments";
                var temp = con.Query<StudentModel>(sql);
                ViewBag.dept = temp;

            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(StudentModel std)
        {
            dapObj.InsertStudent(std);
            return RedirectToAction("Show");
        }








        //update one data
        //public ActionResult Update(int StudentId)
        //{
        //    using (var con = new SqlConnection(dapObj.conn))
        //    {
        //        con.Open();
        //        string departmentSql = "SELECT d.DepartmentName as s.DepartmentId from Students s inner join Departments d on s.DepartmentId = d.DepartmentId  where s.StudentId=@StudentId";
        //        var departments = con.Query<StudentModel>(departmentSql); // Query the correct DepartmentModel
        //        ViewBag.dept = departments;

        //        string studentSql = @"select s.Name, s.RollNo, s.DateOfBirth, s.Gender, s.Address, s.PhoneNumber, 
        //            d.DepartmentName AS Department from Students s inner join Departments d on s.DepartmentId = d.DepartmentId 
        //            where s.StudentId=@StudentId";
        //        var student = con.QueryFirstOrDefault<StudentModel>(studentSql, new { StudentId = StudentId });

        //        if (student == null)
        //        {
        //            return HttpNotFound(); 
        //        }
        //        return View(student); 
        //    }
        //}

        //[HttpPost]
        //public ActionResult Update(StudentModel std)
        //{ 
        //    dapObj.updateData(std);
        //    return RedirectToAction("Show");    
        //}



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
                var departments = con.Query<StudentModel>(departmentSql); // Correct model
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
            dapObj.updateData(model);
            return RedirectToAction("Show");
        }





        //delete data
        public ActionResult Delete(int id)
        {
            dapObj.DeleteStudent(id);
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
            dapObj.RestoreData(id);
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