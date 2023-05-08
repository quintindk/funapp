using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DataAccess;
using ReadIntent;
using Moq;
using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class ReadBlogPosts_Tests
{
  [Fact]
  public void Test_ReadBlogPosts_BlogNotSupplied()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var mockContext = new Mock<BloggingContext>();
    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["name"]).Returns("james");
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var readIntent = new CheckReadCommand(config.Object, mockContext.Object);
    //Act
    var result = readIntent.ReadBlogPosts(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public void Test_ReadBlogPosts_BlogNotFound()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();

    var data = new List<Blog> {
        new Blog { Name = "BBB" },
        new Blog { Name = "ZZZ" },
        new Blog { Name = "AAA" },
    }.AsQueryable();

    var mockSet = new Mock<DbSet<Blog>>();
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("CCC");
    mockRequest.Setup(r => r.Body).Returns(stream);
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var readIntent = new CheckReadCommand(config.Object, mockContext.Object);
    //Act
    var result = readIntent.ReadBlogPosts(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public void Test_ReadBlogPosts_BlogFound()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();

    var data = new List<Blog> {
        new Blog { Name = "BBB" },
        new Blog { Name = "ZZZ" },
        new Blog { Name = "AAA" },
    }.AsQueryable();

    var mockSet = new Mock<DbSet<Blog>>();
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("AAA");
    mockRequest.Setup(r => r.Body).Returns(stream);
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var readIntent = new CheckReadCommand(config.Object, mockContext.Object);
    //Act
    var result = readIntent.ReadBlogPosts(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<OkObjectResult>(result.Result);
  }

  [Fact]
  public void Test_ReadBlogPosts_RequestBodyFails()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();

    var data = new List<Blog> {
        new Blog { Name = "BBB" },
        new Blog { Name = "ZZZ" },
        new Blog { Name = "AAA" },
    }.AsQueryable();

    var mockSet = new Mock<DbSet<Blog>>();
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("{ \"something\": \"\" }");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("AAA");
    mockRequest.Setup(r => r.Body).Returns(stream);
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var readIntent = new CheckReadCommand(config.Object, mockContext.Object);
    //Act
    var result = readIntent.ReadBlogPosts(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<BadRequestObjectResult>(result.Result);
  }

  [Fact]
  public void Test_ReadBlogPosts_RequestBodyCorrect()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();

    var data = new List<Blog> {
        new Blog { Name = "BBB", Posts = new List<Post>() },
        new Blog { Name = "ZZZ", Posts = new List<Post>() },
        new Blog { Name = "AAA", Posts = new List<Post>() },
    }.AsQueryable();

    var mockSet = new Mock<DbSet<Blog>>();
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("{ \"Title\": \"something\" }");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("AAA");
    mockRequest.Setup(r => r.Body).Returns(stream);
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var readIntent = new CheckReadCommand(config.Object, mockContext.Object);
    //Act
    var result = readIntent.ReadBlogPosts(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<OkObjectResult>(result.Result);
  }

  [Fact]
  public void Test_ReadBlogPosts_RequestBodyCorrectWithResults()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();

    var data = new List<Blog> {
        new Blog { Name = "BBB", Posts = new List<Post> {
          new Post {
            Title = "something",
            Content = "something else"
          }
        } },
        new Blog { Name = "ZZZ", Posts = new List<Post>() },
        new Blog { Name = "AAA", Posts = new List<Post>() },
    }.AsQueryable();

    var mockSet = new Mock<DbSet<Blog>>();
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("{ \"Title\": \"something\" }");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("BBB");
    mockRequest.Setup(r => r.Body).Returns(stream);
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var readIntent = new CheckReadCommand(config.Object, mockContext.Object);
    //Act
    var result = readIntent.ReadBlogPosts(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<OkObjectResult>(result.Result);
    Assert.IsAssignableFrom<IEnumerable<Post>>(((OkObjectResult)result.Result).Value);
  }
}