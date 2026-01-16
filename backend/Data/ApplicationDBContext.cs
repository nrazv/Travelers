using backend.Test;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class ApplicationDBContext : DbContext
{
    public DbSet<ApiTestData> Datas { get; set; }
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
}