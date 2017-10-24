using System.ComponentModel.DataAnnotations;

namespace DocWorks.CMS.Api.Model.Request
{
    public class DeviceUnRegisterRequest 
    {
        [Required]
        public string FcmId { get; set; }
    }
}
