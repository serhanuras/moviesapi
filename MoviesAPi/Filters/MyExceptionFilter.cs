
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;


namespace MoviesAPi.Filters
{

    public class MyExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logging;

        public MyExceptionFilter(ILogger<MyExceptionFilter> logger)
        {
            this._logging = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logging.LogError(context.Exception, context.Exception.Message);



            context.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            // context.Result = new JsonResult(new
            // {
            //     error = "There is a problem on server side. Please try again later..."
            // });

            context.Result = new ObjectResult(new ErrorResult()
            {
                Description = "There is a problem on server side. Please try again later..."

            });

            context.ExceptionHandled = true;
            base.OnException(context);
        }


    }

    [DataContract]
    class ErrorResult
    {
        public ErrorResult()
        {

        }
        
        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }
    }
}