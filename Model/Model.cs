using System.ComponentModel.DataAnnotations;
namespace Model
{
  public class Blog
  {
    [Key]
    public int BlogId { get; set; }
    public string? Name { get; set; }

    public virtual List<Post>? Posts { get; set; }
  }

  public class Post
  {
    [Key]
    public int PostId { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }

    public int BlogId { get; set; }
    public virtual Blog? Blog { get; set; }
  }
}