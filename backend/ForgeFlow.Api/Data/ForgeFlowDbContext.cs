using ForgeFlow.Api.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForgeFlow.Api.Data;

public class ForgeFlowDbContext(DbContextOptions<ForgeFlowDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
}
