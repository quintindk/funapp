using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Model;
using DataAccess;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ReadIntent
{
  public class CheckReadCommand
  {
    private readonly IConfiguration _configuration;
    private readonly BloggingContext _bloggingContext;

    public CheckReadCommand(IConfiguration configuration, BloggingContext bloggingContext)
    {
      _configuration = configuration;
      _bloggingContext = bloggingContext;
    }

    [FunctionName("ReadBlogPosts")]
    [OpenApiOperation(operationId: "ReadBlogPosts", tags: new[] { "blog" })] 
    [OpenApiParameter(name: "blog", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The name of the Blog you want to retrieve.")] 
    [OpenApiRequestBody(contentType: "application/json", typeof(string))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")] 
    public async Task<IActionResult> ReadBlogPosts(
        [HttpTrigger(AuthorizationLevel.Admin, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("ReadBlogPosts function processed a request.");

      string blogName = req.Query["blog"];
      if (string.IsNullOrEmpty(blogName)) {
        return new NotFoundResult();  
      }

      string title = null;
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      if (!string.IsNullOrEmpty(requestBody)) {
        dynamic post = JsonConvert.DeserializeObject(requestBody);
        title = post.Title;
        if (string.IsNullOrEmpty(title)) {
          return new BadRequestObjectResult("Missing Title in request body");  
        }
      }

      var blog = _bloggingContext.Blogs.FirstOrDefault(x => x.Name == blogName);
      if (blog == null) {
        return new NotFoundResult();
      }

      if (string.IsNullOrEmpty(title)) {
        return new OkObjectResult(blog.Posts);
      }

      return new OkObjectResult(blog.Posts.Where(x => x.Title.Contains(title)));
    }

    [FunctionName("WriteBlogPost")]
    [OpenApiOperation(operationId: "WriteBlogPost", tags: new[] { "blog" })] 
    [OpenApiParameter(name: "blog", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The name of the Blog you want to update or create")] 
    [OpenApiRequestBody(contentType: "application/json", typeof(Post))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")] 
    public async Task<IActionResult> WriteBlogPost(
        [HttpTrigger(AuthorizationLevel.Admin, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
      log.LogInformation("WriteBlogPost function processed a request.");

      string blogName = req.Query["blog"];
      if (string.IsNullOrEmpty(blogName)) {
        return new NotFoundResult();  
      }

      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      if (string.IsNullOrEmpty(requestBody)) {
        return new BadRequestObjectResult("Missing request body");  
      }

      int? postId = null;
      dynamic post = JsonConvert.DeserializeObject(requestBody);
      string stringPostId = post.PostId;
      string postTitle = post.Title;
      string postContent = post.Content;
      if (string.IsNullOrEmpty(postTitle) || string.IsNullOrEmpty(postContent)) {
        return new BadRequestObjectResult("Missing Title or Content in request body");  
      }
      if (!string.IsNullOrEmpty(stringPostId)) {
        if (int.TryParse(stringPostId, out int parsedInt)) {
          postId = parsedInt;
        }
      }

      var dbBlog = _bloggingContext.Blogs.FirstOrDefault(x => x.Name == blogName);
      if (dbBlog == null) {
        _bloggingContext.Blogs.Add(new Blog
        {
          Name = blogName
        });
        _bloggingContext.SaveChanges();

        dbBlog = _bloggingContext.Blogs.FirstOrDefault(x => x.Name == blogName);
      }

      if (dbBlog == null) {
        throw new Exception("Failed to create the Blog");
      }

      Post dbPost;
      if (postId == null) {
        _bloggingContext.Posts.Add(new Post {
          Title = post.Title,
          Content = post.Content,
          BlogId = dbBlog.BlogId
        });
        _bloggingContext.SaveChanges();

        dbPost = _bloggingContext.Posts.FirstOrDefault(x => x.Title == blogName);
      } else {
        dbPost = _bloggingContext.Posts.FirstOrDefault(x => x.PostId == postId);
        if (dbPost == null) {
          return new NotFoundResult();
        }
        dbPost.Content = post.Content;
        _bloggingContext.SaveChanges();

        dbPost = _bloggingContext.Posts.FirstOrDefault(x => x.PostId == postId);
      }

      return new OkObjectResult(new { Blog = dbBlog, Post = dbPost});
    }
  }
}
