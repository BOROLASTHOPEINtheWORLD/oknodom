using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OKNODOM.Models;

public partial class OknodomDbContext : DbContext
{
    public OknodomDbContext()
    {
    }

    public OknodomDbContext(DbContextOptions<OknodomDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Бригады> Бригады { get; set; }

    public virtual DbSet<ВыполнениеРабот> ВыполнениеРабот { get; set; }

    public virtual DbSet<Заказы> Заказы { get; set; }

    public virtual DbSet<Замеры> Замеры { get; set; }

    public virtual DbSet<Комплектующие> Комплектующие { get; set; }

    public virtual DbSet<Материалы> Материалы { get; set; }

    public virtual DbSet<Окна> Окна { get; set; }

    public virtual DbSet<ОконныеПроемы> ОконныеПроемы { get; set; }

    public virtual DbSet<Пользователи> Пользователи { get; set; }

    public virtual DbSet<Профили> Профили { get; set; }

    public virtual DbSet<Роли> Роли { get; set; }

    public virtual DbSet<СтатусыЗаказа> СтатусыЗаказа { get; set; }

    public virtual DbSet<Створки> Створки { get; set; }

    public virtual DbSet<Стеклопакеты> Стеклопакеты { get; set; }

    public virtual DbSet<ТипыКомплектующих> ТипыКомплектующих { get; set; }

    public virtual DbSet<ТипыСтворок> ТипыСтворок { get; set; }

    public virtual DbSet<ТипыТоваров> ТипыТоваров { get; set; }

    public virtual DbSet<ТипыУслуг> ТипыУслуг { get; set; }

    public virtual DbSet<Товары> Товары { get; set; }

    public virtual DbSet<ТоварыВЗаказе> ТоварыВЗаказе { get; set; }

    public virtual DbSet<Услуги> Услуги { get; set; }

    public virtual DbSet<УслугиВЗаказе> УслугиВЗаказе { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=oknodom_db;Trusted_Connection=true;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Бригады>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Бригады__C8AC1204C1F891E5");

            entity.HasOne(d => d.КодВыполненияМонтажаNavigation).WithMany(p => p.Бригады)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Бригады__код_вып__628FA481");

            entity.HasOne(d => d.КодМонтажникаNavigation).WithMany(p => p.Бригады)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Бригады__код_мон__6383C8BA");
        });

        modelBuilder.Entity<ВыполнениеРабот>(entity =>
        {
            entity.HasKey(e => e.КодВыполнения).HasName("PK__Выполнен__9AD071A2DB0E40E1");

            entity.Property(e => e.КодВыполнения).ValueGeneratedOnAdd();
            entity.Property(e => e.ДатаВыполнения).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.КодВыполненияNavigation).WithOne(p => p.ВыполнениеРабот)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Выполнение_монтажа_Товары_в_заказе");

            entity.HasOne(d => d.КодОконногоПроемаNavigation).WithMany(p => p.ВыполнениеРабот).HasConstraintName("FK__Выполнени__код_о__5CD6CB2B");
        });

        modelBuilder.Entity<Заказы>(entity =>
        {
            entity.HasKey(e => e.КодЗаказа).HasName("PK__Заказы__5F93D6C5544B7A37");

            entity.Property(e => e.ДатаСозданияЗаказа).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.КодКлиентаNavigation).WithMany(p => p.Заказы)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Заказы__код_клие__3B75D760");

            entity.HasOne(d => d.КодСтатусаЗаказаNavigation).WithMany(p => p.Заказы)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Заказы__код_стат__3C69FB99");
        });

        modelBuilder.Entity<Замеры>(entity =>
        {
            entity.HasKey(e => e.КодЗамера).HasName("PK__Замеры__8556EC8BDDBA21EC");

            entity.HasOne(d => d.КодЗаказаNavigation).WithMany(p => p.Замеры)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Замеры__код_зака__5629CD9C");

            entity.HasOne(d => d.КодЗамерщикаNavigation).WithMany(p => p.Замеры)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Замеры__код_заме__5535A963");
        });

        modelBuilder.Entity<Комплектующие>(entity =>
        {
            entity.HasKey(e => e.КодТовара).HasName("PK__Комплект__1F177FD6A0E4B2B8");

            entity.Property(e => e.КодТовара).ValueGeneratedNever();

            entity.HasOne(d => d.КодМатериалаNavigation).WithMany(p => p.Комплектующие).HasConstraintName("FK__Комплекту__код_м__4BAC3F29");

            entity.HasOne(d => d.КодТипаКомплектующегоNavigation).WithMany(p => p.Комплектующие)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Комплекту__код_т__4AB81AF0");

            entity.HasOne(d => d.КодТовараNavigation).WithOne(p => p.Комплектующие)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Комплекту__код_к__49C3F6B7");
        });

        modelBuilder.Entity<Материалы>(entity =>
        {
            entity.HasKey(e => e.КодМатериала).HasName("PK__Материал__F1BA6D2DE462E30F");
        });

        modelBuilder.Entity<Окна>(entity =>
        {
            entity.HasKey(e => e.КодТовара).HasName("PK__Окна__29C955F08BEEE392");

            entity.Property(e => e.КодТовара).ValueGeneratedNever();

            entity.HasOne(d => d.КодПрофиляNavigation).WithMany(p => p.Окна)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Окна__код_профил__44FF419A");

            entity.HasOne(d => d.КодСтеклопакетаNavigation).WithMany(p => p.Окна)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Окна__код_стекло__45F365D3");

            entity.HasOne(d => d.КодТовараNavigation).WithOne(p => p.Окна)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Окна__код_окна__440B1D61");
        });

        modelBuilder.Entity<ОконныеПроемы>(entity =>
        {
            entity.HasKey(e => e.КодПроема).HasName("PK__Оконные___1ED57438A25F8EC3");

            entity.HasOne(d => d.КодЗамераNavigation).WithMany(p => p.ОконныеПроемы)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Оконные_п__код_з__59063A47");
        });

        modelBuilder.Entity<Пользователи>(entity =>
        {
            entity.HasKey(e => e.КодПользователя).HasName("PK__Пользова__9E26E2A10D25CFE9");

            entity.HasOne(d => d.КодРолиNavigation).WithMany(p => p.Пользователи)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Пользоват__код_р__37A5467C");
        });

        modelBuilder.Entity<Профили>(entity =>
        {
            entity.HasKey(e => e.КодПрофиля).HasName("PK__Профили__B0EAA32977055DD5");
        });

        modelBuilder.Entity<Роли>(entity =>
        {
            entity.HasKey(e => e.КодРоли).HasName("PK__Роли__B0ADB95E25348257");
        });

        modelBuilder.Entity<СтатусыЗаказа>(entity =>
        {
            entity.HasKey(e => e.КодСтатусаЗаказа).HasName("PK__Статусы___60E4673DB6E9B43B");
        });

        modelBuilder.Entity<Створки>(entity =>
        {
            entity.HasKey(e => e.КодСтворки).HasName("PK__Створки__9A8B4FBED9EC5F3B");

            entity.HasOne(d => d.КодОкнаNavigation).WithMany(p => p.Створки)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Створки__код_окн__66603565");

            entity.HasOne(d => d.КодТипаСтворкиNavigation).WithMany(p => p.Створки)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Створки__код_тип__6754599E");
        });

        modelBuilder.Entity<Стеклопакеты>(entity =>
        {
            entity.HasKey(e => e.КодСтеклопакета).HasName("PK__Стеклопа__293724A15640AE26");
        });

        modelBuilder.Entity<ТипыКомплектующих>(entity =>
        {
            entity.HasKey(e => e.КодТипа).HasName("PK__Типы_ком__626167EF3EEE27BB");
        });

        modelBuilder.Entity<ТипыСтворок>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Типы_ств__C8AC1204E02DF4EC");
        });

        modelBuilder.Entity<ТипыТоваров>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Типы_тов__C8AC1204C15E286E");
        });

        modelBuilder.Entity<ТипыУслуг>(entity =>
        {
            entity.HasKey(e => e.КодТипаУслуги).HasName("PK__Типы_усл__A93C1E5657C3AA57");
        });

        modelBuilder.Entity<Товары>(entity =>
        {
            entity.HasKey(e => e.КодТовара).HasName("PK__Товары__0D4F8642AA146AD9");

            entity.Property(e => e.Активный).HasDefaultValue(true);

            entity.HasOne(d => d.КодТипаТовараNavigation).WithMany(p => p.Товары)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Товары__тип_това__403A8C7D");
        });

        modelBuilder.Entity<ТоварыВЗаказе>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Товары_в__C8AC1204FD02F540");

            entity.HasOne(d => d.КодЗаказаNavigation).WithMany(p => p.ТоварыВЗаказе)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Товары_в___код_з__5165187F");

            entity.HasOne(d => d.КодОконногоПроемаNavigation).WithMany(p => p.ТоварыВЗаказе)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Товары_в_заказе_Оконные_проемы");

            entity.HasOne(d => d.КодТовараNavigation).WithMany(p => p.ТоварыВЗаказе)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Товары_в___код_т__52593CB8");
        });

        modelBuilder.Entity<Услуги>(entity =>
        {
            entity.HasKey(e => e.КодУслуги).HasName("PK__Услуги__9A31DDB201A2518E");

            entity.Property(e => e.Активна).HasDefaultValue(true);

            entity.HasOne(d => d.КодТипаУслугиNavigation).WithMany(p => p.Услуги)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Услуги__код_типа__03F0984C");
        });

        modelBuilder.Entity<УслугиВЗаказе>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Услуги_в__C8AC120476611D0E");

            entity.Property(e => e.Количество).HasDefaultValue(1);

            entity.HasOne(d => d.КодЗаказаNavigation).WithMany(p => p.УслугиВЗаказе)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Услуги_в___код_з__06CD04F7");

            entity.HasOne(d => d.КодУслугиNavigation).WithMany(p => p.УслугиВЗаказе)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Услуги_в___код_у__07C12930");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
