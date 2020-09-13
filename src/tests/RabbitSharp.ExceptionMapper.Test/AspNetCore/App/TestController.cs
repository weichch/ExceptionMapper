using System;
using Microsoft.AspNetCore.Mvc;
using RabbitSharp.Diagnostics.AspNetCore;

namespace RabbitSharp.ExceptionMapper.Test.AspNetCore.App
{
    [Route("test/controller")]
    public class TestController : Controller
    {
        [Route("{value}/{message}")]
        [MapException("my-tag")]
        [MapException("my-tag3")]
        public IActionResult GetTest(string message)
        {
            throw new InvalidOperationException(message);
        }

        [Route("problem")]
        [MapException("problem-details")]
        public IActionResult ProblemDetailsTest()
        {
            throw new InvalidOperationException();
        }
    }
}
