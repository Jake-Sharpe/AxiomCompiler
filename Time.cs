using System.ComponentModel;

namespace AxiomCompiler;

public enum Time
{
    [Description("1")]
    s,
    [Description("60")]
    m,
    [Description("0.001")]
    ms,
    [Description("3600")]
    h,
    [Description("86400")]
    d
}