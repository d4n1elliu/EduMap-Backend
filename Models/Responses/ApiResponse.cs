namespace EduMap.Models.Responses;

public class ApiResponse<T>
{
    public string Message { get; set; }
    public T? Data { get; set; } // The response data corresponding to the request

    public ApiResponse(string message, T? data = default)
    {
        Message = message;
        Data = data;
    }
}

