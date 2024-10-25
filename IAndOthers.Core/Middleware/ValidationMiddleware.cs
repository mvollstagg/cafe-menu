using IAndOthers.Core.Api.Validations;
using IAndOthers.Core.Data.Enumeration;
using IAndOthers.Core.Data.Result;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace IAndOthers.Core.Middleware
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            // Enable buffering to allow reading the body multiple times
            context.Request.EnableBuffering();

            if (context.Request.Method == HttpMethods.Post || context.Request.Method == HttpMethods.Put)
            {
                var endpoint = context.GetEndpoint();
                if (endpoint != null)
                {
                    var actionDescriptor = endpoint.Metadata.GetMetadata<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>();
                    if (actionDescriptor != null)
                    {
                        var parameters = actionDescriptor.MethodInfo.GetParameters();

                        foreach (var parameter in parameters)
                        {
                            if (parameter.ParameterType.IsClass)
                            {
                                // Read the request body into a memory stream
                                using (var memoryStream = new MemoryStream())
                                {
                                    await context.Request.Body.CopyToAsync(memoryStream);
                                    memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream position

                                    using (var reader = new StreamReader(memoryStream))
                                    {
                                        var bodyAsText = await reader.ReadToEndAsync();
                                        memoryStream.Seek(0, SeekOrigin.Begin); // Reset again for subsequent readers

                                        // Deserialize the string into the model
                                        var model = JsonConvert.DeserializeObject(bodyAsText, parameter.ParameterType);

                                        var errorMessages = new List<KeyValuePair<string, List<string>>>();

                                        foreach (var prop in parameter.ParameterType.GetProperties())
                                        {
                                            var validationAttributes = prop.GetCustomAttributes(false)
                                                .OfType<IIOValidation>();

                                            foreach (var validation in validationAttributes)
                                            {
                                                var value = prop.GetValue(model);
                                                object? relatedValue = null;

                                                // If this validation requires a related property, get its value
                                                if (validation is IOValidationMustBeSameAttribute sameAttribute)
                                                {
                                                    var relatedProperty = model.GetType().GetProperty(sameAttribute.RelatedProperty);
                                                    if (relatedProperty != null)
                                                    {
                                                        relatedValue = relatedProperty.GetValue(model);
                                                    }
                                                }

                                                var result = await validation.IOValidate(value, relatedValue, serviceProvider);
                                                if (result != null)
                                                {
                                                    var existingError = errorMessages.FirstOrDefault(e => e.Key == prop.Name);

                                                    if (existingError.Equals(default(KeyValuePair<string, List<string>>)))
                                                    {
                                                        errorMessages.Add(new KeyValuePair<string, List<string>>(prop.Name, new List<string> { result }));
                                                    }
                                                    else
                                                    {
                                                        var updatedError = new KeyValuePair<string, List<string>>(existingError.Key, existingError.Value);
                                                        updatedError.Value.Add(result);
                                                        errorMessages[errorMessages.IndexOf(existingError)] = updatedError;
                                                    }
                                                }
                                            }
                                        }

                                        if (errorMessages.Count > 0)
                                        {
                                            var ioResult = new IOResult<string>(IOResultStatusEnum.Error, null, errorMessages);

                                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                            context.Response.ContentType = "application/json";
                                            var jsonResult = JsonConvert.SerializeObject(ioResult);
                                            await context.Response.WriteAsync(jsonResult);
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Rewind the request body for the next middleware or endpoint to read
            context.Request.Body.Position = 0;

            await _next(context);
        }
    }
}
