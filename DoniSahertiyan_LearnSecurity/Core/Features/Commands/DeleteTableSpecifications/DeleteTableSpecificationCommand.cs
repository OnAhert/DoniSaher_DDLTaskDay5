using MediatR;

namespace Core.Features.Commands.DeleteTableSpecification
{
    public class DeleteTableSpecificationCommand : IRequest<DeleteTableSpecificationResponse>
    {
        public Guid TableId { get; set; }
    }
}
