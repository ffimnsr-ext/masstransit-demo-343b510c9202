using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MassTransit;
using TokenContracts;
using TokenGatewayApi.ViewModels;

namespace TokenGatewayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public sealed class TokenController : ControllerBase
    {
        private readonly IRequestClient<SubmitToken> _submitTokenRequestClient;

        public TokenController(IRequestClient<SubmitToken> submitTokenRequestClient)
        {
            _submitTokenRequestClient = submitTokenRequestClient;
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate([FromQuery] TokenSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (accepted, rejected) = await _submitTokenRequestClient.GetResponse<TokenAccepted, TokenRejected>(new
            {
                EventId = NewId.NextGuid(),
                InVar.Timestamp,
                model.Token
            });

            if (accepted.IsCompletedSuccessfully)
            {
                var response = await accepted;
                return Accepted(response);
            }

            if (accepted.IsCompleted)
            {
                await accepted;
                return Problem("Token was not accepted");
            }
            else
            {
                var response = await rejected;
                return BadRequest(response.Message);
            }
        }
    }
}
