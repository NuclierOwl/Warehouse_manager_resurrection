using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class storage_zone
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public string? temperature_condition { get; set; }

    public virtual ICollection<rack> racks { get; set; } = new List<rack>();
}
