using Microsoft.AspNetCore.Mvc;

namespace RabbitSharp.ExceptionMapper.Test.AspNetCore.App
{
    public class TestController : Controller
    {
        [Route("{a:int}", Name = "abc")]
        public IActionResult Test1()
        {
            return Ok();
        }
    }
}
