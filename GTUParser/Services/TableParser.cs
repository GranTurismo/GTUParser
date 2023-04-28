using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using GTUParser.Models;

namespace GTUParser.Services
{
    public class TableParser
    {
        private string _htmlSource;
        private HtmlParser _parser;
        private IHtmlDocument _document;

        public TableParser(string htmlSource)
        {
            _htmlSource = htmlSource;
            _parser = new HtmlParser();
            _document = _parser.ParseDocument(_htmlSource);
        }

        public Table ParseTableFromSource()
        {
            Table table = new Table();

            table.GroupName = GetGroupName();
            table.Lectures = GetLectures();


            return table;
        }

        private IList<Lecture> GetLectures()
        {
            IHtmlCollection<IElement> hours = GetHours();

            IList<Lecture> lectures = new List<Lecture>();
            ushort hourId = 0;
            foreach (IElement hour in hours)
            {
                IList<Lecture> lecs = GetLecturesOnHour(hour, hourId++);
                AddRangeInLectures(lectures, lecs);
            }

            return lectures;
        }

        private void AddRangeInLectures(IList<Lecture> inHere, IList<Lecture> fromHere)
        {
            foreach (Lecture lec in fromHere)
                inHere.Add(lec);

        }

        private IList<Lecture> GetLecturesOnHour(IElement hour, ushort hourId)
        {
            IEnumerable<IElement> potentialLectures = hour.Children.Where(o => o.TagName == "TD");
            IList<Lecture> lectures = GenerateLecturesFromLectureElements(potentialLectures, hourId);

            return lectures;
        }

        private IList<Lecture> GenerateLecturesFromLectureElements(IEnumerable<IElement> potentialLectures, ushort hourId)
        {
            int potLecCount = potentialLectures.Count();
            IList<Lecture> lecs = new List<Lecture>();

            for (int i = 0; i < potLecCount; i++)
            {
                IElement element = potentialLectures.ElementAt(i);
                if (!IsValidLecture(element))
                    continue;
                string[] splittedName = element.InnerHtml.Split("<br>");
                bool isPractice = !IsPracticeLecture(element);
                string lecName, teacherName,location;
                int duration;
                IHtmlCollection<IElement> elements;
                if (!isPractice)
                {
                    lecName = GetLectureName(splittedName);
                    teacherName = GetTeacherName(splittedName);
                    duration = GetDuration(element);
                    location = GetLocation(splittedName);
                }
                else
                {
                    elements = element.QuerySelectorAll("td.detailed");
                    teacherName = GetTeacherNameNonOnline(elements[2]);
                    lecName = GetLectureNameNonOnline(elements[1]);
                    duration = 1;
                    location = GetLocationNonOnline(elements.Last());
                }
                Lecture lec = new Lecture(lecName, hourId, teacherName, (ushort)i, duration, location, isPractice);
                lecs.Add(lec);
            }

            return lecs;
        }

        private string GetLocationNonOnline(IElement element) => element.InnerHtml;

        private string GetLectureNameNonOnline(IElement element) => element.InnerHtml;

        private string GetTeacherNameNonOnline(IElement element) => element.InnerHtml;

        private bool IsPracticeLecture(IElement element) =>
            element.FirstElementChild.TagName != "TABLE";

        private string GetLocation(string[] splittedName) => splittedName.Last();

        private int GetDuration(IElement element)
        {
            int duration;
            int.TryParse(element.GetAttribute("rowspan"), out duration);

            return duration < 2 ? 1 : duration;
        }

        private string GetTeacherName(string[] splittedName) => splittedName[1];

        private string GetLectureName(string[] splittedName) => splittedName[0];

        private bool IsValidLecture(IElement o)
        {
            string lecName = o.InnerHtml;
            for (int i = 0; i < lecName.Length - 1; i++)
                if (lecName[i] != lecName[i + 1])
                    return true;

            return false;
        }

        private IHtmlCollection<IElement> GetHours()
        {
            IHtmlCollection<IElement> hours = _document.GetElementsByTagName("tbody")[0].Children;

            return hours;
        }

        private string GetGroupName()
        {
            IHtmlCollection<IElement> elements = _document.GetElementsByTagName("th");

            return elements.First().InnerHtml;
        }
    }
}

