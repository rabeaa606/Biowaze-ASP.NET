namespace API.Helpers;

public class GroupParams : PaginationParams
{
    public string Username { get; set; }
    public string Groupname { get; set; }
    public string Core { get; set; }
}
