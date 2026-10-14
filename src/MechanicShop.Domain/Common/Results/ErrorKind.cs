namespace MechanicShop.Domain.Common.Results;

/// <summary>
/// Represents the kind of error that may occur.
/// </summary>
public enum ErrorKind
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
    Forbidden,
}