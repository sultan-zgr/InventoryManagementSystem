using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;


namespace InventoryManagementSystem.Tests.MiddlewareTests
{
    public class ExceptionMiddlewareTests
    {
        [Fact]
        public async Task Middleware_Should_Return_404_For_KeyNotFoundException()
        {
            // Arrange
            var middleware = new ExceptionMiddleware(async (innerHttpContext) =>
            {
                throw new KeyNotFoundException("Resource not found.");
            });

            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();

            var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            response.Message.Should().Be("Resource not found.");
        }

        [Fact]
        public async Task Middleware_Should_Return_400_For_ArgumentException()
        {
            // Arrange
            var middleware = new ExceptionMiddleware(async (innerHttpContext) =>
            {
                throw new ArgumentException("Invalid argument.");
            });

            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();

            var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            response.Message.Should().Be("Invalid argument.");
        }

        [Fact]
        public async Task Middleware_Should_Return_500_For_UnexpectedException()
        {
            // Arrange
            var middleware = new ExceptionMiddleware(async (innerHttpContext) =>
            {
                throw new Exception("Unexpected error.");
            });

            var context = new DefaultHttpContext();
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();

            var response = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Message.Should().Be("An unexpected error occurred. Please try again later.");
        }
        private class ErrorResponse
        {
            public int StatusCode { get; set; }
            public string Message { get; set; }
        }
    }


}
