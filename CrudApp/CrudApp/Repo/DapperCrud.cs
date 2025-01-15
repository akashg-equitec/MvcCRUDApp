using CrudApp.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CrudApp.Repo
{
    public class DapperCrud
    {
       public    string conn = ConfigurationManager.ConnectionStrings["connectStr"].ConnectionString;
        StudentModel studObj = new StudentModel();
            



        //shows students data
        public List<StudentModel> ShowData()
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string sql = @"
                    select s.StudentID, s.Name, s.RollNo, s.DateOfBirth, s.Gender, s.Address, s.PhoneNumber, 
                    d.DepartmentName AS Department from Students s inner join Departments d on s.DepartmentId = d.DepartmentId";
                return con.Query<StudentModel>(sql).ToList();
            }
        }




     
        //insert data
        public void InsertStudent(StudentModel obj)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string checkSql = "SELECT COUNT(*) FROM Students WHERE RollNo = @RollNo";
                int existingCount = con.ExecuteScalar<int>(checkSql, new { obj.RollNo });

                if (existingCount > 0)
                {
                    throw new Exception("Roll number already exists in the database.");
                }

                string insertSql = @"
                    INSERT INTO Students (Name, RollNo, DepartmentId, DateOfBirth, Gender, Address, PhoneNumber)
                    VALUES (@Name, @RollNo, @DepartmentId, @DateOfBirth, @Gender, @Address, @PhoneNumber)";
                con.Execute(insertSql, obj);
            }
        }





        //update one data
        public void updateData(StudentModel obj)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string sql = @"
            UPDATE Students 
            SET Name = @Name, RollNo = @RollNo, DateOfBirth = @DateOfBirth, 
                Gender = @Gender, Address = @Address, PhoneNumber = @PhoneNumber, 
                DepartmentId = @DepartmentId
            WHERE StudentId = @StudentId";
                con.Execute(sql, obj);
            }
        }





        //delete data 
        public bool DeleteStudent(int StudentId)
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    string sqlQuery = "SELECT * FROM Students WHERE StudentId = @StudentId";
                    var temp = con.Query<StudentModel>(sqlQuery, new { StudentId });

                    if (temp.Any()) 
                    {
                        string backupQuery = "INSERT INTO StudentsBackup (Name, RollNo, DepartmentId, DateOfBirth, Gender, Address, PhoneNumber) VALUES (@Name, @RollNo, @DepartmentId, @DateOfBirth, @Gender, @Address, @PhoneNumber)";
                        con.Execute(backupQuery, temp);
                        string deleteQuery = "DELETE FROM Students WHERE StudentId = @StudentId";
                        con.Execute(deleteQuery, new { StudentId });
                        return true;
                    }
                }
                return false; 
            }
            catch
            {
                return false; 
            }
        }







        //deleted data
        public List<StudentModel> DeletedShow()
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string sql = @" select s.StudentID, s.Name, s.RollNo, s.DateOfBirth, s.Gender, s.Address, s.PhoneNumber, 
                    d.DepartmentName AS Department from StudentsBackup s inner join Departments d on s.DepartmentId = d.DepartmentId";
                return con.Query<StudentModel>(sql).ToList();
            }
        }






        //restore data 
        public bool RestoreData(int StudentId)
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    string sqlCheck = "SELECT * FROM StudentsBackup WHERE StudentId = @StudentId";
                    var student = con.Query<StudentModel>(sqlCheck, new { StudentId });

                    if (student.Any()) 
                    {
                        string sqlRestore = @"INSERT INTO Students (Name, RollNo, DepartmentId, DateOfBirth, Gender, Address, PhoneNumber)
                                      VALUES (@Name, @RollNo, @DepartmentId, @DateOfBirth, @Gender, @Address, @PhoneNumber)";
                        con.Execute(sqlRestore, student);

                        string sqlDelete = "DELETE FROM StudentsBackup WHERE StudentId = @StudentId";
                        con.Execute(sqlDelete, new { StudentId });

                        return true; 
                    }
                }

                return false; 
            }
            catch
            {
                return false; 
            }
        }






        //Show Details of one data
        public StudentModel ViewData(int StudentId)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
     
                string sql = @"
                    select s.Name, s.RollNo, s.DateOfBirth, s.Gender, s.Address, s.PhoneNumber, 
                    d.DepartmentName AS Department from Students s inner join Departments d on s.DepartmentId = d.DepartmentId where s.StudentId=@StudentId";

                return con.QueryFirstOrDefault<StudentModel>(sql, new { StudentId });
            }
        }






        //Show Details of one Deleted Data
        public StudentModel ViewDeletedData(int StudentId)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();

                string sql = @"
                    select s.Name, s.RollNo, s.DateOfBirth, s.Gender, s.Address, s.PhoneNumber, d.DepartmentName 
                    AS Department from StudentsBackup s inner join Departments d on s.DepartmentId = d.DepartmentId where s.StudentId=@StudentId";
                
                return con.QueryFirstOrDefault<StudentModel>(sql, new { StudentId });
            }
        }






        //DeltePermanent
        public void DeletePermData(int StudentId)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string sql = "delete from StudentsBackup where StudentId=@StudentId";
                con.Execute(sql, new { StudentId });
            }
        }









    }
}
