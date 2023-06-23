using GTUParser.Models;
using GTUParser.Remote;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StudentApi.Controllers;

public class HomeController:Controller
{
    private GTUDbContext _dbContext;
    
    public HomeController(GTUDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<Table> Index()
    {
        return _dbContext.Tables.Include(o => o.Lectures).ToList();
    }
    
    public Table GetTableByGroup([FromRoute]string id)
    {
        Table table = _dbContext.Tables.Include(o=>o.Lectures).First(o => o.GroupName == id);

        return table;
    }
}