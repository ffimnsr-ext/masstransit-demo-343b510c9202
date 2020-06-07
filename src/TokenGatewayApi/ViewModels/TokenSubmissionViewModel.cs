using System.ComponentModel.DataAnnotations;

namespace TokenGatewayApi.ViewModels
{
    public sealed class TokenSubmissionViewModel
    {
        [Required]
        public string Token { get; set; }
    }
}
