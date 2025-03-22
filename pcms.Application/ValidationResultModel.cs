using Microsoft.AspNetCore.Mvc;

namespace pcms.Application
{
    public class ValidationResultModel
    {
        public bool IsValid { get; set; } = true;

        public ProblemDetails customProblemDetail { get; set; }
    }

    
}
