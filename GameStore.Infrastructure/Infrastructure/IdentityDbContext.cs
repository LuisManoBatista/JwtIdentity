using GameStore.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<ApplicationIdentityUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Move all Identity tables to the "auth" schema and rename them
        builder.Entity<ApplicationIdentityUser>(entity => entity.ToTable("IdentityUsers", "auth"));
        builder.Entity<IdentityRole>(entity => entity.ToTable("IdentityRoles", "auth"));
        builder.Entity<IdentityUserRole<string>>(entity => entity.ToTable("IdentityUserRoles", "auth"));
        builder.Entity<IdentityUserClaim<string>>(entity => entity.ToTable("IdentityUserClaims", "auth"));
        builder.Entity<IdentityUserLogin<string>>(entity => entity.ToTable("IdentityUserLogins", "auth"));
        builder.Entity<IdentityRoleClaim<string>>(entity => entity.ToTable("IdentityRoleClaims", "auth"));
        builder.Entity<IdentityUserToken<string>>(entity => entity.ToTable("IdentityUserTokens", "auth"));
    }
}