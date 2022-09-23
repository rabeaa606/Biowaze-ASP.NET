namespace API.Entities;
public class Food
{
    public int Id { get; set; }
    public AppUser Creater { get; set; }
    public string CreaterUsername { get; set; }
    public int CreaterId { get; set; }
    public string Value { get; set; }
    public string Kind { get; set; }
    public DateTime FoodDate { get; set; } = DateTime.UtcNow;

}
