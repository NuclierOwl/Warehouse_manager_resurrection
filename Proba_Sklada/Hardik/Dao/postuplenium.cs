using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class postuplenium
{
    public int id { get; set; }

    public string invoice_number { get; set; } = null!;

    public int supplier_id { get; set; }

    public DateOnly invoice_date { get; set; }

    public DateOnly? arrival_date { get; set; }

    public decimal? total_amount { get; set; }

    public string? status { get; set; }

    public int created_by { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual user created_byNavigation { get; set; } = null!;

    public virtual ICollection<postuplenia_item> postuplenia_items { get; set; } = new List<postuplenia_item>();

    public virtual counter_ogent supplier { get; set; } = null!;
}
