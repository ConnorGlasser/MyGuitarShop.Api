using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.API.Abstract;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Data.EFCore.Entities;
using MyGuitarShop.Data.EFCore.Repositories;

namespace MyGuitarShop.API.Controllers.EFCoreControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersEFCoreController(
        CustomerRepository repository,
        ILogger<CustomersEFCoreController> logger
        ) : BaseController<CustomerDTO, Customer>(logger, repository)
    {   }
}
