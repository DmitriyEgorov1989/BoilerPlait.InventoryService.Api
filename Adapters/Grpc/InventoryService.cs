using BoilerPlait.InventoryService.Core.Application.UseCases.Queries.GetListParts;
using BoilerPlait.InventoryService.Core.Application.UseCases.Queries.GetPart;
using BoilerPlait.InventoryService.Core.Application.UseCases.Queries.SharedKernelDto;
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
            _mediator = mediator;
        }
        public override async Task<GetListPartsResponse> GetListParts(GetListPartsRequest request, ServerCallContext context)
        {
            if (request == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Not valid request"));
            }
;
            var query = new GetListPartsQuery(
                new PartsFilterModel(
                    request.Filter.Uuids.Select(i => i).ToList(),
                   request.Filter.Names.Select(n => n).ToList(),
                    request.Filter.Categories.Select(c => c.ToString()).ToList(),
                    request.Filter.ManufacturerCountries.Select(c => c).ToList(),
                    request.Filter.Tags.Select(t => t).ToList())
                );

            var getListParts = await _mediator.Send(query);

            if (getListParts.Error == GeneralErrors.NotFound())
                throw new RpcException(new Status(StatusCode.NotFound, "Parts with such filters were not found "));

            var listParts = getListParts.Value;

            var response = new GetListPartsResponse
            {
                Parts = { listParts.Parts.Select(Mapto) }
            };

            return response;
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
            var partDto = resultGetPart.Value.part;

            var part = Mapto(partDto);

            return new GetPartByIdResponse
            {
                Part = part
            };
        }

        public Part Mapto(PartDto response)
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
            return part;
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