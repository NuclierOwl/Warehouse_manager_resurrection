using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class product_category
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public int? parent_id { get; set; }

    public string? description { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<product_category> Inverseparent { get; set; } = new List<product_category>();

    public virtual product_category? parent { get; set; }

    public virtual ICollection<product> products { get; set; } = new List<product>();
}
