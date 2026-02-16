namespace UCourses_Back_End.Core.ModelsView
{
    public class ApiResponse<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Success(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> Fail(string error)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                Errors = new List<string> { error }
            };
        }

        public static ApiResponse<T> Fail(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                Errors = errors
            };
        }

        public static ApiResponse<T> Fail(IEnumerable<string> errors)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }
    }

    // For responses without data
    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse Success(string? message = null)
        {
            return new ApiResponse
            {
                Succeeded = true,
                Message = message
            };
        }

        public new static ApiResponse Fail(string error)
        {
            return new ApiResponse
            {
                Succeeded = false,
                Errors = new List<string> { error }
            };
        }

        public new static ApiResponse Fail(List<string> errors)
        {
            return new ApiResponse
            {
                Succeeded = false,
                Errors = errors
            };
        }
    }
}
