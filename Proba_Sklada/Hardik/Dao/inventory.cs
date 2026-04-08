using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class inventory
{
    public int id { get; set; }

    public int product_id { get; set; }

    public int location_id { get; set; }

    public int kolichestvo { get; set; }

    public string? batch_number { get; set; }

    public DateOnly? production_date { get; set; }

    public DateOnly? spisanie_date { get; set; }

    public string? serial_number { get; set; }

    public DateTime? last_updated { get; set; }

    public virtual storage_location location { get; set; } = null!;

    public virtual product product { get; set; } = null!;
}
