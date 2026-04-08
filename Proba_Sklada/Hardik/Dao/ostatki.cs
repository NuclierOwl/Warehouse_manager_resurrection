using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class ostatki
{
    public int id { get; set; }

    public string inventory_number { get; set; } = null!;

    public DateOnly start_date { get; set; }

    public DateOnly? end_date { get; set; }

    public string? status { get; set; }

    public string? description { get; set; }

    public int created_by { get; set; }

    public DateTime? created_at { get; set; }

    public virtual user created_byNavigation { get; set; } = null!;

    public virtual ICollection<inventory_discrepancy> inventory_discrepancies { get; set; } = new List<inventory_discrepancy>();
}
