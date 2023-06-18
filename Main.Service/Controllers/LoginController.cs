using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nvenger.Common.BaseModel;
using Serilog;

namespace Main.Service.Controllers;

[ApiController]
[Route("v1/login/")]
public class LoginController : BaseApiController
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("accesstoken/get")]
    public async Task<ActionResult<ModelResponse>>  AccessTokenGet()
    {
        try
        {
                
            /*userDepositCreate.xCorrelationID = Request.Headers["X-Correlation-ID"];
            var resp = await _mediator.Send(request: new UsersDepositCreateCommand
            {
                userDepositCreate = _mapper.Map<UserDepositCreate>(userDepositCreate),
                modelResponse =  null
            });*/

            return ResponseModel(null);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return BadRequest();
        }
    }
}