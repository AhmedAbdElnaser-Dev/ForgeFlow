using Microsoft.EntityFrameworkCore;

namespace ForgeFlow.Api.Data;

public class ForgeFlowDbContext(DbContextOptions<ForgeFlowDbContext> options) : DbContext(options)
{
}
