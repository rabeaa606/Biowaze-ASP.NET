namespace API.Entities;
public class Feeling
{
    public int Id { get; set; }
    public AppUser Creater { get; set; }
    public string CreaterUsername { get; set; }
    public int CreaterId { get; set; }
    public string Value { get; set; }
    public DateTime FeelingDate { get; set; } = DateTime.UtcNow;

}
