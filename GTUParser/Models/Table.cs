using System;
using System.ComponentModel.DataAnnotations;

namespace GTUParser.Models
{
	public class Table
	{
		[Key]
		public int Id { get; set; }
		public string GroupName { get; set; }
		public IList<Lecture> Lectures { get; set; }
	}
}

