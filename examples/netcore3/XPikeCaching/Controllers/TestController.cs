using System;
using System.Threading.Tasks;
using Example.Library;
using Microsoft.AspNetCore.Mvc;
using XPike.Caching;
using XPike.Configuration;

namespace XPikeCaching.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController
        : ControllerBase
    {
        private readonly ICachingService _cachingService;
        private readonly IConfig<TestConfig> _config;

        public TestController(ICachingService cachingService, IConfig<TestConfig> config)
        {
            _cachingService = cachingService;
            _config = config;
        }

        [HttpGet("")]
        public async Task<IActionResult> TestAsync()
        {
            var config = await _config.GetLatestValueAsync();

            var item = new RandomItem
            {
                Timestamp = DateTime.UtcNow,
                Something = "Something!",
                ConfigValue = config.TestValue,
                ConfigTimestamp = config.Loaded
            };

            await _cachingService.SetValueAsync(null, "item.123", item, TimeSpan.FromMinutes(15));

            var item2 = await _cachingService.GetItemAsync<RandomItem>(null, "item.123");

            return Ok(item2);
        }

        [HttpGet("retrieve")]
        public async Task<IActionResult> RetrieveAsync()
        {
            var item2 = await _cachingService.GetItemAsync<RandomItem>(null, "item.123");

            return Ok(item2);
        }
    }
}