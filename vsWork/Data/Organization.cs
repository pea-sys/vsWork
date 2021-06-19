using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vsWork.Data
{
    /// <summary>
    /// 組織モデル
    /// </summary>
    public class Organization : BaseEntity
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        
    }
}
