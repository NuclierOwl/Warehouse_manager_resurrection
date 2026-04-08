using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class postuplenia_item
{
    public int id { get; set; }

    public int invoice_id { get; set; }

    public int product_id { get; set; }

    public int location_id { get; set; }

    public int quantity { get; set; }

    public decimal unit_price { get; set; }

    public decimal? total_price { get; set; }

    public string? batch_number { get; set; }

    public DateOnly? production_date { get; set; }

    public DateOnly? expiration_date { get; set; }

    public string? serial_number { get; set; }

    public virtual postuplenium invoice { get; set; } = null!;

    public virtual storage_location location { get; set; } = null!;

    public virtual product product { get; set; } = null!;
}
