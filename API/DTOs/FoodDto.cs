namespace API.DTOs;

public class FoodDto
{
    public int Id { get; set; }
    public int CreaterId { get; set; }
    public string CreaterUsername { get; set; }
    public string Value { get; set; }
    public string Kind { get; set; }
    public DateTime FoodDate { get; set; }
}
