using System;
namespace GTUParser.Models
{
	public class Table
	{
		public string GroupName { get; set; }
		public IList<Lecture> Lectures { get; set; }
	}
}

