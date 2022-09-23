
namespace API.Entities;
public class UserGroup
{
    public int UserId { get; set; }
    public AppUser User { get; set; }
    public int GroupId { get; set; }
    public MemberGroup Group { get; set; }
    public GroupRpg UserRole { get; set; }

}
