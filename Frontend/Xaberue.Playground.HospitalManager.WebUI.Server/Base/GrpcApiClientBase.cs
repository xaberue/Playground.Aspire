using Grpc.Core;
using Grpc.Net.Client;

namespace Xaberue.Playground.HospitalManager.WebUI.Server.Base;

public abstract class GrpcApiClientBase : IDisposable
{

    private readonly string _apiUrl;
    private readonly string _apiKey;
    private readonly GrpcChannel _grpcChannel;
    private readonly Metadata _headers = new Metadata();

    protected GrpcChannel GrpcChannel => _grpcChannel;
    protected Metadata Headers => _headers;


    protected GrpcApiClientBase(string apiUrl, string apiKey)
    {
        _apiUrl = apiUrl;
        _apiKey = apiKey;

        _grpcChannel = GrpcChannel.ForAddress(_apiUrl);

        _headers.Add("X-ApiKey", _apiKey);
    }


    public void Dispose()
    {
        _grpcChannel.Dispose();
    }
}
