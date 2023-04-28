using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using GTUParser.Models;
using GTUParser.Services;
using System;
using System.Collections.ObjectModel;
using HtmlAgilityPack;
using System.Net;

DateTime start = DateTime.Now;

List<String> tables = new List<string>();
HttpClient client = new HttpClient();
Console.WriteLine("INPUT URL:");
string input=Console.ReadLine();
Console.WriteLine("GETTING SOURCE . . .");
string pageSource = await client.GetStringAsync(input);
Console.WriteLine("COLLECTING TABLES . . .");
HtmlDocument doc = new HtmlDocument();
doc.LoadHtml(pageSource);
HtmlNodeCollection nodes=doc.DocumentNode.SelectNodes("//table[@class='odd_table' or @class='even_table']");
tables.AddRange(nodes.Select(o=>o.OuterHtml));
Console.WriteLine($"{tables.Count} TABLES COLLECTED");
Console.WriteLine("PARSING DATA . . . ");
IList<Table> parsedTables = new List<Table>();

foreach (string item in tables)
{
    TableParser parser = new TableParser(item);
    Table table = parser.ParseTableFromSource();
    parsedTables.Add(table);
}
DateTime end = DateTime.Now;
Console.WriteLine($"{parsedTables.Count} TABLES PARSED");
Console.WriteLine((end-start).ToString());
Console.ReadKey();