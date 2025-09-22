﻿namespace ParcelTracking.Api.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }


        private ApiResponse(bool  success, string message, T data = default!)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>(true, message, data);
        }

        public static ApiResponse<T> FailResponse(string message)
        {
            return new ApiResponse<T>(false, message);
        }

    }
}
