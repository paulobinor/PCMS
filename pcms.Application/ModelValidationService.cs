using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using FluentValidation;

namespace pcms.Application.Validation
{
    public class ModelValidationService
    {
        private static IServiceProvider _serviceProvider;

        public ModelValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ValidationResultModel Validate<TModel>(TModel model)
        {
            ValidationResultModel validationResultModelDto = new ValidationResultModel();
            var validator = _serviceProvider.GetService<IValidator<TModel>>();
            if (validator == null)
            {
                throw new InvalidOperationException($"No validator found for {typeof(TModel).Name}");
            }

            var res = validator.Validate(model);
            if (!res.IsValid)
            {
                string message = string.Empty;

                foreach (var item in res.Errors)
                {
                    message += string.Join(" | ", $"{item.PropertyName}: {item.ErrorMessage}, ");
                }

                validationResultModelDto.IsValid = false;
                validationResultModelDto.customProblemDetail = new ProblemDetails { Detail = message, Status = (int)HttpStatusCode.BadRequest, Title = "Validation Errors" };

                return validationResultModelDto;
            }
            return validationResultModelDto;
        }

        public static T GetServiceProvider<T>(T model) where T : class
        {
            var sp = _serviceProvider.GetService<T>();
            if (sp == null)
            {
                throw new InvalidOperationException($"No service provider found for {typeof(T).Name}");
            }
            return sp;
        }

        
    }

    
}
