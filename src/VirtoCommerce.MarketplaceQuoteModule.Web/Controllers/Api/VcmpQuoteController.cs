using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models;
using VirtoCommerce.MarketplaceQuoteModule.Core.Models.Conditions;
using VirtoCommerce.MarketplaceQuoteModule.Data.Commands;
using VirtoCommerce.MarketplaceQuoteModule.Data.Queries;
using VirtoCommerce.MarketplaceVendorModule.Core.Common;
using VirtoCommerce.MarketplaceVendorModule.Data.Authorization;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.QuoteModule.Core.Models;
using Permissions = VirtoCommerce.QuoteModule.Core.ModuleConstants.Security.Permissions;

namespace VirtoCommerce.MarketplaceQuoteModule.Web.Controllers.Api;

[Authorize]
[Route("api/vcmp/quote")]
public class VcmpQuoteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAuthorizationService _authorizationService;

    public VcmpQuoteController(
    IMediator mediator,
    IAuthorizationService authorizationService
    )
    {
        _mediator = mediator;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<QuoteRequestSearchResult>> Search([FromBody] SearchQuoteRequestsQuery query)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, query, new SellerAuthorizationRequirement(Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [Route("getbyid")]
    public async Task<ActionResult<VcmpQuoteRequest>> GetById([FromQuery] string id)
    {
        var query = AbstractTypeFactory<GetQuoteRequestQuery>.TryCreateInstance();
        query.Id = id;

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, query, new SellerAuthorizationRequirement(Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost]
    [Route("calculatetotals")]
    public async Task<ActionResult<QuoteRequest>> CalculateTotals([FromBody] QuoteRequest quoteRequest)
    {
        var command = ExType<CalculateTotalsCommand>.New();
        command.QuoteRequest = quoteRequest;

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, command, new SellerAuthorizationRequirement(Permissions.Read));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost]
    [Route("update")]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update([FromBody] QuoteRequest quoteRequest)
    {
        var command = ExType<UpdateQuoteRequestCommand>.New();
        command.QuoteRequest = quoteRequest;

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, command, new SellerAuthorizationRequirement(Permissions.Update));
        if (!authorizationResult.Succeeded)
        {
            return Forbid();
        }
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpGet]
    [Route("condition/prototype")]
    public ActionResult<QuoteRequestCondition> GetConditionPrototype()
    {
        var query = ExType<GetQuoteRequestConditionPrototypeQuery>.New();
        var result = _mediator.Send(query);

        return Ok(result);
    }
}
