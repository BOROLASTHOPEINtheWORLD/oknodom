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

    public virtual DbSet<Бригады> Бригадыs { get; set; }

    public virtual DbSet<ВыполнениеМонтажа> ВыполнениеМонтажаs { get; set; }

    public virtual DbSet<Заказы> Заказыs { get; set; }

    public virtual DbSet<Замеры> Замерыs { get; set; }

    public virtual DbSet<Комплектующие> Комплектующиеs { get; set; }

    public virtual DbSet<Материалы> Материалыs { get; set; }

    public virtual DbSet<Окна> Окнаs { get; set; }

    public virtual DbSet<ОконныеПроемы> ОконныеПроемыs { get; set; }

    public virtual DbSet<Пользователи> Пользователиs { get; set; }

    public virtual DbSet<Профили> Профилиs { get; set; }

    public virtual DbSet<Роли> Ролиs { get; set; }

    public virtual DbSet<СтатусыЗаказа> СтатусыЗаказаs { get; set; }

    public virtual DbSet<СтатусыСотрудников> СтатусыСотрудниковs { get; set; }

    public virtual DbSet<Створки> Створкиs { get; set; }

    public virtual DbSet<Стеклопакеты> Стеклопакетыs { get; set; }

    public virtual DbSet<ТипыКомплектующих> ТипыКомплектующихs { get; set; }

    public virtual DbSet<ТипыСтворок> ТипыСтворокs { get; set; }

    public virtual DbSet<ТипыТоваров> ТипыТоваровs { get; set; }

    public virtual DbSet<ТипыУслуг> ТипыУслугs { get; set; }

    public virtual DbSet<Товары> Товарыs { get; set; }

    public virtual DbSet<ТоварыВЗаказе> ТоварыВЗаказеs { get; set; }

    public virtual DbSet<Услуги> Услугиs { get; set; }

    public virtual DbSet<УслугиВЗаказе> УслугиВЗаказеs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DeffaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Бригады>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Бригады__C8AC1204C1F891E5");

            entity.ToTable("Бригады");

            entity.HasIndex(e => new { e.КодВыполненияМонтажа, e.КодМонтажника }, "IX_бригады_код_выполнения_монтажа__код_монтажника");

            entity.Property(e => e.Код).HasColumnName("код");
            entity.Property(e => e.КодВыполненияМонтажа).HasColumnName("код_выполнения_монтажа");
            entity.Property(e => e.КодМонтажника).HasColumnName("код_монтажника");

            entity.HasOne(d => d.КодВыполненияМонтажаNavigation).WithMany(p => p.Бригадыs)
                .HasForeignKey(d => d.КодВыполненияМонтажа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Бригады__код_вып__628FA481");

            entity.HasOne(d => d.КодМонтажникаNavigation).WithMany(p => p.Бригадыs)
                .HasForeignKey(d => d.КодМонтажника)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Бригады__код_мон__6383C8BA");
        });

        modelBuilder.Entity<ВыполнениеМонтажа>(entity =>
        {
            entity.HasKey(e => e.КодВыполнения).HasName("PK__Выполнен__9AD071A2DB0E40E1");

            entity.ToTable("Выполнение_монтажа");

            entity.Property(e => e.КодВыполнения)
                .ValueGeneratedOnAdd()
                .HasColumnName("код_выполнения");
            entity.Property(e => e.ДатаВыполнения)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("дата_выполнения");
            entity.Property(e => e.КодЗаказа).HasColumnName("код_заказа");
            entity.Property(e => e.КодОконногоПроема).HasColumnName("код_оконного_проема");
            entity.Property(e => e.КодУстановленногоОкна).HasColumnName("код_установленного_окна");
            entity.Property(e => e.Фотография)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("фотография");

            entity.HasOne(d => d.КодВыполненияNavigation).WithOne(p => p.ВыполнениеМонтажа)
                .HasForeignKey<ВыполнениеМонтажа>(d => d.КодВыполнения)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Выполнение_монтажа_Товары_в_заказе");

            entity.HasOne(d => d.КодЗаказаNavigation).WithMany(p => p.ВыполнениеМонтажаs)
                .HasForeignKey(d => d.КодЗаказа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Выполнени__код_з__5BE2A6F2");

            entity.HasOne(d => d.КодОконногоПроемаNavigation).WithMany(p => p.ВыполнениеМонтажаs)
                .HasForeignKey(d => d.КодОконногоПроема)
                .HasConstraintName("FK__Выполнени__код_о__5CD6CB2B");
        });

        modelBuilder.Entity<Заказы>(entity =>
        {
            entity.HasKey(e => e.КодЗаказа).HasName("PK__Заказы__5F93D6C5544B7A37");

            entity.ToTable("Заказы");

            entity.HasIndex(e => new { e.КодСтатусаЗаказа, e.ДатаСозданияЗаказа }, "IX_Заказы_Статус_Дата");

            entity.HasIndex(e => new { e.КодКлиента, e.Стоимость }, "ix_заказы_клиент_стоимость");

            entity.Property(e => e.КодЗаказа).HasColumnName("код_заказа");
            entity.Property(e => e.Адрес).HasColumnName("адрес");
            entity.Property(e => e.ДатаСозданияЗаказа)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("дата_создания_заказа");
            entity.Property(e => e.КодКлиента).HasColumnName("код_клиента");
            entity.Property(e => e.КодСтатусаЗаказа).HasColumnName("код_статуса_заказа");
            entity.Property(e => e.СтатусОплаты).HasColumnName("статус_оплаты");
            entity.Property(e => e.Стоимость)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("стоимость");

            entity.HasOne(d => d.КодКлиентаNavigation).WithMany(p => p.Заказыs)
                .HasForeignKey(d => d.КодКлиента)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Заказы__код_клие__3B75D760");

            entity.HasOne(d => d.КодСтатусаЗаказаNavigation).WithMany(p => p.Заказыs)
                .HasForeignKey(d => d.КодСтатусаЗаказа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Заказы__код_стат__3C69FB99");
        });

        modelBuilder.Entity<Замеры>(entity =>
        {
            entity.HasKey(e => e.КодЗамера).HasName("PK__Замеры__8556EC8BDDBA21EC");

            entity.ToTable("Замеры");

            entity.Property(e => e.КодЗамера).HasColumnName("код_замера");
            entity.Property(e => e.ДатаЗамера).HasColumnName("дата_замера");
            entity.Property(e => e.КодЗаказа).HasColumnName("код_заказа");
            entity.Property(e => e.КодЗамерщика).HasColumnName("код_замерщика");

            entity.HasOne(d => d.КодЗаказаNavigation).WithMany(p => p.Замерыs)
                .HasForeignKey(d => d.КодЗаказа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Замеры__код_зака__5629CD9C");

            entity.HasOne(d => d.КодЗамерщикаNavigation).WithMany(p => p.Замерыs)
                .HasForeignKey(d => d.КодЗамерщика)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Замеры__код_заме__5535A963");
        });

        modelBuilder.Entity<Комплектующие>(entity =>
        {
            entity.HasKey(e => e.КодКомплектующего).HasName("PK__Комплект__1F177FD6A0E4B2B8");

            entity.ToTable("Комплектующие");

            entity.Property(e => e.КодКомплектующего)
                .ValueGeneratedNever()
                .HasColumnName("код_комплектующего");
            entity.Property(e => e.ВесКг)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("вес_кг");
            entity.Property(e => e.ВысотаМм).HasColumnName("высота_мм");
            entity.Property(e => e.ДлинаМм).HasColumnName("длина_мм");
            entity.Property(e => e.КодМатериала).HasColumnName("код_материала");
            entity.Property(e => e.КодТипаКомплектующего).HasColumnName("код_типа_комплектующего");
            entity.Property(e => e.ШиринаМм).HasColumnName("ширина_мм");

            entity.HasOne(d => d.КодКомплектующегоNavigation).WithOne(p => p.Комплектующие)
                .HasForeignKey<Комплектующие>(d => d.КодКомплектующего)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Комплекту__код_к__49C3F6B7");

            entity.HasOne(d => d.КодМатериалаNavigation).WithMany(p => p.Комплектующиеs)
                .HasForeignKey(d => d.КодМатериала)
                .HasConstraintName("FK__Комплекту__код_м__4BAC3F29");

            entity.HasOne(d => d.КодТипаКомплектующегоNavigation).WithMany(p => p.Комплектующиеs)
                .HasForeignKey(d => d.КодТипаКомплектующего)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Комплекту__код_т__4AB81AF0");
        });

        modelBuilder.Entity<Материалы>(entity =>
        {
            entity.HasKey(e => e.КодМатериала).HasName("PK__Материал__F1BA6D2DE462E30F");

            entity.ToTable("Материалы");

            entity.HasIndex(e => e.Название, "UQ__Материал__5ED3ECC60910AD9D").IsUnique();

            entity.Property(e => e.КодМатериала).HasColumnName("код_материала");
            entity.Property(e => e.Название)
                .HasMaxLength(100)
                .HasColumnName("название");
            entity.Property(e => e.Описание)
                .HasMaxLength(255)
                .HasColumnName("описание");
        });

        modelBuilder.Entity<Окна>(entity =>
        {
            entity.HasKey(e => e.КодОкна).HasName("PK__Окна__29C955F08BEEE392");

            entity.ToTable("Окна");

            entity.Property(e => e.КодОкна)
                .ValueGeneratedNever()
                .HasColumnName("код_окна");
            entity.Property(e => e.Высота)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("высота");
            entity.Property(e => e.КодПрофиля).HasColumnName("код_профиля");
            entity.Property(e => e.КодСтеклопакета).HasColumnName("код_стеклопакета");
            entity.Property(e => e.КоличествоСтворок).HasColumnName("количество_створок");
            entity.Property(e => e.Стандартное).HasColumnName("стандартное");
            entity.Property(e => e.Ширина)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ширина");

            entity.HasOne(d => d.КодОкнаNavigation).WithOne(p => p.Окна)
                .HasForeignKey<Окна>(d => d.КодОкна)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Окна__код_окна__440B1D61");

            entity.HasOne(d => d.КодПрофиляNavigation).WithMany(p => p.Окнаs)
                .HasForeignKey(d => d.КодПрофиля)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Окна__код_профил__44FF419A");

            entity.HasOne(d => d.КодСтеклопакетаNavigation).WithMany(p => p.Окнаs)
                .HasForeignKey(d => d.КодСтеклопакета)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Окна__код_стекло__45F365D3");
        });

        modelBuilder.Entity<ОконныеПроемы>(entity =>
        {
            entity.HasKey(e => e.КодПроема).HasName("PK__Оконные___1ED57438A25F8EC3");

            entity.ToTable("Оконные_проемы");

            entity.Property(e => e.КодПроема).HasColumnName("код_проема");
            entity.Property(e => e.Высота)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("высота");
            entity.Property(e => e.КодЗамера).HasColumnName("код_замера");
            entity.Property(e => e.Описание).HasColumnName("описание");
            entity.Property(e => e.Ширина)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ширина");

            entity.HasOne(d => d.КодЗамераNavigation).WithMany(p => p.ОконныеПроемыs)
                .HasForeignKey(d => d.КодЗамера)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Оконные_п__код_з__59063A47");
        });

        modelBuilder.Entity<Пользователи>(entity =>
        {
            entity.HasKey(e => e.КодПользователя).HasName("PK__Пользова__9E26E2A10D25CFE9");

            entity.ToTable("Пользователи");

            entity.HasIndex(e => e.Логин, "UX_Пользователи_Логин").IsUnique();

            entity.HasIndex(e => e.Телефон, "UX_Пользователи_Телефон").IsUnique();

            entity.Property(e => e.КодПользователя).HasColumnName("код_пользователя");
            entity.Property(e => e.Имя)
                .HasMaxLength(100)
                .HasColumnName("имя");
            entity.Property(e => e.КодРоли).HasColumnName("код_роли");
            entity.Property(e => e.Логин)
                .HasMaxLength(50)
                .HasColumnName("логин");
            entity.Property(e => e.Отчество)
                .HasMaxLength(100)
                .HasColumnName("отчество");
            entity.Property(e => e.Пароль)
                .HasMaxLength(255)
                .HasColumnName("пароль");
            entity.Property(e => e.Паспорт)
                .HasMaxLength(12)
                .HasColumnName("паспорт");
            entity.Property(e => e.СтатусСотрудника).HasColumnName("статус_сотрудника");
            entity.Property(e => e.Телефон)
                .HasMaxLength(20)
                .HasColumnName("телефон");
            entity.Property(e => e.Фамилия)
                .HasMaxLength(100)
                .HasColumnName("фамилия");

            entity.HasOne(d => d.КодРолиNavigation).WithMany(p => p.Пользователиs)
                .HasForeignKey(d => d.КодРоли)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Пользоват__код_р__37A5467C");

            entity.HasOne(d => d.СтатусСотрудникаNavigation).WithMany(p => p.Пользователиs)
                .HasForeignKey(d => d.СтатусСотрудника)
                .HasConstraintName("FK__Пользоват__стату__38996AB5");
        });

        modelBuilder.Entity<Профили>(entity =>
        {
            entity.HasKey(e => e.КодПрофиля).HasName("PK__Профили__B0EAA32977055DD5");

            entity.ToTable("Профили");

            entity.Property(e => e.КодПрофиля).HasColumnName("код_профиля");
            entity.Property(e => e.Звукоизоляция)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("звукоизоляция");
            entity.Property(e => e.КоличествоКамер).HasColumnName("количество_камер");
            entity.Property(e => e.Название)
                .HasMaxLength(100)
                .HasColumnName("название");
            entity.Property(e => e.СопротивлениеТеплопередаче)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("сопротивление_теплопередаче");
            entity.Property(e => e.ТолщинаСтеклопакета)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("толщина_стеклопакета");
            entity.Property(e => e.Ширина)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("ширина");
        });

        modelBuilder.Entity<Роли>(entity =>
        {
            entity.HasKey(e => e.КодРоли).HasName("PK__Роли__B0ADB95E25348257");

            entity.ToTable("Роли");

            entity.Property(e => e.КодРоли).HasColumnName("код_роли");
            entity.Property(e => e.Название)
                .HasMaxLength(50)
                .HasColumnName("название");
        });

        modelBuilder.Entity<СтатусыЗаказа>(entity =>
        {
            entity.HasKey(e => e.КодСтатусаЗаказа).HasName("PK__Статусы___60E4673DB6E9B43B");

            entity.ToTable("Статусы_заказа");

            entity.Property(e => e.КодСтатусаЗаказа).HasColumnName("код_статуса_заказа");
            entity.Property(e => e.Название)
                .HasMaxLength(100)
                .HasColumnName("название");
        });

        modelBuilder.Entity<СтатусыСотрудников>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Статусы___C8AC1204D7ADAECC");

            entity.ToTable("Статусы_сотрудников");

            entity.Property(e => e.Код).HasColumnName("код");
            entity.Property(e => e.Название)
                .HasMaxLength(50)
                .HasColumnName("название");
        });

        modelBuilder.Entity<Створки>(entity =>
        {
            entity.HasKey(e => e.КодСтворки).HasName("PK__Створки__9A8B4FBED9EC5F3B");

            entity.ToTable("Створки");

            entity.Property(e => e.КодСтворки).HasColumnName("код_створки");
            entity.Property(e => e.КодОкна).HasColumnName("код_окна");
            entity.Property(e => e.КодТипаСтворки).HasColumnName("код_типа_створки");
            entity.Property(e => e.НомерСтворки).HasColumnName("номер_створки");

            entity.HasOne(d => d.КодОкнаNavigation).WithMany(p => p.Створкиs)
                .HasForeignKey(d => d.КодОкна)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Створки__код_окн__66603565");

            entity.HasOne(d => d.КодТипаСтворкиNavigation).WithMany(p => p.Створкиs)
                .HasForeignKey(d => d.КодТипаСтворки)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Створки__код_тип__6754599E");
        });

        modelBuilder.Entity<Стеклопакеты>(entity =>
        {
            entity.HasKey(e => e.КодСтеклопакета).HasName("PK__Стеклопа__293724A15640AE26");

            entity.ToTable("Стеклопакеты");

            entity.Property(e => e.КодСтеклопакета).HasColumnName("код_стеклопакета");
            entity.Property(e => e.Название)
                .HasMaxLength(100)
                .HasColumnName("название");
        });

        modelBuilder.Entity<ТипыКомплектующих>(entity =>
        {
            entity.HasKey(e => e.КодТипа).HasName("PK__Типы_ком__626167EF3EEE27BB");

            entity.ToTable("Типы_комплектующих");

            entity.HasIndex(e => e.Название, "UQ__Типы_ком__5ED3ECC607BDB074").IsUnique();

            entity.Property(e => e.КодТипа).HasColumnName("код_типа");
            entity.Property(e => e.Название)
                .HasMaxLength(50)
                .HasColumnName("название");
            entity.Property(e => e.Описание)
                .HasMaxLength(255)
                .HasColumnName("описание");
        });

        modelBuilder.Entity<ТипыСтворок>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Типы_ств__C8AC1204E02DF4EC");

            entity.ToTable("Типы_створок");

            entity.Property(e => e.Код).HasColumnName("код");
            entity.Property(e => e.Название)
                .HasMaxLength(100)
                .HasColumnName("название");
            entity.Property(e => e.Описание).HasColumnName("описание");
        });

        modelBuilder.Entity<ТипыТоваров>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Типы_тов__C8AC1204C15E286E");

            entity.ToTable("Типы_товаров");

            entity.Property(e => e.Код).HasColumnName("код");
            entity.Property(e => e.Название)
                .HasMaxLength(100)
                .HasColumnName("название");
        });

        modelBuilder.Entity<ТипыУслуг>(entity =>
        {
            entity.HasKey(e => e.КодТипаУслуги).HasName("PK__Типы_усл__A93C1E5657C3AA57");

            entity.ToTable("Типы_услуг");

            entity.HasIndex(e => e.Название, "UQ__Типы_усл__5ED3ECC696EADE5C").IsUnique();

            entity.Property(e => e.КодТипаУслуги).HasColumnName("код_типа_услуги");
            entity.Property(e => e.Название)
                .HasMaxLength(50)
                .HasColumnName("название");
            entity.Property(e => e.Описание)
                .HasMaxLength(255)
                .HasColumnName("описание");
        });

        modelBuilder.Entity<Товары>(entity =>
        {
            entity.HasKey(e => e.КодТовара).HasName("PK__Товары__0D4F8642AA146AD9");

            entity.ToTable("Товары");

            entity.Property(e => e.КодТовара).HasColumnName("код_товара");
            entity.Property(e => e.Активный)
                .HasDefaultValue(true)
                .HasColumnName("активный");
            entity.Property(e => e.Название)
                .HasMaxLength(255)
                .HasColumnName("название");
            entity.Property(e => e.ТипТовара).HasColumnName("тип_товара");
            entity.Property(e => e.Фото)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("фото");
            entity.Property(e => e.Цвет)
                .HasMaxLength(50)
                .HasColumnName("цвет");
            entity.Property(e => e.Цена)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("цена");

            entity.HasOne(d => d.ТипТовараNavigation).WithMany(p => p.Товарыs)
                .HasForeignKey(d => d.ТипТовара)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Товары__тип_това__403A8C7D");
        });

        modelBuilder.Entity<ТоварыВЗаказе>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Товары_в__C8AC1204FD02F540");

            entity.ToTable("Товары_в_заказе");

            entity.HasIndex(e => new { e.КодЗаказа, e.КодТовара }, "IX_ТоварыВЗаказе_ЗаказТовар");

            entity.Property(e => e.Код).HasColumnName("код");
            entity.Property(e => e.КодЗаказа).HasColumnName("код_заказа");
            entity.Property(e => e.КодОконногоПроема).HasColumnName("код_оконного_проема");
            entity.Property(e => e.КодТовара).HasColumnName("код_товара");
            entity.Property(e => e.Количество).HasColumnName("количество");
            entity.Property(e => e.ЦенаНаМоментЗаказа)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("цена_на_момент_заказа");

            entity.HasOne(d => d.КодЗаказаNavigation).WithMany(p => p.ТоварыВЗаказеs)
                .HasForeignKey(d => d.КодЗаказа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Товары_в___код_з__5165187F");

            entity.HasOne(d => d.КодОконногоПроемаNavigation).WithMany(p => p.ТоварыВЗаказеs)
                .HasForeignKey(d => d.КодОконногоПроема)
                .HasConstraintName("FK_Товары_в_заказе_Оконные_проемы");

            entity.HasOne(d => d.КодТовараNavigation).WithMany(p => p.ТоварыВЗаказеs)
                .HasForeignKey(d => d.КодТовара)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Товары_в___код_т__52593CB8");
        });

        modelBuilder.Entity<Услуги>(entity =>
        {
            entity.HasKey(e => e.КодУслуги).HasName("PK__Услуги__9A31DDB201A2518E");

            entity.ToTable("Услуги");

            entity.Property(e => e.КодУслуги).HasColumnName("код_услуги");
            entity.Property(e => e.Активна)
                .HasDefaultValue(true)
                .HasColumnName("активна");
            entity.Property(e => e.БазоваяСтоимость)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("базовая_стоимость");
            entity.Property(e => e.КодТипаУслуги).HasColumnName("код_типа_услуги");
            entity.Property(e => e.Название)
                .HasMaxLength(255)
                .HasColumnName("название");
            entity.Property(e => e.Описание).HasColumnName("описание");

            entity.HasOne(d => d.КодТипаУслугиNavigation).WithMany(p => p.Услугиs)
                .HasForeignKey(d => d.КодТипаУслуги)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Услуги__код_типа__03F0984C");
        });

        modelBuilder.Entity<УслугиВЗаказе>(entity =>
        {
            entity.HasKey(e => e.Код).HasName("PK__Услуги_в__C8AC120476611D0E");

            entity.ToTable("Услуги_в_заказе");

            entity.HasIndex(e => e.КодЗаказа, "IX_УслугиВЗаказе_Заказ");

            entity.Property(e => e.Код).HasColumnName("код");
            entity.Property(e => e.КодЗаказа).HasColumnName("код_заказа");
            entity.Property(e => e.КодТовараВЗаказе).HasColumnName("код_товара_в_заказе");
            entity.Property(e => e.КодУслуги).HasColumnName("код_услуги");
            entity.Property(e => e.Количество)
                .HasDefaultValue(1)
                .HasColumnName("количество");
            entity.Property(e => e.Примечание)
                .HasMaxLength(500)
                .HasColumnName("примечание");
            entity.Property(e => e.ЦенаНаМоментЗаказа)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("цена_на_момент_заказа");

            entity.HasOne(d => d.КодЗаказаNavigation).WithMany(p => p.УслугиВЗаказеs)
                .HasForeignKey(d => d.КодЗаказа)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Услуги_в___код_з__06CD04F7");

            entity.HasOne(d => d.КодТовараВЗаказеNavigation).WithMany(p => p.УслугиВЗаказеs)
                .HasForeignKey(d => d.КодТовараВЗаказе)
                .HasConstraintName("FK__Услуги_в___код_т__09A971A2");

            entity.HasOne(d => d.КодУслугиNavigation).WithMany(p => p.УслугиВЗаказеs)
                .HasForeignKey(d => d.КодУслуги)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Услуги_в___код_у__07C12930");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
