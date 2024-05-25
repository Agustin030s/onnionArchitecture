using Application.DTOs;
using Application.Features.Customers.Commands.CreateCustomerCommand;
using Application.Features.Customers.Queries.GetCustomerById;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            #region Dtos
            // mapea de customer a customerdto
            CreateMap <Customer, CustomerDto>();
            #endregion
            #region Commands
            //esto me mapea lo del create command a la clase customer, es decir que las props tomen los mismos valores, mapear
            CreateMap<CreateCustomerCommand, Customer>();
            #endregion
        }
    }
}
