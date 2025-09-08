using FitBridge_API.Helpers;
using FitBridge_API.Helpers.RequestHelpers;
using FitBridge_Application.Dtos;
using FitBridge_Application.Features.Sample;
using FitBridge_Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Searching.Query;

namespace FitBridge_API.Controllers
{
    public class WeatherForecastController(IMediator mediator) : BaseApiController
    {
        // flow: query paging -> PagingResultDto -> Pagination<T> -> BaseResponse<T>
        // query 1 item -> ItemResponse -> BaseResponse<T>
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("WeatherForecast")]
        public ActionResult<WeatherForecast> Get()
        {
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

            return StatusCode(
                StatusCodes.Status200OK,
                new BaseResponse<WeatherForecast>(
                    StatusCodes.Status200OK.ToString(),
                    "Weather is good",
                    result[0]
                ));
        }

        [HttpGet("WeatherForecastWithPaging")]
        public ActionResult<Pagination<WeatherForecast>> GetPaging()
        {
            var result = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            var pagingResult = new PagingResultDto<WeatherForecast>(result.Length, [.. result]);

            return StatusCode(
                StatusCodes.Status200OK,
                new BaseResponse<Pagination<WeatherForecast>>(
                    StatusCodes.Status200OK.ToString(),
                    "Weather is good",
                    ResultWithPagination(pagingResult.Items, pagingResult.Total, 1, 5)
                ));
        }

        [HttpPost("CreateWeather")]
        public async Task<IActionResult> CreateWeather()
        {
            var command = new SampleCommand1
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "TestPassword123!"
            };

            await mediator.Send(command);

            return StatusCode(
                StatusCodes.Status201Created,
                new BaseResponse<object>(
                    StatusCodes.Status201Created.ToString(),
                    "User created successfully",
                    null
                ));
        }
    }
}