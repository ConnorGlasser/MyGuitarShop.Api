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
    public class OrdersEFCoreController(
        OrderRepository repository,
        ILogger<OrdersEFCoreController> logger
        ) : BaseController<OrderDTO, Order>(logger, repository)
    {   }
}
