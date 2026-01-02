using Grpc.Core;

namespace BoilerPlait.InventoryService.Api.Adapters.Grpc
{
    public class InventoryService : InventoryServices.InventoryServicesBase
    {
        public override Task<GetListPartsResponse> GetListParts(GetListPartsRequest request, ServerCallContext context)
        {
            return base.GetListParts(request, context);
        }

        public override Task<GetPartByIdResponse> GetPart(GetPartByIdRequest request, ServerCallContext context)
        {
            return base.GetPart(request, context);
        }
    }
}