using HR.Models;

namespace HR.Utilities
{
    public class ServiceResponse<T>
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public static ServiceResponse<T> Fail(string message)=>
            new ServiceResponse<T> { Success = false, Message = message };

        public static ServiceResponse<T> Ok(T data, string? message = null) =>
            new ServiceResponse<T> { Success = true, Data = data, Message = message ?? string.Empty };

        
    }
}
