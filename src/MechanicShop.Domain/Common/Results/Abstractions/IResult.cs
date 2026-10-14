namespace MechanicShop.Domain.Common.Results.Abstractions;

public interface IResult
{
    List<Error>? Errors { get; }

    bool IsSuccess { get; }
}

/// <summary>
/// generic interface that represents the result of an operation that may have succeeded or failed, and if it succeeded,
/// contains a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value contained in the result.</typeparam>

public interface IResult<out TValue> : IResult
{
    TValue Value { get; }
}