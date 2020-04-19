using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MoviesAPi.Filters
{

    public class MyActionFilter:IActionFilter
    {
        private ILogger _logging;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            this._logging = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logging.LogWarning("OnActionExecuting");

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logging.LogWarning("OnActionExecuted");
        }

        
    }
}