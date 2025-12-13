using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FitHubBackendAPI.Filters
{
    public class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // nothing
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;

            // اجمع كل الأخطاء في رسالة واحدة (تقدر تعدل الشكل لو تحب)
            var errors = context.ModelState
                .Where(kvp => kvp.Value.Errors.Count > 0)
                .Select(kvp =>
                {
                    var key = kvp.Key;
                    var msgs = kvp.Value.Errors.Select(e => e.ErrorMessage).Where(m => !string.IsNullOrWhiteSpace(m));
                    return new { Key = key, Messages = msgs.ToArray() };
                })
                .ToArray();

            // صياغة رسالة موجزة (مثلاً: "LicenseFile: Invalid file type. ... | Email: ...")
            var messages = errors
                .SelectMany(e => e.Messages.Select(m => $"{(string.IsNullOrWhiteSpace(e.Key) ? "" : e.Key + ": ")}{m}"))
                .ToArray();

            var combinedMessage = string.Join(" | ", messages);

            var response = ResponseViewModel<string>.Fail(combinedMessage, ErrorCode.BadRequest);

            context.Result = new BadRequestObjectResult(response);
        }
    }
}
