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
       



        //insert new data
        public void InsertStudent(StudentModel obj)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string sql = @"
                    insert into Students (Name, RollNo, DepartmentId, DateOfBirth, Gender, Address, PhoneNumber)
                  values (@Name, @RollNo, @DepartmentId, @DateOfBirth, @Gender, @Address, @PhoneNumber)";
                con.Execute(sql, obj);

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
        public void DeleteStudent(int StudentId)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string sqlquery = "select * from Students where StudentId=@StudentId";
                var temp = con.Query<StudentModel>(sqlquery, new { StudentId });
                var cmd = "insert into StudentsBackup values(@Name, @RollNo, @DepartmentId, @DateOfBirth, @Gender, @Address, @PhoneNumber)";
                con.Execute(cmd, temp);
                string sql = "delete from Students where StudentId=@StudentId";
                con.Execute(sql, new { StudentId });
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
        public void RestoreData(int StudentId)
        {
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                string sql1 = "SELECT * FROM StudentsBackup WHERE StudentId = @StudentId";
                var student = con.Query<StudentModel>(sql1, new { StudentId });
                string sql2 = @" insert into Students (Name, RollNo, DepartmentId, DateOfBirth, Gender, Address, PhoneNumber)
                  values (@Name, @RollNo, @DepartmentId, @DateOfBirth, @Gender, @Address, @PhoneNumber)";
                con.Execute(sql2, student);
                string sql3 = "DELETE FROM StudentsBackup WHERE StudentId = @StudentId";
                con.Execute(sql3, new { StudentId });
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
