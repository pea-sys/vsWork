using vsWork.Utils;

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
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public RankType Rank { get; set; }

        public enum RankType
        {
            [EnumText("なし")]
            None = 0,
            [EnumText("システム管理者")]
            SystemAdmin = 1,
            [EnumText("組織管理者")]
            OrganizationAdmin = 2,
            [EnumText("一般")]
            General = 3
        }
    }
}
