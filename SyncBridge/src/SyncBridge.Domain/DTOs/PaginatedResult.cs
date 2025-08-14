namespace SyncBridge.Domain.DTOs;



public class Paging
{
    public Links _links { get; set; }
    public int limit { get; set; }
    public int totalCount { get; set; }
    public string currentToken { get; set; }
    public string nextToken { get; set; }
}

public class PaginatedResult<T>
{
    public Paging paging { get; set; }
    public List<T> data { get; set; }
}

public class Links
{
    public Self self { get; set; }
    public Next next { get; set; }
}

public class Next
{
    public string href { get; set; }
}

public class Self
{
    public string href { get; set; }
}