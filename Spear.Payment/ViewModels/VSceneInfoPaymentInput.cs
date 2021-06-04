using System.ComponentModel.DataAnnotations;

namespace Spear.Gateway.Payment.ViewModels
{
    public class VSceneInfoPaymentInput : VPaymentInput
    {
        /// <summary> 场景信息,该字段用于上报支付的场景信息,针对H5支付三种场景,请根据对应场景上报 </summary>
        [Required(ErrorMessage = "场景信息不能为空")]
        public string SceneInfo { get; set; }
    }
}
