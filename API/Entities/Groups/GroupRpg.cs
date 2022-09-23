namespace API.Entities.Groups;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GroupRpg
{
    Admin = 1,
    Member = 2,
    Request = 3,
}
