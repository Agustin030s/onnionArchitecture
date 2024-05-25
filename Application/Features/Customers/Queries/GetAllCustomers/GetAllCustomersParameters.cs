using Application.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersParameters : RequestParameter
    {
        public string? Name { get; set; }
        public string? Lastname { get; set; }
    }
}
