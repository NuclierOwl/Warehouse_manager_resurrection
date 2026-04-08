using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class unit
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string short_name { get; set; } = null!;

    public virtual ICollection<product> products { get; set; } = new List<product>();
}
