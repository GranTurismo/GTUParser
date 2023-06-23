using GTUParser.Models;

namespace GTUParser.Services;

public interface ITableParser
{
    public Table ParseTableFromSource();
}