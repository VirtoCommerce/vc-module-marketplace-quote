using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Permissions = VirtoCommerce.MarketplaceQuoteModule.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.MarketplaceQuoteModule.Web.Controllers.Api;

[Authorize]
[Route("api/marketplace-quote-module")]
public class MarketplaceQuoteModuleController : Controller
{
    // GET: api/marketplace-quote-module
    /// <summary>
    /// Get message
    /// </summary>
    /// <remarks>Return "Hello world!" message</remarks>
    [HttpGet]
    [Route("")]
    [Authorize(/*Permissions.Read*/)]
    public ActionResult<string> Get()
    {
        return Ok(new { result = "Hello world!" });
    }
}
