using CrudApp.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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


        //insert students data
        public void InsertStudent(StudentModel obj)
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    string procedure = "InsertStudent";
                    var parameters = new
                    {
                        obj.Name,
                        obj.RollNo,
                        obj.DepartmentId,
                        obj.DateOfBirth,
                        obj.Gender,
                        obj.Address,
                        obj.PhoneNumber
                    };
                    con.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                // Log the error to a text file
                ErrorLogger.LogError(ex);
                throw; // Rethrow the exception to allow the controller to handle it
            }
        }



        //update one data
        public void updateData(StudentModel obj)
        {
            try
            {

                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    string procedure = "UpdateStudent";
                    var parameters = new
                    {
                        obj.StudentId,
                        obj.Name,
                        obj.RollNo,
                        obj.DateOfBirth,
                        obj.Gender,
                        obj.Address,
                        obj.PhoneNumber,
                        obj.DepartmentId
                    };
                    con.Execute(procedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }


        //Show Details of one data
        public StudentModel ViewData(int StudentId)
        {


            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    string procedure = "GetStudentDetails";
                    return con.QueryFirstOrDefault<StudentModel>(
                        procedure,
                        new { StudentId },
                        commandType: CommandType.StoredProcedure
                    );
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }



        //DeltePermanent
        public void DeletePermData(int StudentId)
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    string sql = "delete from Students where StudentId=@StudentId";
                    con.Execute(sql, new { StudentId });
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }



        // SOFT DELETE
        public void SoftDeleteStudent(int studentId)
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    con.Execute("SoftDeleteStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }


        // SHOW NOT DELETED STUDENTS
        public List<StudentModel> GetActiveStudents()
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    return con.Query<StudentModel>("GetActiveStudents", commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }





        // GET DELETED STUDENTS
        public List<StudentModel> GetDeletedStudents()
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    return con.Query<StudentModel>("GetDeletedStudents", commandType: CommandType.StoredProcedure).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }


        // RESTORED DELETED STUDENTS 
        public void RestoreStudent(int studentId)
        {
            try
            {
                using (var con = new SqlConnection(conn))
                {
                    con.Open();
                    con.Execute("RestoreStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex);
                throw;
            }
        }



        //ERROR LOG 
        public static class ErrorLogger
            {
                private static readonly string LogFilePath = @"C:\Users\Equi-PC\Desktop\Akash\Log\ErrorLog.txt";

                public static void LogError(Exception ex)
                {
                    try
                    {

                        using (StreamWriter writer = new StreamWriter(LogFilePath, true))
                        {
                            writer.WriteLine($"Time: {DateTime.Now}");
                            writer.WriteLine($"Message: {ex.Message}");
                            writer.WriteLine($"StackTrace: {ex.StackTrace}");
                            if (ex.InnerException != null)
                            {
                                writer.WriteLine($"InnerException: {ex.InnerException.Message}");
                            }
                            writer.WriteLine(new string('-', 50));
                        }
                    }
                    catch (Exception loggingEx)
                        {
                             Console.WriteLine($"Logging failed: {loggingEx.Message}"); // Handle the logging error
                        }
            }
            }

         public void RestoreStudent()
        { 
            try
            {
                throw new Exception("Test Exception");
            }
            catch (Exception ex)
            {
                DapperCrud.ErrorLogger.LogError(ex);
            }

        }

    }
}
