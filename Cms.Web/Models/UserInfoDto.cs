namespace Cms.Web.Models
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public int Gender { get; set; }
        /// <summary>
        /// 用户头像，存储头像路径
        /// </summary>
        public string? PhotoUrl { get; set; }
    }
}
