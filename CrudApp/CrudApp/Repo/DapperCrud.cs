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
        public string conn = ConfigurationManager.ConnectionStrings["connectStr"].ConnectionString;
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
                throw (ex);// Rethrow the exception to allow the controller to handle it
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
                throw (ex);
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
                throw (ex);
            }
        }

        // GetDepartments
        public List<StudentModel> GetDepartments()
        {
            using (var connection = new SqlConnection(conn))
            {
                string query = "GetDepartments";
                return connection.Query<StudentModel>(query).ToList();
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
                    string sql = "delete from StudentsBackup where StudentId=@StudentId";
                    con.Execute(sql, new { StudentId });
                }
            }
            catch (Exception ex)
            {
                throw (ex);
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
                throw (ex);
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
                throw (ex);
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
                throw (ex);
            }
        }


        // RESTORED DELETED STUDENTS 
        public void RestoreStudent(int studentId)
        {
            try { 
            using (var con = new SqlConnection(conn))
            {
                con.Open();
                con.Execute("RestoreStudent", new { StudentId = studentId }, commandType: CommandType.StoredProcedure);
            }
            }
            catch (Exception ex)
            { 
                throw(ex);
            }
        }




        public List<StudentModel> GetSortedStudents(string columnName, string sortOrder, int pageNumber, int pageSize)
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                SqlCommand cmd = new SqlCommand("GetSortedStudents", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@ColumnName", columnName);
                cmd.Parameters.AddWithValue("@SortOrder", sortOrder);
                cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<StudentModel> students = new List<StudentModel>();

                while (reader.Read())
                {
                    students.Add(new StudentModel
                    {
                        StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        RollNo = reader.GetInt32(reader.GetOrdinal("RollNo")),
                        Department = reader.GetString(reader.GetOrdinal("Department")),
                        Gender = reader.GetString(reader.GetOrdinal("Gender")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth"))
                    });
                }
                return students;
            }
        }

        public int GetTotalStudentsCount()
        {
            using (SqlConnection con = new SqlConnection(conn))
            {
                SqlCommand cmd = new SqlCommand("GetTotalStudentsCount", con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }


    }
}
