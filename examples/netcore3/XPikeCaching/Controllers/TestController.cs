using System;
using System.Threading.Tasks;
using Example.Library;
using Microsoft.AspNetCore.Mvc;
using XPike.Caching;

namespace XPikeCaching.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController
        : ControllerBase
    {
        private readonly ICachingService _cachingService;

        public TestController(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }

        [HttpGet("")]
        public async Task<IActionResult> TestAsync()
        {
            var item = new RandomItem
            {
                Timestamp = DateTime.UtcNow,
                Something = "Something!"
            };

            await _cachingService.SetValueAsync(null, "item.123", item, TimeSpan.FromMinutes(15));

            var item2 = await _cachingService.GetItemAsync<RandomItem>(null, "item.123");

            return Ok(item2);
        }
    }
}