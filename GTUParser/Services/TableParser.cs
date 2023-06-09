﻿using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using GTUParser.Models;

namespace GTUParser.Services
{
    public class TableParser : ITableParser
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
            ushort hourId = 1;
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

        private IList<Lecture> GenerateLecturesFromLectureElements(IEnumerable<IElement> potentialLectures,
            ushort hourId)
        {
            int potLecCount = potentialLectures.Count();
            IList<Lecture> lecs = new List<Lecture>();

            for (int i = 0; i < potLecCount; i++)
            {
                IElement element = potentialLectures.ElementAt(i);
                if (!IsValidLecture(element))
                    continue;
                string[] splittedName = element.InnerHtml.Split("<br>");
                bool isTabled = !IsTabled(element);
                string lecName, teacherName, location;
                int duration;
                IHtmlCollection<IElement> elements;
                bool isPractice = !element.InnerHtml.Contains("ლექცია");
                if (!isTabled)
                {
                    lecName = GetLectureName(splittedName);
                    teacherName = GetTeacherName(splittedName);
                    duration = GetDuration(element);
                    location = GetLocation(splittedName);
                    lecs.Add(new Lecture(lecName, hourId, teacherName, (ushort)i, duration, location, isPractice));
                }
                else
                {
                    elements = element.QuerySelectorAll("td.detailed");
                    duration = 1;
                    int offset = element.QuerySelector("tr").Children.Length;
                    int diff = offset;
                    while (offset > 0)
                    {
                        lecName = GetLectureNameNonOnline(elements[^(offset + (2 * diff))]);
                        teacherName = GetTeacherNameNonOnline(elements[^(offset + diff)]);
                        location = GetLocationNonOnline(elements[^(offset)]);
                        offset--;
                        lecs.Add(new Lecture(lecName, hourId, teacherName, (ushort)i, duration, location, isPractice));
                    }
                }
            }

            return lecs;
        }

        private string GetLocationNonOnline(IElement element) => element.InnerHtml;

        private string GetLectureNameNonOnline(IElement element) => NormalizeLectureName(element.InnerHtml);

        private string NormalizeLectureName(string txt)
        {
            int startIndex = txt.IndexOf('(');
            if (startIndex > 0)
                txt = txt.Remove(startIndex, txt.IndexOf(')') - startIndex + 1);

            return txt.Trim();
        }

    private string GetTeacherNameNonOnline(IElement element) => element.InnerHtml.Trim();

    private bool IsTabled(IElement element) =>
            element.FirstElementChild.TagName != "TABLE";

        private string GetLocation(string[] splittedName) => splittedName.Last();

        private int GetDuration(IElement element)
        {
            int duration;
            int.TryParse(element.GetAttribute("rowspan"), out duration);

            return duration < 2 ? 1 : duration;
        }

        private string GetTeacherName(string[] splittedName) => splittedName[1].Trim();

        private string GetLectureName(string[] splittedName) => NormalizeLectureName(splittedName[0]);

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

