using BoilerPlait.InventoryService.Core.Application.UseCases.Queries.GetPart;
using BoilerPlait.InventoryService.Core.Application.UseCases.Queries.SharedKernelDto;
using CSharpFunctionalExtensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Primitives;

namespace BoilerPlait.InventoryService.Api.Adapters.Grpc
{
    public class InventoryService : InventoryServices.InventoryServicesBase
    {
        private readonly IMediator _mediator;

        public InventoryService(IMediator mediator)
        {

        }
        public override async Task<GetListPartsResponse> GetListParts(GetListPartsRequest request, ServerCallContext context)
        {
            return null;
        }

        public override async Task<GetPartByIdResponse> GetPart(GetPartByIdRequest request, ServerCallContext context)
        {
            var resultGetPart =
                await _mediator.Send(new GetPartQuery(request.Uuid), context.CancellationToken);

            if (resultGetPart.IsFailure)
            {
                if (resultGetPart.Error == GeneralErrors.NotFound())
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "Part Not Found"));
                }
                if (resultGetPart.Error == GeneralErrors.ValueIsInvalid(request.Uuid))
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Not valid request ID{request.Uuid}"));
                }
            }
            var part = resultGetPart.Value.part;

            var response = Mapto(part);

            return response;
        }
        public GetPartByIdResponse Mapto(PartDto response)
        {
            var part = new Part
            {
                Uuid = response.Id,
                Name = response.Name,
                Description = response.Description,
                Price = response.Price,
                StockQuantity = response.StockQuantity,
                Category = ParseCategory(response.Category),
                Dimensions = new Dimensions
                {
                    Length = response.Dimensions.Length,
                    Width = response.Dimensions.Width,
                    Height = response.Dimensions.Height,
                    Weight = response.Dimensions.Weight
                },
                Manufacturer = new Manufacturer
                {
                    Name = response.Manufacturer.Name,
                    Country = response.Manufacturer.Country,
                    Website = response.Manufacturer.Website
                },
                Tags = { response.Tags.Select(t => t) },
                CreatedAt = Timestamp.FromDateTime(response.CreatedAt),
            };
            if (part.UpdatedAt != null)
            {
                part.UpdatedAt = Timestamp.FromDateTime(response.UpdatedAat!.Value.ToUniversalTime());
            }
            return new GetPartByIdResponse
            {
                Part = part
            };
        }

        private static Category ParseCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                throw new RpcException(
                          new Status(StatusCode.InvalidArgument, "category is empty")
                      );
            }
            return categoryName.Trim().ToUpperInvariant() switch
            {
                "UNKNOWN" => Category.Unknown,
                "ENGINE" => Category.Engine,
                "FUEL" => Category.Fuel,
                "PORTHOLE" => Category.Porthole,
                "WING" => Category.Wing,
                _ => throw new RpcException(
                                  new Status(StatusCode.InvalidArgument, $"category unknown{categoryName}")
                              )
            };
        }
    }
}