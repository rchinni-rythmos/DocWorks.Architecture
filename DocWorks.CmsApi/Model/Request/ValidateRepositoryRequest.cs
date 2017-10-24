using System.ComponentModel.DataAnnotations;

namespace DocWorks.CMS.Api.Model.Request
{
    public class ValidateRepositoryRequest
    {
        [Required]
        public string RepositoryUrl { get; set; }
    }
}
