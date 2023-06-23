using System.Text.Json.Serialization;
using GTUParser.Models;
using GTUParser.Remote;
using GTUParser.Services;
using HtmlAgilityPack;
using Newtonsoft.Json;

GTUDbContext con = new GTUDbContext();
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
    ITableParser parser = new TableParser(item);
    Table table = parser.ParseTableFromSource();
    parsedTables.Add(table);
}

/*StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/file.json");
    writer.WriteLine(JsonConvert.SerializeObject(parsedTables));
    writer.Flush();
    writer.Close();*/

Console.WriteLine("ADDING IN DB . . .");
con.Lectures.RemoveRange(con.Lectures);
con.Tables.RemoveRange(con.Tables);
con.Tables.AddRange(parsedTables);
con.SaveChanges();

DateTime end = DateTime.Now;
Console.WriteLine($"{parsedTables.Count} TABLES PARSED AND ADDED");
Console.WriteLine((end-start).ToString());
Console.ReadKey();