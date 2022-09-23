
namespace API.Data;

public class DataContext : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<PostLike> PostsLikes { get; set; }
    public DbSet<PostComment> PostsComments { get; set; }
    public DbSet<PostPhoto> PostsPhoto { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Food> Foods { get; set; }
    public DbSet<Feeling> Feelings { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<MemberGroup> MemberGroup { get; set; }
    public DbSet<UserGroup> UserGroup { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Group>()
                     .HasMany(x => x.Connections)
                     .WithOne()
                     .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<PostLike>()
                        .HasKey(k => new { k.SourcePostId, k.LikedUserId });

        builder.Entity<PostLike>()
            .HasOne(s => s.SourcePost)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.SourcePostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PostComment>()
            .HasOne(s => s.SourcePost)
            .WithMany(l => l.CommentedByUsers)
            .HasForeignKey(s => s.SourcePostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PostPhoto>()
        .HasOne(s => s.SourcePost)
        .WithMany(l => l.Photos)
        .HasForeignKey(s => s.SourcePostId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);

        builder.Entity<Post>()
             .HasOne(u => u.Creater)
             .WithMany(p => p.UserPosts)
             .HasForeignKey(s => s.CreaterId)
             .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<UserGroup>().HasKey(sc => new { sc.UserId, sc.GroupId });

        builder.Entity<MemberGroup>()
          .HasMany(ur => ur.Users)
          .WithOne(u => u.Group)
          .HasForeignKey(ur => ur.GroupId)
          .IsRequired();

        builder.Entity<Food>()
                  .HasOne(u => u.Creater)
                  .WithMany(p => p.UserFoods)
                  .HasForeignKey(s => s.CreaterId)
                  .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Feeling>()
                  .HasOne(u => u.Creater)
                 .WithMany(p => p.UserFeelings)
                 .HasForeignKey(s => s.CreaterId)
                 .OnDelete(DeleteBehavior.Cascade);
    }
}


