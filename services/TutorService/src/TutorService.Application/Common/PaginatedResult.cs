using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
}