using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class user
{
    public int id { get; set; }

    public string username { get; set; } = null!;

    public string password { get; set; } = null!;

    public string full_name { get; set; } = null!;

    public string? email { get; set; }

    public string? role { get; set; }

    public bool? is_active { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? last_login { get; set; }

    public virtual ICollection<inventory_discrepancy> inventory_discrepancies { get; set; } = new List<inventory_discrepancy>();

    public virtual ICollection<ostatki> ostatkis { get; set; } = new List<ostatki>();

    public virtual ICollection<postuplenium> postuplenia { get; set; } = new List<postuplenium>();

    public virtual ICollection<schet_faktura_soderzanie> schet_faktura_soderzanies { get; set; } = new List<schet_faktura_soderzanie>();

    public virtual ICollection<schet_faktura> schet_fakturas { get; set; } = new List<schet_faktura>();

    public virtual ICollection<vnutrinie_movement> vnutrinie_movements { get; set; } = new List<vnutrinie_movement>();
}
