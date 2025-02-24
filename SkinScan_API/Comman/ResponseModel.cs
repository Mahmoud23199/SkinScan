namespace SkinScan_API.Common
{
    public class ResponseModel<T>
    {
        public int StatusCode { get; set; }  // HTTP status code (e.g., 200, 400, 500)
        public bool Success { get; set; }  // Indicates if the request was successful
        public string Message { get; set; } // Success or error message
        public T Data { get; set; } // The actual data returned

        public ResponseModel(int statusCode, bool success, string message, T data)
        {
            StatusCode = statusCode;
            Success = success;
            Message = message;
            Data = data;
        }

        // Helper Methods
        public static ResponseModel<T> SuccessResponse(T data, string message = "Request successful", int statusCode = 200)
        {
            return new ResponseModel<T>(statusCode, true, message, data);
        }

        public static ResponseModel<T> ErrorResponse(string message, int statusCode = 400, T data = default)
        {
            return new ResponseModel<T>(statusCode, false, message, data);
        }
    }
}
