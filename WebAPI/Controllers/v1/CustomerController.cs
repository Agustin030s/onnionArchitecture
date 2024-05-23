using Application.Features.Customers.Commands.CreateCustomerCommand;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    public class CustomerController : BaseApiController
    {
        //POST api/<controller>
        [HttpPost]
        public async Task <IActionResult> Post(CreateCustomerCommand command)
        {
            //cuando se ejecute el send se va ir al CreateCustomerCommandHandler y ejecutara lo que esta ahi. 
            return Ok(await Mediator.Send(command));
        }
    }
}
