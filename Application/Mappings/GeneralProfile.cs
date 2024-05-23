using Application.Features.Customers.Commands.CreateCustomerCommand;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            #region Commands
            //esto me mapea lo del command a la clase customer, es decir que las props tomen los mismos valores, mapear
            CreateMap<CreateCustomerCommand, Customer>();
            #endregion
        }
    }
}
