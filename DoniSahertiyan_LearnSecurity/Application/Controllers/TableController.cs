using Core.Features.Queries.GetTableSpecifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Core.Features.Commands.CreateTableSpecification;
using Core.Features.Commands.DeleteTableSpecification;

namespace Application.Controllers;

public class TableController : BaseController
{
    private readonly IMediator _mediator;

    public TableController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("v1/table/specification/{id}")]
    public async Task<GetTableSpecificationsResponse> GetTableSpecifications(Guid id)
    {
        var request = new GetTableSpecificationsQuery()
        {
            TableSpecificationId = id
        };
        var response = await _mediator.Send(request);
        return response;
    }
    [HttpPost("v1/table/specification")]
    public async Task<CreateTableSpecificationResponse> CreateTableSpecification([FromBody] CreateTableSpecificationCommand command)
    {
        var response = await _mediator.Send(command);
        return response;
    }
    [HttpDelete("v1/table/specification/{id}")]
    public async Task<DeleteTableSpecificationResponse> DeleteTableSpecification(Guid id)
    {
        var command = new DeleteTableSpecificationCommand
        {
            TableId = id
        };
        var response = await _mediator.Send(command);
        return response;
    }
}