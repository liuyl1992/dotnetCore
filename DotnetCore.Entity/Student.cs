using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetCore.Entity
{
	public class Student
	{
		public int ID { get; set; }
		[Required]
		[StringLength(50)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }
		[Required]
		[StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
		[Column("FirstName")]
		[Display(Name = "First Name")]
		public string FirstMidName { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Enrollment Date")]
		public DateTime EnrollmentDate { get; set; }
		[Display(Name = "Full Name")]
		public string FullName
		{
			get
			{
				return LastName + ", " + FirstMidName;
			}
		}

		public ICollection<Enrollment> Enrollments { get; set; }
	}

	public class Instructor
	{
		public int ID { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		[StringLength(50)]
		public string LastName { get; set; }

		[Required]
		[Column("FirstName")]
		[Display(Name = "First Name")]
		[StringLength(50)]
		public string FirstMidName { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Hire Date")]
		public DateTime HireDate { get; set; }

		[Display(Name = "Full Name")]
		public string FullName
		{
			get { return LastName + ", " + FirstMidName; }
		}

		public ICollection<CourseAssignment> CourseAssignments { get; set; }
		public OfficeAssignment OfficeAssignment { get; set; }
	}

	public class Course
	{
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Display(Name = "Number")]
		public int CourseID { get; set; }

		[StringLength(50, MinimumLength = 3)]
		public string Title { get; set; }

		[Range(0, 5)]
		public int Credits { get; set; }

		public int DepartmentID { get; set; }

		public Department Department { get; set; }
		public ICollection<Enrollment> Enrollments { get; set; }
		public ICollection<CourseAssignment> CourseAssignments { get; set; }
	}

	public class Department
	{
		public int DepartmentID { get; set; }

		[StringLength(50, MinimumLength = 3)]
		public string Name { get; set; }

		[DataType(DataType.Currency)]
		[Column(TypeName = "money")]
		public decimal Budget { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "Start Date")]
		public DateTime StartDate { get; set; }

		public int? InstructorID { get; set; }

		public Instructor Administrator { get; set; }
		public ICollection<Course> Courses { get; set; }
	}

	public class CourseAssignment
	{
		public int InstructorID { get; set; }
		public int CourseID { get; set; }
		public Instructor Instructor { get; set; }
		public Course Course { get; set; }
	}

	public class OfficeAssignment
	{
		[Key]
		public int InstructorID { get; set; }
		[StringLength(50)]
		[Display(Name = "Office Location")]
		public string Location { get; set; }

		public Instructor Instructor { get; set; }
	}

	public enum Grade
	{
		A, B, C, D, F
	}

	public class Enrollment
	{
		public int EnrollmentID { get; set; }
		public int CourseID { get; set; }
		public int StudentID { get; set; }
		[DisplayFormat(NullDisplayText = "No grade")]
		public Grade? Grade { get; set; }

		public Course Course { get; set; }
		public Student Student { get; set; }
	}
}
