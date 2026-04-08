using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class product
{
    public int id { get; set; }

    public string sku { get; set; } = null!;

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public int? category_id { get; set; }

    public int unit_id { get; set; }

    public decimal? weight { get; set; }

    public decimal? volume { get; set; }

    public int? min_stock_level { get; set; }

    public int? max_stock_level { get; set; }

    public decimal? purchase_price { get; set; }

    public decimal? selling_price { get; set; }

    public string? barcode { get; set; }

    public bool? is_active { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual product_category? category { get; set; }

    public virtual ICollection<inventory> inventories { get; set; } = new List<inventory>();

    public virtual ICollection<inventory_discrepancy> inventory_discrepancies { get; set; } = new List<inventory_discrepancy>();

    public virtual ICollection<postuplenia_item> postuplenia_items { get; set; } = new List<postuplenia_item>();

    public virtual ICollection<schet_faktura_soderzanie> schet_faktura_soderzanies { get; set; } = new List<schet_faktura_soderzanie>();

    public virtual unit unit { get; set; } = null!;

    public virtual ICollection<vnutrinie_movement> vnutrinie_movements { get; set; } = new List<vnutrinie_movement>();
}
