using System;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace ead_backend.Utills
{
    public class CustomResponse
    {
        public bool IsSuccessful { get; set; }
        public string TimeStamp { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public CustomResponse(bool isSuccessful, string message, object data)
        {
            IsSuccessful = isSuccessful;
            TimeStamp = DateTime.UtcNow.ToString("o");
            Message = message;
            Data = data;
        }
    }

    public static class ResponseExtensions
    {
        public static IActionResult CustomResponse(this ControllerBase controller, bool isSuccessful, int statusCode, string message, object data)
        {
            var response = new CustomResponse(isSuccessful, message, data);
            return new ContentResult
            {
                Content = JsonConvert.SerializeObject(response),
                ContentType = "application/json",
                StatusCode = statusCode
            };
        }
    }
}