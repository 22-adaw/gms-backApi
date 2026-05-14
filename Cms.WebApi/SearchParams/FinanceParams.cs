namespace Gms.WebApi.SearchParams
{
    public class FinanceParams:BaseParams
    {
        public int? FinanceType { get; set; }
        public string? TypeName { get; set; }
        public int? RelatedCode { get; set; }
    }
}
