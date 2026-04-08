using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class rack
{
    public int id { get; set; }

    public int zone_id { get; set; }

    public string name { get; set; } = null!;

    public string? description { get; set; }

    public virtual ICollection<storage_location> storage_locations { get; set; } = new List<storage_location>();

    public virtual storage_zone zone { get; set; } = null!;
}
