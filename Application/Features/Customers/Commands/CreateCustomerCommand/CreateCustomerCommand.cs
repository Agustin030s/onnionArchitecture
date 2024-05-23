using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomerCommand
{
    //Devuelvo un entero porque es el id que se va a crear cuando ejecutemos el comando
    public class CreateCustomerCommand : IRequest<Response<int>>
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryAsync;
        private readonly IMapper _mapper;

        public CreateCustomerCommandHandler(IRepositoryAsync<Customer> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<Response<int>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            //mapea el request a la clase customer
            var nuevoRegistro = _mapper.Map<Customer>(request);

            //guarda el nuevo registro
            var data = await _repositoryAsync.AddAsync(nuevoRegistro, cancellationToken);

            //retorna el id
            return new Response<int>(data.Id);
        }
    }
}
