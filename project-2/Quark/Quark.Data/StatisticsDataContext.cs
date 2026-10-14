using Microsoft.EntityFrameworkCore;
using Quark.Data.Models;
using Quark.Data.Models.News;

namespace Quark.Data;

public class StatisticsDataContext : DbContext
{
    public StatisticsDataContext(DbContextOptions<StatisticsDataContext> options) : base(options)
    {
    }

    public DbSet<NewsViewsHourly> NewsViewsHourlies { get; set; }
    public DbSet<NewsViewsMinutely> NewsViewsMinutelies { get; set; }
    public DbSet<NewsViewsDaily> NewsViewsDailies { get; set; }
    public DbSet<NewsViewsRaw> NewsViewsRaws { get; set; }
    public DbSet<NewsStatus> NewsStatus { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<NewsPhoto> NewsPhotos { get; set; }
    public DbSet<NewsType> NewsTypes { get; set; }
    public DbSet<RelatedNews> RelatedNews { get; set; }
    public DbSet<NewsTag> NewsTags { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Caricaturist> Caricaturists { get; set; }
    public DbSet<Astrologer> Astrologers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfiguration(new CategoryLanguageValueConfiguration());

        modelBuilder.Entity<NewsTag>()
            .HasKey(x => new { x.NewsID, x.TagID });
        
        modelBuilder.Entity<UserCategory>()
            .HasKey(x => new { x.UserID, x.CategoryID });
        
        modelBuilder.Entity<AuthorCategory>()
            .HasKey(x => new { x.AuthorID, x.CategoryID });
        
        base.OnModelCreating(modelBuilder);
    }
}