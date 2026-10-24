namespace AdoptAPet.Service.Results
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; init; }

        public T? Data { get; init; }

        public string? ErrorKey { get; init; }

        public string? ErrorMessage { get; init; }

        public static ServiceResult<T> Success(T data)
            => new()
            {
                IsSuccess = true,
                Data = data
            };

        public static ServiceResult<T> Failure(string errorKey, string errorMessage)
            => new()
            {
                IsSuccess = false,
                ErrorKey = errorKey,
                ErrorMessage = errorMessage
            };
    }
}