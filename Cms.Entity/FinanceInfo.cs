using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gms.Entity
{
    public class FinanceInfo : BaseEntity<int>
    {
        /// <summary>
        /// 财务类型（0-收入，1-支出） 
        /// </summary>
        public int? FinanceType { get; set; }
        /// <summary>
        /// 具体类型（如"办卡收入""工资支出"）
        /// </summary>
        public string? TypeName { get; set; }  
        /// <summary>
        /// 金额
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// 财务编码
        /// </summary>
        public string? FinanceCode { get; set; }
        /// <summary>
        /// 关联Id
        /// </summary>
        public int? RelatedCode { get; set; }
        /// <summary>
        /// 关联类型（如关联会员卡收入则为vip，关联课程收入则为course，其他则为other）    
        /// </summary>
        public string? RelatedType { get; set; }     
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
