using Microsoft.EntityFrameworkCore;
using MeTenTenAPI.Models;

namespace MeTenTenAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Diary> Diaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User 엔티티 설정
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // 이메일 유니크 제약
                entity.HasIndex(e => e.Email).IsUnique();
                
                // 자기 참조 관계 (파트너)
                entity.HasOne(e => e.Partner)
                      .WithOne()
                      .HasForeignKey<User>(e => e.PartnerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Topic 엔티티 설정
            modelBuilder.Entity<Topic>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                
                // User와의 관계
                entity.HasOne(e => e.CreatedByUser)
                      .WithMany(u => u.Topics)
                      .HasForeignKey(e => e.CreatedByUserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Diary 엔티티 설정
            modelBuilder.Entity<Diary>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.EmotionTag).HasMaxLength(50);
                
                // User와의 관계 - NO ACTION으로 설정하여 순환 참조 방지
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Diaries)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.NoAction);
                
                // Topic과의 관계 - CASCADE 유지 (Topic이 삭제되면 관련 Diary도 삭제)
                entity.HasOne(e => e.Topic)
                      .WithMany(t => t.Diaries)
                      .HasForeignKey(e => e.TopicId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
