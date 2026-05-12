using System;

namespace Proba_Sklada.Reports;

public enum ReportOperationFilter
{
    All,
    Incoming,
    Outgoing,
    InternalMovement
}

public sealed record ReportRow(
    DateTime DateTime,
    string Operation,
    string DocumentNumber,
    string Counterparty,
    string Product,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string LocationFrom,
    string LocationTo
);

