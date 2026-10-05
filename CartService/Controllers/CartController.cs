using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {

    }
}
