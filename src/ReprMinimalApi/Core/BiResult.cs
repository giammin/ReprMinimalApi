namespace ReprMinimalApi.Core;

public readonly struct BiResult<TSuccess, TFail>
{
    private readonly TFail? _failResponse;
    private readonly TSuccess? _successResponse;

    private BiResult(TSuccess successResponse)
    {
        _successResponse = successResponse;
        _failResponse = default;
    }

    private BiResult(TFail failResponse)
    {
        _failResponse = failResponse;
        _successResponse = default;
    }

    public bool IsSuccess => _failResponse == null;
    public bool IsFailed => _failResponse != null;

    public TSuccess GetSuccessResult()
    {
        if (IsFailed)
        {
            throw new Exception("result failed");
        }

        if (_successResponse == null)
        {
            throw new Exception("result is null");
        }
        return _successResponse;
    }
    public TSuccess? GetSuccessResultOrDefault()
    {
        if (IsFailed)
        {
            throw new Exception("result failed");
        }
        return _successResponse;
    }
    public TFail GetFailResult()
    {
        if (_failResponse == null)
        {
            throw new Exception("result succeded");
        }
        return _failResponse;
    }

    public BiResult<TB, TFail> Map<TB>(Func<TSuccess, TB> mapFunc) =>
        !IsSuccess
            ? new BiResult<TB, TFail>(GetFailResult())
            : new BiResult<TB, TFail>(mapFunc(GetSuccessResult()));

    /// <summary>
    /// ritorna il success result o esegue la funzione passata per generare il successresult
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public TSuccess IfFail(Func<TFail, TSuccess> f) =>
        IsSuccess
            ? GetSuccessResult()
            : f(GetFailResult());


    public void IfSuccess(Action<TSuccess> f)
    {
        if (IsSuccess) f(GetSuccessResult());
    }
    public TMap? IfSuccessMap<TMap>(Func<TSuccess, TMap> f)
    {
        if (IsSuccess) return f(GetSuccessResult());
        return default;
    }
    public void IfFail(Action<TFail> f)
    {
        if (!IsSuccess) f(GetFailResult());
    }

    public TR Match<TR>(Func<TSuccess, TR> success, Func<TFail, TR> fail) =>
        IsSuccess
            ? success(GetSuccessResult())
            : fail(GetFailResult());

    public static implicit operator BiResult<TSuccess, TFail>(TSuccess success) => new(success);
    public static implicit operator BiResult<TSuccess, TFail>(TFail fail) => new(fail);
}