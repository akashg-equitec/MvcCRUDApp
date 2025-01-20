using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CrudApp.Models
{
    public class StudentModel
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; }



        [Required(ErrorMessage = "Roll Number is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Roll Number must be greater than 0")]
        public int RollNo { get; set; }



        [Required(ErrorMessage = "Department is required")] 
        public int DepartmentId { get; set; }  // Foreign key for Department



        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime DateOfBirth { get; set; }



        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }



        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }



        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone Number must be exactly 10 digits")]
        public string PhoneNumber { get; set; }


        public string DepartmentName { get; set; }

        public string Department { get; set; }
    }
}