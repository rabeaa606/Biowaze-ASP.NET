namespace API.DTOs;

public class FeelingDto
{
    public int Id { get; set; }
    public int CreaterId { get; set; }
    public string CreaterUsername { get; set; }
    public string Value { get; set; }
    public DateTime FeelingDate { get; set; }
}
