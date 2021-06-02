namespace vsWork.Data
{
    /// <summary>
    /// ユーザーモデル
    /// </summary>
    public class User : BaseEntity
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
