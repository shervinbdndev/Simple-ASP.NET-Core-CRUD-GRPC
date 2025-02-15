using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcSample.Interceptors {

    public class ExceptionInterceptor : Interceptor {

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try {

                return await continuation(request, context);
            }
            catch (Exception ex) {

                var correlationId = Guid.NewGuid();

                Metadata trailers = new();

                trailers.Add("CorrelationId", correlationId.ToString());

                throw new RpcException(new Status(StatusCode.Internal, ex.Message), trailers, "Serverside Exception");
            }
        }
    }
}