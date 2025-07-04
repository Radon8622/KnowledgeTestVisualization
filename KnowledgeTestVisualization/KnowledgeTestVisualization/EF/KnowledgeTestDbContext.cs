using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeTestVisualization.EF;

public partial class KnowledgeTestDbContext : DbContext
{
    public KnowledgeTestDbContext()
    {
    }
    public static KnowledgeTestDbContext GetDBContext(out bool hasDbConnection)
    {
        var db = new KnowledgeTestDbContext();

        try
        {
            var select = db.Groups.ToList();
            hasDbConnection = true;
        }
        catch (Exception)
        {
            hasDbConnection = false;
        }

        return db;
    }

    public KnowledgeTestDbContext(DbContextOptions<KnowledgeTestDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<JournalQuestion> JournalQuestions { get; set; }

    public virtual DbSet<Lecturer> Lecturers { get; set; }

    public virtual DbSet<LecturerSubject> LecturerSubjects { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionBuffer> QuestionBuffers { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    public virtual DbSet<VwJournalAttempt> VwJournalAttempts { get; set; }

    public virtual DbSet<VwJournalDetailed> VwJournalDetaileds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=RADON-PC\\SQLEXPRESS;Initial Catalog=KnowledgeTestDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Group__3214EC07C4A7D750");

            entity.ToTable("Group");

            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Journal__3214EC07F929D45C");

            entity.ToTable("Journal");

            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Journals)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Lecturer");

            entity.HasOne(d => d.Student).WithMany(p => p.Journals)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Student");

            entity.HasOne(d => d.Subject).WithMany(p => p.Journals)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Subject");

            entity.HasOne(d => d.Test).WithMany(p => p.Journals)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Test");

            entity.HasOne(d => d.LecturerSubject).WithMany(p => p.Journals)
                .HasPrincipalKey(p => new { p.LecturerId, p.SubjectId })
                .HasForeignKey(d => new { d.LecturerId, d.SubjectId })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_LecturerSubject");
        });

        modelBuilder.Entity<JournalQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__JournalQ__3214EC0719C045C3");

            entity.ToTable("JournalQuestion", tb => tb.HasTrigger("TR_Check_PointsScored"));

            entity.HasOne(d => d.Journal).WithMany(p => p.JournalQuestions)
                .HasForeignKey(d => d.JournalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JournalQuestion_Journal");

            entity.HasOne(d => d.Question).WithMany(p => p.JournalQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JournalQuestion_Question");
        });

        modelBuilder.Entity<Lecturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lecturer__3214EC075159669E");

            entity.ToTable("Lecturer");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LecturerSubject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lecturer__3214EC07BFDD3F73");

            entity.ToTable("LecturerSubject");

            entity.HasIndex(e => new { e.LecturerId, e.SubjectId }, "UQ_Lecturer_Subject").IsUnique();

            entity.HasOne(d => d.Lecturer).WithMany(p => p.LecturerSubjects)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LecturerSubject_Lecturer");

            entity.HasOne(d => d.Subject).WithMany(p => p.LecturerSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LecturerSubject_Subject");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC0751855977");

            entity.ToTable("Question");

            entity.HasIndex(e => new { e.TestId, e.QuestionNumber }, "UX_Question_TestNumber").IsUnique();

            entity.HasOne(d => d.Test).WithMany(p => p.Questions)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Question_Test");
        });

        modelBuilder.Entity<QuestionBuffer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3214EC072EB84768");

            entity.ToTable("QuestionBuffer");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3214EC073EDA04BB");

            entity.ToTable("Student");

            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Student_Group");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subject__3214EC07C42B51D0");

            entity.ToTable("Subject");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Test__3214EC072F6F92D0");

            entity.ToTable("Test");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Lecturer).WithMany(p => p.Tests)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Test_Lecturer");

            entity.HasOne(d => d.Subject).WithMany(p => p.Tests)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Test_Subject");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAcco__3214EC077CA65F44");

            entity.ToTable("UserAccount");

            entity.HasIndex(e => e.Username, "UQ__UserAcco__536C85E4F3AF98FC").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(64);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Lecturer).WithMany(p => p.UserAccounts)
                .HasForeignKey(d => d.LecturerId)
                .HasConstraintName("FK_UserAccount_Lecturer");
        });

        modelBuilder.Entity<VwJournalAttempt>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwJournalAttempts");

            entity.Property(e => e.CreateTime).HasColumnType("datetime");
            entity.Property(e => e.TestName).HasMaxLength(100);
        });

        modelBuilder.Entity<VwJournalDetailed>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vwJournalDetailed");

            entity.Property(e => e.CreateTime).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
