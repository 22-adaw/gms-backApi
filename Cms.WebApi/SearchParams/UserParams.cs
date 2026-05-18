namespace Gms.WebApi.SearchParams
{
    public class UserParams:BaseParams
    {
        public string? UserName { get; set; }
        public string? RealName { get; set; }
        public string? UserPhone { get; set; }
        public string? DepartmentName { get; set; }
        public string? UserEmail { get; set; }
    }
}
