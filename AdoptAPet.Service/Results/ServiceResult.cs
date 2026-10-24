
namespace AdoptAPet.Service.Results
{
    public class ServiceResult
    {
        public bool IsSuccess { get; init; }

        public string? ErrorKey { get; init; }

        public string? ErrorMessage { get; init; }

        public static ServiceResult Success()
            => new() { IsSuccess = true };

        public static ServiceResult Failure(string errorKey, string errorMessage)
            => new()
            {
                IsSuccess = false,
                ErrorKey = errorKey,
                ErrorMessage = errorMessage
            };
    }
}
