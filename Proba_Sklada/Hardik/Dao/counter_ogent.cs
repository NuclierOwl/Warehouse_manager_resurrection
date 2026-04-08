using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class counter_ogent
{
    public int id { get; set; }

    public string name { get; set; } = null!;

    public string type { get; set; } = null!;

    public string? inn { get; set; }

    public string? address { get; set; }

    public string? phone { get; set; }

    public string? email { get; set; }

    public string? contact_person { get; set; }

    public bool? is_active { get; set; }

    public DateTime? created_at { get; set; }

    public virtual ICollection<postuplenium> postuplenia { get; set; } = new List<postuplenium>();

    public virtual ICollection<schet_faktura> schet_fakturas { get; set; } = new List<schet_faktura>();
}
