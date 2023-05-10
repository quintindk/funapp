using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DataAccess;
using API;
using Moq;
using System.Collections.Specialized;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class WriteBlogPost_Tests
{
  [Fact]
  public void Test_WriteBlogPost_BlogNotSupplied()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var mockContext = new Mock<BloggingContext>();
    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["name"]).Returns(string.Empty);
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var blogsApi = new BlogsAPI(config.Object, mockContext.Object);
    //Act
    var result = blogsApi.WriteBlogPost(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public void Test_WriteBlogPost_BlogSuppliedBlank()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var mockContext = new Mock<BloggingContext>();
    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns(string.Empty);
    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var blogsApi = new BlogsAPI(config.Object, mockContext.Object);
    //Act
    var result = blogsApi.WriteBlogPost(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public void Test_WriteBlogPost_BlankRequestBody()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var mockContext = new Mock<BloggingContext>();
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("something");
    mockRequest.Setup(r => r.Body).Returns(stream);

    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var blogsApi = new BlogsAPI(config.Object, mockContext.Object);
    //Act
    var result = blogsApi.WriteBlogPost(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<BadRequestObjectResult>(result.Result);
  }

  [Fact]
  public void Test_WriteBlogPost_MalformedRequestBody()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var mockContext = new Mock<BloggingContext>();
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("{ \"Some\": \"Thing\", \"Here\": \"Breaks\"}");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("something");
    mockRequest.Setup(r => r.Body).Returns(stream);

    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var blogsApi = new BlogsAPI(config.Object, mockContext.Object);
    //Act
    var result = blogsApi.WriteBlogPost(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<BadRequestObjectResult>(result.Result);
  }

  [Fact]
  public void Test_WriteBlogPost_RequestBodyNoId()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var data = new List<Blog> {
        new Blog { Name = "BBB", Posts = new List<Post>() },
        new Blog { Name = "ZZZ", Posts = new List<Post>() },
        new Blog { Name = "AAA", Posts = new List<Post>() },
    }.AsQueryable();

    var postData = new List<Post> {
        new Post {
          PostId = 1,
          Title = "something",
          Content = "something else"
        }
    }.AsQueryable();

    var mockBlogSet = new Mock<DbSet<Blog>>();
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockPostSet = new Mock<DbSet<Post>>();
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(postData.Provider);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(postData.Expression);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(postData.ElementType);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(() => postData.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockBlogSet.Object);
    mockContext.Setup(c => c.Posts).Returns(mockPostSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("{ \"Title\": \"Testing Rocks\", \"Content\": \"Testing really rocks\"}");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("AAA");
    mockRequest.Setup(r => r.Body).Returns(stream);

    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var blogsApi = new BlogsAPI(config.Object, mockContext.Object);
    //Act
    var result = blogsApi.WriteBlogPost(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<OkObjectResult>(result.Result);
  }

  [Fact]
  public void Test_WriteBlogPost_RequestBodyWithIdButNotFound()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var data = new List<Blog> {
        new Blog { Name = "BBB", Posts = new List<Post>() },
        new Blog { Name = "ZZZ", Posts = new List<Post>() },
        new Blog { Name = "AAA", Posts = new List<Post>() },
    }.AsQueryable();

    var postData = new List<Post> {
        new Post {
          PostId = 1,
          Title = "something",
          Content = "something else"
        }
    }.AsQueryable();

    var mockBlogSet = new Mock<DbSet<Blog>>();
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockPostSet = new Mock<DbSet<Post>>();
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(postData.Provider);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(postData.Expression);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(postData.ElementType);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(() => postData.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockBlogSet.Object);
    mockContext.Setup(c => c.Posts).Returns(mockPostSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("{ \"PostId\": \"-1\", \"Title\": \"Testing Rocks\", \"Content\": \"Testing really rocks\"}");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("BBB");
    mockRequest.Setup(r => r.Body).Returns(stream);

    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var blogsApi = new BlogsAPI(config.Object, mockContext.Object);
    //Act
    var result = blogsApi.WriteBlogPost(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public void Test_WriteBlogPost_RequestBodyWithId()
  {
    //Arrange
    var logger = new Mock<ILogger>();
    var config = new Mock<IConfiguration>();
    var data = new List<Blog> {
        new Blog { Name = "BBB", Posts = new List<Post>() },
        new Blog { Name = "ZZZ", Posts = new List<Post>() },
        new Blog { Name = "AAA", Posts = new List<Post>() },
    }.AsQueryable();

    var postData = new List<Post> {
        new Post {
          PostId = 1,
          Title = "something",
          Content = "something else"
        }
    }.AsQueryable();

    var mockBlogSet = new Mock<DbSet<Blog>>();
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.Provider).Returns(data.Provider);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.Expression).Returns(data.Expression);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.ElementType).Returns(data.ElementType);
    mockBlogSet.As<IQueryable<Blog>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

    var mockPostSet = new Mock<DbSet<Post>>();
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.Provider).Returns(postData.Provider);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.Expression).Returns(postData.Expression);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.ElementType).Returns(postData.ElementType);
    mockPostSet.As<IQueryable<Post>>().Setup(m => m.GetEnumerator()).Returns(() => postData.GetEnumerator());

    var mockContext = new Mock<BloggingContext>();
    mockContext.Setup(c => c.Blogs).Returns(mockBlogSet.Object);
    mockContext.Setup(c => c.Posts).Returns(mockPostSet.Object);
    
    var stream = new MemoryStream();
    var writer = new StreamWriter(stream);
    writer.Write("{ \"PostId\": \"1\", \"Title\": \"Testing Rocks\", \"Content\": \"Testing really rocks\"}");
    writer.Flush();
    stream.Position = 0;

    var mockRequest = new Mock<HttpRequest>();
    mockRequest.Setup(r => r.Query["blog"]).Returns("BBB");
    mockRequest.Setup(r => r.Body).Returns(stream);

    var mockHttpContext = new Mock<HttpContext>();
    mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);

    var blogsApi = new BlogsAPI(config.Object, mockContext.Object);
    //Act
    var result = blogsApi.WriteBlogPost(mockHttpContext.Object.Request, logger.Object);
    result.Wait();

    //Assert
    Assert.IsType<OkObjectResult>(result.Result);
  }
}