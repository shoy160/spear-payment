using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Spear.Payment.Alipay.Domain
{
    /// <summary>
    /// 外部指定买家
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class ExtUserInfo
    {
        /// <summary>
        /// 证件号
        /// 注：need_check_info=T时该参数才有效
        /// </summary>
        public string CertNo { get; set; }

        /// <summary>
        /// 身份证：IDENTITY_CARD、护照：PASSPORT、军官证：OFFICER_CARD、士兵证：SOLDIER_CARD、户口本：HOKOU等。如有其它类型需要支持，请与蚂蚁金服工作人员联系。
        /// 注： need_check_info=T时该参数才有效
        /// </summary>
        public string CertType { get; set; }

        /// <summary>
        /// 是否强制校验付款人身份信息
        /// T:强制校验，F：不强制
        /// </summary>
        public string FixBuyer { get; set; }

        /// <summary>
        /// 允许的最小买家年龄，买家年龄必须大于等于所传数值
        /// 注：1. need_check_info=T时该参数才有效  2. min_age为整数，必须大于等于0
        /// </summary>
        public string MinAge { get; set; }

        /// <summary>
        /// 手机号
        /// 注：该参数暂不校验
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 姓名
        /// 注：need_check_info=T时该参数才有效
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否强制校验身份信息
        /// T:强制校验，F：不强制
        /// </summary>
        public string NeedCheckInfo { get; set; }
    }
}
