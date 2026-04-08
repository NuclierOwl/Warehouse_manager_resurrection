using System;
using System.Collections.Generic;

namespace Proba_Sklada.Hardik.Dao;

public partial class schet_faktura_soderzanie
{
    public int id { get; set; }

    public int invoice_id { get; set; }

    public int product_id { get; set; }

    public int quantity { get; set; }

    public decimal unit_price { get; set; }

    public decimal? total_price { get; set; }

    public int? picked_quantity { get; set; }

    public DateTime? picked_at { get; set; }

    public int? picked_by { get; set; }

    public virtual schet_faktura invoice { get; set; } = null!;

    public virtual user? picked_byNavigation { get; set; }

    public virtual product product { get; set; } = null!;
}
