using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using CompanyEmployees.Presentation.ModelBinders;
using CompanyEmployees.Presentation.ActionFilters;
using System;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;

namespace CompanyEmployees.Presentation.Controllers;
//[ApiVersion("1.0")]
// we're basically creating a class and converting it into
// a controller by inheriting from the ControllerBase abstract class
[Route("api/companies")]
//[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = "v1")]
//[ResponseCache(CacheProfileName = "120SecondsDuration")]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager _service;
    public CompaniesController(IServiceManager service) => _service = service;

    /// <summary>
    /// Gets the list of all companies
    /// </summary>
    /// <returns>The companies list</returns>
    [HttpGet(Name = "GetCompanies")]
    [Authorize(Roles = "Manager")]
    // not changing the name of the method to GetCompaniesAsync because this is the name
    // the frontend will see when they call the method.
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await
             _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
        return Ok(companies);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    // overriding rule action > controller > global
    // this is for the Marvin.Cache.Header:
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
    //[HttpCacheValidation(MustRevalidate = false)]
    // this is generic:
    //[ResponseCache(Duration = 60)] //this overrides whatever duration has been set on the controller
    // the constraint: guid is to ensure the id is of a guid type
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var company = await _service.CompanyService.GetCompanyByIdAsync(id, trackChanges: false);
        return Ok(company);
    }
    //The route for this action is /api/companies/id and that’s because the
    ///api/companies part applies from the root route(on top of the
    //controller) and the id part is applied from the action attribute
    //[HttpGet(“{id:guid
    //    }“)].

    /// <summary>
    /// Creates a newly created company
    /// </summary>
    /// <param name="company"></param>
    /// <returns>A newly created company</returns>
    /// <response code="201">Returns the newly created item</response>
    /// <response code="400">If the item is null</response>
    /// <response code="422">If the model is invalid</response>
    // the action filter below covers the validation method validation generally
    [HttpPost(Name = "CreateCompany")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(422)]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
    {
        //if (company == null)
        //    // need to also consider for if the company name already exists.
        //    return BadRequest("CompanyForCreationDto object is null");
        var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
        return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType =
    typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    //Our ArrayModelBinder will be triggered before an action executes.It
    //will convert the sent string parameter to the IEnumerable<Guid> type,
    //and then the action will be executed:
    {
        var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
        return Ok(companies);
    }
    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection([FromBody]
    IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var result =
            await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
        return CreatedAtRoute("CompanyCollection", new { result.ids },
            result.companies);
    }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
        return NoContent();
    }
    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
    {
        //if (company == null)
        //    return BadRequest("CompanyForUpdateDto object is null");

        await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
        return NoContent();
    }
    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }
}
