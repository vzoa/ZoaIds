using System.Text.Json.Serialization;

namespace ZoaIdsBackend.Modules.Charts.Models;

public class Chart
{
    public string AirportName { get; set; }
    public string FaaIdent { get; set; }
    public string IcaoIdent { get; set; }
    public string ChartSeq { get; set; }
    public string ChartCode { get; set; }
    public string ChartName { get; set; }
    public ICollection<ChartPage> Pages { get; set;}

}

public class ChartPage
{
    public int PageNumber { get; set; }
    public string PdfName { get; set; }
    public string PdfPath { get; set; }
}
