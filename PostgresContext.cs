using Microsoft.EntityFrameworkCore;
using RR;

public class PostrgresContext : DbContext
{
    public PostrgresContext() { }
    public PostrgresContext(DbContextOptions<PostrgresContext> options) : base(options) { }

    public DbSet<PriceItems> PriceItems { get; set; } = null!;
}