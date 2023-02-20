using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.Web.Controllers.Base;

public abstract class BaseController : Controller
{
    protected string UserId => User.Claims.Where(user => user.Type == "sub")?.FirstOrDefault()?.Value;
}