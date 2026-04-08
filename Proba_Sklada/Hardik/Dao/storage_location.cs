using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class storage_location
{
    public int id { get; set; }

    public int rack_id { get; set; }

    public string location_code { get; set; } = null!;

    public int? level { get; set; }

    public int? position { get; set; }

    public decimal? width { get; set; }

    public decimal? height { get; set; }

    public decimal? depth { get; set; }

    public decimal? max_weight { get; set; }

    public bool? is_occupied { get; set; }

    public virtual ICollection<inventory> inventories { get; set; } = new List<inventory>();

    public virtual ICollection<inventory_discrepancy> inventory_discrepancies { get; set; } = new List<inventory_discrepancy>();

    public virtual ICollection<postuplenia_item> postuplenia_items { get; set; } = new List<postuplenia_item>();

    public virtual rack rack { get; set; } = null!;

    public virtual ICollection<vnutrinie_movement> vnutrinie_movementfrom_locations { get; set; } = new List<vnutrinie_movement>();

    public virtual ICollection<vnutrinie_movement> vnutrinie_movementto_locations { get; set; } = new List<vnutrinie_movement>();
}
