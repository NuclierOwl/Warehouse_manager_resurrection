using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class inventory_discrepancy
{
    public int id { get; set; }

    public int inventory_id { get; set; }

    public int product_id { get; set; }

    public int location_id { get; set; }

    public int expected_quantity { get; set; }

    public int actual_quantity { get; set; }

    public int? discrepancy { get; set; }

    public string? discrepancy_reason { get; set; }

    public bool? resolved { get; set; }

    public DateTime? resolved_at { get; set; }

    public int? resolved_by { get; set; }

    public virtual ostatki inventory { get; set; } = null!;

    public virtual storage_location location { get; set; } = null!;

    public virtual product product { get; set; } = null!;

    public virtual user? resolved_byNavigation { get; set; }
}
