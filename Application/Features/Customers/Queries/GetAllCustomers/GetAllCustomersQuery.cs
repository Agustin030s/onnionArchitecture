using Application.DTOs;
using Application.Interfaces;
using Application.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : GetAllCustomersParameters ,IRequest<PagedResponse<List<CustomerDto>>>
    {
        //public int PageNumber { get; set; }
        //public int PageSize { get; set; }
        //public string? Name { get; set; }
        //public string? Lastname { get; set; }

        public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, PagedResponse<List<CustomerDto>>>
        {
            private readonly IRepositoryAsync<Customer> _repositoryAsync;
            private readonly IMapper _mapper;
            private readonly IDistributedCache _distributedCache;

            public GetAllCustomersQueryHandler(IRepositoryAsync<Customer> repositoryAsync, IMapper mapper, IDistributedCache distributedCache)
            {
                _repositoryAsync = repositoryAsync;
                _mapper = mapper;
                _distributedCache = distributedCache;
            }

            public async Task<PagedResponse<List<CustomerDto>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
            {
                string cacheKey = $"customersList_{request.PageNumber}_{request.PageSize}_{request.Name}_{request.Lastname}";
                string serializedCustomersList;
                var customersList = new List<Customer>();

                var redisCustomersList = await _distributedCache.GetAsync(cacheKey);
                if(redisCustomersList != null)
                {
                    serializedCustomersList = Encoding.UTF8.GetString(redisCustomersList);
                    customersList = JsonConvert.DeserializeObject<List<Customer>>(serializedCustomersList);
                }
                else
                {
                    customersList = await _repositoryAsync.ListAsync(new PagedCustomersSpecification(request.PageNumber, request.PageSize, request.Name, request.Lastname));
                    serializedCustomersList = JsonConvert.SerializeObject(customersList);
                    redisCustomersList = Encoding.UTF8.GetBytes(serializedCustomersList);

                    var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                    await _distributedCache.SetAsync(cacheKey, redisCustomersList, options);
                }

                var customerDto = _mapper.Map<List<CustomerDto>>(customersList);

                return new PagedResponse<List<CustomerDto>>(customerDto, request.PageNumber, request.PageSize);
            }
        }
    }
}
