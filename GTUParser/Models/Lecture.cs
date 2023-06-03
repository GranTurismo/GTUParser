using System.ComponentModel.DataAnnotations;

namespace GTUParser.Models
{
    public class Lecture
    {
        [Key]
        public int Id { get; set; }
        public string LectureName { get; set; }
        public DayOfWeek LectureDay { get; set; }
        public ushort TimeId { get; set; }
        public string TeacherName { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; }
        public bool IsPractice { get; set; }
        public bool IsOnline { get; set; }
        public string ZoomId { get; set; }

        public Lecture()
        {
            
        }
        
        public Lecture(string lecName, ushort timeId, string teachName,
            ushort lectureDay, int duration, string location, bool isPractice)
        {
            (LectureName, TimeId, TeacherName, Duration, Location, IsPractice) =
            (lecName, timeId, teachName, duration, location, isPractice);
            LectureDay =(DayOfWeek) ((lectureDay+1) % 7);
            Parse();
        }

        private void Parse()
        {
            if (TeacherName.Contains("Zoom ID:"))
            {
                string[] splitted = TeacherName.Split("Zoom ID:");
                ZoomIdParse(splitted);
                IsOnline = true;
                ParseTeacherName(splitted);
            }
        }

        private void ParseTeacherName(string[] splitted)
        {
            TeacherName = new string(splitted.First().Where(o=>!char.IsPunctuation(o))
                .ToArray())
                .Trim();
        }

        private void ZoomIdParse(string[] splitted)
        {
            ZoomId = new string(splitted[1].Where(o => char.IsDigit(o)).ToArray());
        }
    }
}