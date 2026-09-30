namespace FormationGhApi.Services.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"Aucun handler enregistré pour la requête '{requestType.Name}'.");

        var handleMethod = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync))
            ?? throw new InvalidOperationException($"La méthode HandleAsync est introuvable sur le handler de '{requestType.Name}'.");

        var task = (Task<TResponse>)handleMethod.Invoke(handler, new object[] { request, cancellationToken })!;
        return await task;
    }
}
