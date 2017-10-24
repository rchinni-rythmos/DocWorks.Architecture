using System.ComponentModel.DataAnnotations;

namespace DocWorks.CMS.Api.Model.Request
{
    public class DeviceRegisterRequest 
    {
        [Required]
        public string FcmId { get; set; }
    }
}
