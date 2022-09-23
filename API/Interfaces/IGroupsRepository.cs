namespace API.Interfaces;

public interface IGroupsRepository
{
    void CreateGroup(MemberGroup group, UserGroup usergroup);////
    Task<PagedList<GroupDto>> GetGroups(GroupParams groupParams, AppUser user);////
    Task<PagedList<MemberDto>> GetGroupMembers(GroupParams groupParams);////
    Boolean CheckGroupMember(PostGroupDto postGroupDto, AppUser User);
    Task<PagedList<MemberDto>> GetGrouprequests(GroupParams groupParams);////
    Task<UserGroup> GetGroupAdmin(PostGroupDto postGroupDto);
    Task<PagedList<PostDto>> GetGroupPosts(GroupParams groupParams, AppUser User);////
    Task AsktoJoin(PostGroupDto postGroupDto, AppUser user);////
    Task DeleteJoinAsync(PostGroupDto postGroupDto, AppUser user);////

    Task AcceptUser(MemberGroup group, AppUser user);////

    void AddGroupPost(MemberGroup group, Post post);/////
    void DeleteGroupPost(Post post);
    Task<Post> GetGroupPost(int id);//
    Task<Post> GetlastUserPost(string creater);//
    Task<MemberGroup> GetGroupBynameAsync(string groupname);//
    Task<bool> GroupExists(string groupname);
}
