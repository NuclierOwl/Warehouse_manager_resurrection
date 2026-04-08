using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class schet_faktura
{
    public int id { get; set; }

    public string invoice_number { get; set; } = null!;

    public int customer_id { get; set; }

    public DateOnly invoice_date { get; set; }

    public DateOnly? shipment_date { get; set; }

    public decimal? total_amount { get; set; }

    public string? status { get; set; }

    public int created_by { get; set; }

    public DateTime? created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual user created_byNavigation { get; set; } = null!;

    public virtual counter_ogent customer { get; set; } = null!;

    public virtual ICollection<schet_faktura_soderzanie> schet_faktura_soderzanies { get; set; } = new List<schet_faktura_soderzanie>();
}
