using Application.Wrappers;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomerCommand
{
    //Devuelvo un entero porque es el id que se va a crear cuando ejecutemos el comando
    public class CreateCustomerCommand : IRequest<Response<int>>
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Response<int>>
    {
        public Task<Response<int>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
