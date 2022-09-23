namespace API.SignalR;

public class PostHub : Hub
{

    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _tracker;
    private readonly IUnitOfWork _unitOfWork;

    public PostHub(IUnitOfWork unitOfWork, IHubContext<PresenceHub> presenceHub,
      PresenceTracker tracker)
    {
        _unitOfWork = unitOfWork;
        _tracker = tracker;
        _presenceHub = presenceHub;
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task AddLike(AddLikeDto addLikeDto)
    {
        var postId = addLikeDto.postId;
        var Id = addLikeDto.userId;//userID

        var postLike = await _unitOfWork.PostLikesRepository.GetPostLike(postId, Id); //check if user liked this post
        if (postLike == null)
        {

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(Id);
            var post = await _unitOfWork.PostRepository.GetPost(postId);
            user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.UserName);  //for including photos

            if (user == null) throw new HubException("Not found user");
            if (post == null) throw new HubException("Not found post");


            var like = new PostLike
            {
                LikedUser = user,
                LikedUserId = Id,
                SourcePost = post,
                SourcePostId = postId
            };
            _unitOfWork.PostLikesRepository.AddLike(like);

            if (await _unitOfWork.Complete())
            {
                if (user.UserName == post.CreaterUsername)
                {


                    var connections1 = await _tracker.GetConnectionsForUser(user.UserName);
                    if (connections1 != null)
                    {
                        var photourl = user.Photo?.FirstOrDefault(p => p.IsMain).Url ?? "";
                        await Clients.Caller.SendAsync("NewLikeDone",
                          new
                          {
                              id = postId,
                              username = user.UserName,
                              knownAs = user.KnownAs,
                              photoUrl = photourl
                          });
                    }
                }
                else
                {
                    var connections = await _tracker.GetConnectionsForUser(post.CreaterUsername);
                    if (connections != null)
                    {
                        await _presenceHub.Clients.Clients(connections).SendAsync("NewPostLikeReceived",
                            new { username = user.UserName, knownAs = user.KnownAs });
                    }
                }

            }
        }
        else
        {
            await Clients.Caller.SendAsync("NewLikeReject", "Post liked Rejected");
        }
    }

    public async Task AddComment(PostCommentDto postCommentDto)
    {
        var postId = postCommentDto.PostId;

        var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(postCommentDto.Username);
        var post = await _unitOfWork.PostRepository.GetPost(postId);
        user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(user.UserName);  //for including photos

        if (user == null) throw new HubException("Not found user");


        var comment = new PostComment
        {
            SourcePost = post,
            SourcePostId = postId,
            CommentedUser = user,
            CommentedUserId = user.Id,
            Content = postCommentDto.Content,
            Role = postCommentDto.Role,
            CommentCreated = DateTime.UtcNow
        };
        _unitOfWork.PostCommentsRepository.AddComment(comment);

        if (await _unitOfWork.Complete())
        {
            if (user.UserName == post.CreaterUsername)
            {

                var connections1 = await _tracker.GetConnectionsForUser(user.UserName);
                if (connections1 != null)
                {
                    var photourl = user.Photo?.FirstOrDefault(p => p.IsMain).Url ?? "";
                    await Clients.Caller.SendAsync("NewCommentDone", new
                    {
                        // id = thisComment.Id,
                        postId = postId,
                        username = user.UserName,
                        knownAs = user.KnownAs,
                        photoUrl = photourl,
                        content = postCommentDto.Content,
                        role = postCommentDto.Role,
                        commentCreated = postCommentDto.CommentCreated
                    }
                     );
                }
            }
            else
            {
                var connections = await _tracker.GetConnectionsForUser(post.CreaterUsername);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewPostCommentReceived",
                       new { username = user.UserName, knownAs = user.KnownAs, role = postCommentDto.Role });
                }
            }
        }
        else
        {
            await Clients.Caller.SendAsync("NewCommentReject", "Comment Rejected");
        }
    }


}



