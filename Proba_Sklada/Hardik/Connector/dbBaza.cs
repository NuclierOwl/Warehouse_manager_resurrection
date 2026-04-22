using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Proba_Sklada.Hardik.Dao;

namespace Proba_Sklada.Hardik.Connector;

public partial class dbBaza : DbContext
{
    public dbBaza()
    {
    }

    public dbBaza(DbContextOptions<dbBaza> options)
        : base(options)
    {
    }

    public virtual DbSet<counter_ogent> counter_ogents { get; set; }

    public virtual DbSet<inventory> inventories { get; set; }

    public virtual DbSet<inventory_discrepancy> inventory_discrepancies { get; set; }

    public virtual DbSet<ostatki> ostatkis { get; set; }

    public virtual DbSet<postuplenia_item> postuplenia_items { get; set; }

    public virtual DbSet<postuplenium> postuplenia { get; set; }

    public virtual DbSet<product> products { get; set; }

    public virtual DbSet<product_category> product_categories { get; set; }

    public virtual DbSet<rack> racks { get; set; }

    public virtual DbSet<schet_faktura> schet_fakturas { get; set; }

    public virtual DbSet<schet_faktura_soderzanie> schet_faktura_soderzanies { get; set; }

    public virtual DbSet<storage_location> storage_locations { get; set; }

    public virtual DbSet<storage_zone> storage_zones { get; set; }

    public virtual DbSet<unit> units { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<vnutrinie_movement> vnutrinie_movements { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;

        var cs = Environment.GetEnvironmentVariable("ConnectionStrings__dbBaza")
                 ?? Environment.GetEnvironmentVariable("DBBAZA_CONNECTION");

        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException(
                "Database is not configured. Configure DI (AddDbContext<dbBaza>) or set environment variable 'ConnectionStrings__dbBaza'.");

        optionsBuilder.UseNpgsql(cs);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<counter_ogent>(entity =>
        {
            entity.HasKey(e => e.id).HasName("counterparties_pkey");

            entity.ToTable("counter_ogents", "sklad");

            entity.Property(e => e.id).HasDefaultValueSql("nextval('sklad.counterparties_id_seq'::regclass)");
            entity.Property(e => e.contact_person).HasMaxLength(100);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.inn).HasMaxLength(20);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.name).HasMaxLength(200);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.type).HasMaxLength(10);
        });

        modelBuilder.Entity<inventory>(entity =>
        {
            entity.HasKey(e => e.id).HasName("inventory_pkey");

            entity.ToTable("inventory", "sklad");

            entity.HasIndex(e => e.spisanie_date, "idx_inventory_expiration");

            entity.HasIndex(e => e.location_id, "idx_inventory_location");

            entity.HasIndex(e => e.product_id, "idx_inventory_product");

            entity.HasIndex(e => new { e.product_id, e.location_id, e.batch_number, e.serial_number }, "inventory_product_id_location_id_batch_number_serial_number_key").IsUnique();

            entity.Property(e => e.batch_number).HasMaxLength(100);
            entity.Property(e => e.last_updated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.serial_number).HasMaxLength(100);

            entity.HasOne(d => d.location).WithMany(p => p.inventories)
                .HasForeignKey(d => d.location_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_location_id_fkey");

            entity.HasOne(d => d.product).WithMany(p => p.inventories)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_product_id_fkey");
        });

        modelBuilder.Entity<inventory_discrepancy>(entity =>
        {
            entity.HasKey(e => e.id).HasName("inventory_discrepancies_pkey");

            entity.ToTable("inventory_discrepancies", "sklad");

            entity.HasIndex(e => new { e.inventory_id, e.product_id, e.location_id }, "inventory_discrepancies_inventory_id_product_id_location_id_key").IsUnique();

            entity.Property(e => e.discrepancy).HasComputedColumnSql("(actual_quantity - expected_quantity)", true);
            entity.Property(e => e.discrepancy_reason).HasMaxLength(200);
            entity.Property(e => e.resolved).HasDefaultValue(false);
            entity.Property(e => e.resolved_at).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.inventory).WithMany(p => p.inventory_discrepancies)
                .HasForeignKey(d => d.inventory_id)
                .HasConstraintName("inventory_discrepancies_inventory_id_fkey");

            entity.HasOne(d => d.location).WithMany(p => p.inventory_discrepancies)
                .HasForeignKey(d => d.location_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_discrepancies_location_id_fkey");

            entity.HasOne(d => d.product).WithMany(p => p.inventory_discrepancies)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventory_discrepancies_product_id_fkey");

            entity.HasOne(d => d.resolved_byNavigation).WithMany(p => p.inventory_discrepancies)
                .HasForeignKey(d => d.resolved_by)
                .HasConstraintName("inventory_discrepancies_resolved_by_fkey");
        });

        modelBuilder.Entity<ostatki>(entity =>
        {
            entity.HasKey(e => e.id).HasName("inventories_pkey");

            entity.ToTable("ostatki", "sklad");

            entity.HasIndex(e => e.inventory_number, "inventories_inventory_number_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('sklad.inventories_id_seq'::regclass)");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.inventory_number).HasMaxLength(50);
            entity.Property(e => e.status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'planned'::character varying");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.ostatkis)
                .HasForeignKey(d => d.created_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("inventories_created_by_fkey");
        });

        modelBuilder.Entity<postuplenia_item>(entity =>
        {
            entity.HasKey(e => e.id).HasName("income_invoice_items_pkey");

            entity.ToTable("postuplenia_items", "sklad");

            entity.HasIndex(e => new { e.invoice_id, e.product_id, e.location_id }, "income_invoice_items_invoice_id_product_id_location_id_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('sklad.income_invoice_items_id_seq'::regclass)");
            entity.Property(e => e.batch_number).HasMaxLength(100);
            entity.Property(e => e.serial_number).HasMaxLength(100);
            entity.Property(e => e.total_price)
                .HasPrecision(15, 2)
                .HasComputedColumnSql("((quantity)::numeric * unit_price)", true);
            entity.Property(e => e.unit_price).HasPrecision(15, 2);

            entity.HasOne(d => d.invoice).WithMany(p => p.postuplenia_items)
                .HasForeignKey(d => d.invoice_id)
                .HasConstraintName("income_invoice_items_invoice_id_fkey");

            entity.HasOne(d => d.location).WithMany(p => p.postuplenia_items)
                .HasForeignKey(d => d.location_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("income_invoice_items_location_id_fkey");

            entity.HasOne(d => d.product).WithMany(p => p.postuplenia_items)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("income_invoice_items_product_id_fkey");
        });

        modelBuilder.Entity<postuplenium>(entity =>
        {
            entity.HasKey(e => e.id).HasName("income_invoices_pkey");

            entity.ToTable("postuplenia", "sklad");

            entity.HasIndex(e => e.invoice_date, "idx_income_invoices_date");

            entity.HasIndex(e => e.invoice_number, "income_invoices_invoice_number_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('sklad.income_invoices_id_seq'::regclass)");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.invoice_number).HasMaxLength(50);
            entity.Property(e => e.status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'draft'::character varying");
            entity.Property(e => e.total_amount)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.postuplenia)
                .HasForeignKey(d => d.created_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("income_invoices_created_by_fkey");

            entity.HasOne(d => d.supplier).WithMany(p => p.postuplenia)
                .HasForeignKey(d => d.supplier_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("income_invoices_supplier_id_fkey");
        });

        modelBuilder.Entity<product>(entity =>
        {
            entity.HasKey(e => e.id).HasName("products_pkey");

            entity.ToTable("products", "sklad");

            entity.HasIndex(e => e.category_id, "idx_products_category");

            entity.HasIndex(e => e.sku, "idx_products_sku");

            entity.HasIndex(e => e.sku, "products_sku_key").IsUnique();

            entity.Property(e => e.barcode).HasMaxLength(100);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.min_stock_level).HasDefaultValue(0);
            entity.Property(e => e.name).HasMaxLength(200);
            entity.Property(e => e.purchase_price).HasPrecision(15, 2);
            entity.Property(e => e.selling_price).HasPrecision(15, 2);
            entity.Property(e => e.sku).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.volume).HasPrecision(10, 3);
            entity.Property(e => e.weight).HasPrecision(10, 3);

            entity.HasOne(d => d.category).WithMany(p => p.products)
                .HasForeignKey(d => d.category_id)
                .HasConstraintName("products_category_id_fkey");

            entity.HasOne(d => d.unit).WithMany(p => p.products)
                .HasForeignKey(d => d.unit_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_unit_id_fkey");
        });

        modelBuilder.Entity<product_category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("product_categories_pkey");

            entity.ToTable("product_categories", "sklad");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.name).HasMaxLength(100);

            entity.HasOne(d => d.parent).WithMany(p => p.Inverseparent)
                .HasForeignKey(d => d.parent_id)
                .HasConstraintName("product_categories_parent_id_fkey");
        });

        modelBuilder.Entity<rack>(entity =>
        {
            entity.HasKey(e => e.id).HasName("racks_pkey");

            entity.ToTable("racks", "sklad");

            entity.Property(e => e.name).HasMaxLength(50);

            entity.HasOne(d => d.zone).WithMany(p => p.racks)
                .HasForeignKey(d => d.zone_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("racks_zone_id_fkey");
        });

        modelBuilder.Entity<schet_faktura>(entity =>
        {
            entity.HasKey(e => e.id).HasName("expense_invoices_pkey");

            entity.ToTable("schet_faktura", "sklad");

            entity.HasIndex(e => e.invoice_number, "expense_invoices_invoice_number_key").IsUnique();

            entity.HasIndex(e => e.invoice_date, "idx_expense_invoices_date");

            entity.Property(e => e.id).HasDefaultValueSql("nextval('sklad.expense_invoices_id_seq'::regclass)");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.invoice_number).HasMaxLength(50);
            entity.Property(e => e.status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'draft'::character varying");
            entity.Property(e => e.total_amount)
                .HasPrecision(15, 2)
                .HasDefaultValue(0m);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.schet_fakturas)
                .HasForeignKey(d => d.created_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expense_invoices_created_by_fkey");

            entity.HasOne(d => d.customer).WithMany(p => p.schet_fakturas)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expense_invoices_customer_id_fkey");
        });

        modelBuilder.Entity<schet_faktura_soderzanie>(entity =>
        {
            entity.HasKey(e => e.id).HasName("expense_invoice_items_pkey");

            entity.ToTable("schet_faktura_soderzanie", "sklad");

            entity.HasIndex(e => new { e.invoice_id, e.product_id }, "expense_invoice_items_invoice_id_product_id_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('sklad.expense_invoice_items_id_seq'::regclass)");
            entity.Property(e => e.picked_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.picked_quantity).HasDefaultValue(0);
            entity.Property(e => e.total_price)
                .HasPrecision(15, 2)
                .HasComputedColumnSql("((quantity)::numeric * unit_price)", true);
            entity.Property(e => e.unit_price).HasPrecision(15, 2);

            entity.HasOne(d => d.invoice).WithMany(p => p.schet_faktura_soderzanies)
                .HasForeignKey(d => d.invoice_id)
                .HasConstraintName("expense_invoice_items_invoice_id_fkey");

            entity.HasOne(d => d.picked_byNavigation).WithMany(p => p.schet_faktura_soderzanies)
                .HasForeignKey(d => d.picked_by)
                .HasConstraintName("expense_invoice_items_picked_by_fkey");

            entity.HasOne(d => d.product).WithMany(p => p.schet_faktura_soderzanies)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("expense_invoice_items_product_id_fkey");
        });

        modelBuilder.Entity<storage_location>(entity =>
        {
            entity.HasKey(e => e.id).HasName("storage_locations_pkey");

            entity.ToTable("storage_locations", "sklad");

            entity.HasIndex(e => e.location_code, "idx_storage_locations_code");

            entity.HasIndex(e => e.location_code, "storage_locations_location_code_key").IsUnique();

            entity.Property(e => e.depth).HasPrecision(8, 2);
            entity.Property(e => e.height).HasPrecision(8, 2);
            entity.Property(e => e.is_occupied).HasDefaultValue(false);
            entity.Property(e => e.location_code).HasMaxLength(50);
            entity.Property(e => e.max_weight).HasPrecision(10, 2);
            entity.Property(e => e.width).HasPrecision(8, 2);

            entity.HasOne(d => d.rack).WithMany(p => p.storage_locations)
                .HasForeignKey(d => d.rack_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("storage_locations_rack_id_fkey");
        });

        modelBuilder.Entity<storage_zone>(entity =>
        {
            entity.HasKey(e => e.id).HasName("storage_zones_pkey");

            entity.ToTable("storage_zones", "sklad");

            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.temperature_condition).HasMaxLength(50);
        });

        modelBuilder.Entity<unit>(entity =>
        {
            entity.HasKey(e => e.id).HasName("units_pkey");

            entity.ToTable("units", "sklad");

            entity.Property(e => e.name).HasMaxLength(50);
            entity.Property(e => e.short_name).HasMaxLength(10);
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.ToTable("users", "sklad");

            entity.HasIndex(e => e.username, "users_username_key").IsUnique();

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.full_name).HasMaxLength(100);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.last_login).HasColumnType("timestamp without time zone");
            entity.Property(e => e.password).HasMaxLength(255);
            entity.Property(e => e.role)
                .HasMaxLength(20)
                .HasDefaultValueSql("'operator'::character varying");
            entity.Property(e => e.username).HasMaxLength(50);
        });

        modelBuilder.Entity<vnutrinie_movement>(entity =>
        {
            entity.HasKey(e => e.id).HasName("internal_movements_pkey");

            entity.ToTable("vnutrinie_movements", "sklad");

            entity.HasIndex(e => e.movement_number, "internal_movements_movement_number_key").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('sklad.internal_movements_id_seq'::regclass)");
            entity.Property(e => e.completed).HasDefaultValue(false);
            entity.Property(e => e.movement_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.movement_number).HasMaxLength(50);
            entity.Property(e => e.reason).HasMaxLength(200);

            entity.HasOne(d => d.from_location).WithMany(p => p.vnutrinie_movementfrom_locations)
                .HasForeignKey(d => d.from_location_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("internal_movements_from_location_id_fkey");

            entity.HasOne(d => d.moved_byNavigation).WithMany(p => p.vnutrinie_movements)
                .HasForeignKey(d => d.moved_by)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("internal_movements_moved_by_fkey");

            entity.HasOne(d => d.product).WithMany(p => p.vnutrinie_movements)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("internal_movements_product_id_fkey");

            entity.HasOne(d => d.to_location).WithMany(p => p.vnutrinie_movementto_locations)
                .HasForeignKey(d => d.to_location_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("internal_movements_to_location_id_fkey");
        });
        modelBuilder.HasSequence("products_id_new_seq", "sklad");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
