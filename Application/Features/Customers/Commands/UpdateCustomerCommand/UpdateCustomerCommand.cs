using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomerCommand
{
    public class UpdateCustomerCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Response<int>>
    {
        private readonly IRepositoryAsync<Customer> _repositoryAsync;

        public UpdateCustomerCommandHandler(IRepositoryAsync<Customer> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<Response<int>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _repositoryAsync.GetByIdAsync(request.Id);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Registro no encontrado con el id: {request.Id}");
            }
            else
            {
                customer.Name = request.Name;
                customer.LastName = request.LastName;
                customer.Birthdate = request.Birthdate;
                customer.Phone = request.Phone;
                customer.Email = request.Email;
                customer.Address = request.Address;

                await _repositoryAsync.UpdateAsync(customer);

                return new Response<int>(customer.Id);
            }
        }
    }
}
