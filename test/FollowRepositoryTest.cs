namespace test;
using System.Diagnostics.Contracts;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class FollowRepositoryTest
{
      ChirpDBContext context;
    SqliteConnection _connection;
    //might not be neccarcary when pagesize is refactored
    int pageSize;
    /// <summary>
    /// The setup for cheeprepo-test, here the connection to the database and the dbContext is created 
    /// </summary>
    public FollowRepositoryTest()
    {
        _connection =  new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var _contextOptions = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(_connection).Options;
        context = new ChirpDBContext(_contextOptions);
        context.Database.EnsureCreated();
        pageSize = 32;
        SeedDatabase(context);
    }

    /// <summary>
    /// method for seeding the database with data for testing
    /// </summary>
    /// <param name="context"> the context dbcontext for testing</param>
    void SeedDatabase(ChirpDBContext context) 
    {
        var a1 = new Author() { AuthorId = 1, Name = "existingAuthor", Email = "existingEmail@mail.com", Cheeps = new List<Cheep>() };
        var a2 = new Author() { AuthorId = 2, Name = "Luanna Muro", Email = "Luanna-Muro@ku.dk", Cheeps = new List<Cheep>() };
        var a3 = new Author() { AuthorId = 3, Name = "Filip Muro", Email = "Filip-Muro@ku.dk", Cheeps = new List<Cheep>() };
        
        
        var authors = new List<Author>() { a1, a2};

        var c1 = new Cheep() { CheepId = 1, Author = a1, Text = "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.", TimeStamp = DateTime.Parse("2023-08-01 13:14:37") };
        var c2 = new Cheep() { CheepId = 2, Author = a1, Text = "And then, as he listened to all that''s left o'' twenty-one people.", TimeStamp = DateTime.Parse("2023-08-01 13:15:21") };
        var c3 = new Cheep() { CheepId = 3, Author = a2, Text = "In various enchanted attitudes, like the Sperm Whale.", TimeStamp = DateTime.Parse("2023-08-01 13:14:58") };
        var c4 = new Cheep() { CheepId = 4, Author = a1, Text = "Unless we succeed in establishing ourselves in some monomaniac way whatever significance might lurk in them.", TimeStamp = DateTime.Parse("2023-08-01 13:14:34") };
        var c5 = new Cheep() { CheepId = 5, Author = a2, Text = "At last we came back!", TimeStamp = DateTime.Parse("2023-08-01 13:14:35") };
        var c6 = new Cheep() { CheepId = 6, Author = a3, Text = "I like the cheese.", TimeStamp = DateTime.Parse("2023-08-01 13:16:34") };
        var c7 = new Cheep() { CheepId = 7, Author = a3, Text = "At last we came back to cheese!", TimeStamp = DateTime.Parse("2023-08-01 13:18:35") };

        var cheeps = new List<Cheep>(){c1,c2,c3,c4,c5,c6,c7};
        
        context.Follows.Add(new Follow() {Follower =a1,FollowerId = a1.AuthorId,Following = a1, FollowingId = a1.AuthorId});
        context.Follows.Add(new Follow() {Follower =a2,FollowerId = a2.AuthorId,Following = a2, FollowingId = a2.AuthorId});
        context.Follows.Add(new Follow() {Follower =a3,FollowerId = a3.AuthorId,Following = a3, FollowingId = a3.AuthorId});

        context.Follows.Add(new Follow() {Follower =a3,FollowerId = a3.AuthorId,Following = a2, FollowingId = a2.AuthorId});
        
        context.Follows.Add(new Follow() {Follower =a2,FollowerId = a2.AuthorId,Following = a3, FollowingId = a3.AuthorId});

        
        a1.Cheeps = new List<Cheep>() { c1, c2,c4 };
        a2.Cheeps = new List<Cheep>() { c3,c5 };
        context.Authors.AddRange(authors);
        context.Cheeps.AddRange(cheeps);
        context.SaveChanges();
    }

    /// <summary>
    /// checks that a follow is added and that it is the correct follow
    /// when the AddFollow task is called on two users that exist and are not following each other
    /// </summary>
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    [Theory]
    [InlineData("Filip Muro", "existingAuthor")]
    
    public async void AddFollow(string follower, string following)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        int size = 0;
        //act
        size = context.Follows.Count();
        await followRepository.AddFollower(follower, following);
        
        //assert
        Assert.Equal(size+1, context.Follows.Count());
        
        Assert.NotNull(context.Follows
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .SingleOrDefault(f => f.Follower.Name == follower && f.Following.Name == following));
        
    }
    /// <summary>
    /// checks that no follow is added when the AddFollow task is called
    /// while one or both authors do not exist
    /// </summary>
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    [Theory]
    [InlineData("Filip Muro", "nonExistingAuthor1")]
    [InlineData("nonExistingAuthor2", "ExistingAuthor")]
    [InlineData("nonExistingAuthor2", "nonExistingAuthor1")]
    
    public async void AddFollowWhileOneOrBothParametersDoesNotExist(string follower, string following)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        int size = 0;
        //act
        size = context.Follows.Count();
        async Task result() => await followRepository.AddFollower(follower, following);
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(size, context.Follows.Count());
    }
    
    /// <summary>
    /// checks that no follow is added when the AddFollow task is called
    /// while one or both authors are null
    /// </summary>
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    [Theory]
    [InlineData("Filip Muro", null)]
    [InlineData(null, "ExistingAuthor")]
    [InlineData(null, null)]
    
    public async void AddFollowWhileOneOrBothParametersAreNull(string follower, string following)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        int size = 0;
        //act
        size = context.Follows.Count();
        async Task result() => await followRepository.AddFollower(follower, following);
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(size, context.Follows.Count());
    }
    
    /// <summary>
    /// checks that a follow is removed and that it is the correct follow removed
    /// when the RemoveFollow task is called on two users that exist and are following each other
    /// </summary>
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    [Theory]
    [InlineData("Filip Muro", "Luanna Muro")]
    
    public async void RemoveFollow(string follower, string following)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        int size = 0;
        //act
        size = context.Follows.Count();
        await followRepository.RemoveFollower(follower, following);
        //assert
        Assert.Equal(size-1, context.Follows.Count());
        
        Assert.Null(context.Follows
            .Include(f => f.Follower)
            .Include(f => f.Following)
            .SingleOrDefault(f => f.Follower.Name == follower && f.Following.Name == following));
    }
    /// <summary>
    /// checks that no follow is removed when the RemoveFollower task is called
    /// while one or both authors do not exist
    /// </summary>
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    [Theory]
    [InlineData("Filip Muro", "nonExistingAuthor1")]
    [InlineData("nonExistingAuthor2", "Luanna Muro")]
    [InlineData("nonExistingAuthor2", "nonExistingAuthor1")]
    
    public async void RemoveFollowWhileOneOrBothParametersDoesNotExist(string follower, string following)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        int size = 0;
        //act
        size = context.Follows.Count();
        async Task result() => await followRepository.RemoveFollower(follower, following);
        //assert
        await Assert.ThrowsAsync<ArgumentException>(result);
        Assert.Equal(size, context.Follows.Count());
    }
    
    /// <summary>
    /// checks that no follow is removed when the RemoveFollower task is called
    /// while one or both authors are null
    /// </summary>
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    [Theory]
    [InlineData("Filip Muro", null)]
    [InlineData(null, "Luanna Muro")]
    [InlineData(null, null)]
    
    public async void RemoveFollowWhileOneOrBothParametersAreNull(string follower, string following)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        int size = 0;
        //act
        size = context.Follows.Count();
        async Task result() => await followRepository.RemoveFollower(follower, following);
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
        Assert.Equal(size, context.Follows.Count());
    }

    /// <summary>
    /// checks that the IsFollowing task returns the correct truth values when used
    /// on both an exising follow and a non exising follow
    /// </summary>
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    /// <param name="exists"> whether the follow exists or not</param>
    [Theory]
    [InlineData("Filip Muro", "Luanna Muro", true)]
    [InlineData("Filip Muro", "ExistingAuthor", false)]
    [InlineData("Filip Muro", "nonExistingAuthor1", false)]
    [InlineData("nonExistingAuthor2", "ExistingAuthor", false)]
    [InlineData("nonExistingAuthor2", "nonExistingAuthor1", false)]
   

    public async void testIsFollowingOnNotNullValues(string follower, string following,bool exists)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        //act
        var result = await followRepository.IsFollowing(follower, following);
        //assert
          Assert.Equal(exists,result);
    }
    /// <summary>
    /// checks that the IsFollowing task throws an ArgumentNullException
    ///  when one or both parameters is null 
    /// <param name="follower">the name of the one following</param>
    /// <param name="following">the name of the one being followed</param>
    
    [Theory]
    [InlineData("Filip Muro", null)]
    [InlineData(null, "Luanna Muro")]
    [InlineData(null, null)]

    public async void testIsFollowingOnNullValues(string follower, string following)
    {
        //arrange
        FollowRepository followRepository = new FollowRepository(context);
        //act
        async Task result() => await followRepository.IsFollowing(follower, following);
        //assert
        await Assert.ThrowsAsync<ArgumentNullException>(result);
    }
}
