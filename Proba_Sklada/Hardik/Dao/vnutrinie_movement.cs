using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class vnutrinie_movement
{
    public int id { get; set; }

    public string movement_number { get; set; } = null!;

    public int product_id { get; set; }

    public int from_location_id { get; set; }

    public int to_location_id { get; set; }

    public int quantity { get; set; }

    public DateTime? movement_date { get; set; }

    public string? reason { get; set; }

    public int moved_by { get; set; }

    public bool? completed { get; set; }

    public virtual storage_location from_location { get; set; } = null!;

    public virtual user moved_byNavigation { get; set; } = null!;

    public virtual product product { get; set; } = null!;

    public virtual storage_location to_location { get; set; } = null!;
}
