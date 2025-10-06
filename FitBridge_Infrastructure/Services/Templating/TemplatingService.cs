using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Templates;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using Fluid;
using Fluid.Values;
using Microsoft.Extensions.Logging;

namespace FitBridge_Infrastructure.Services.Templating
{
    internal class TemplatingService()
    {
        private readonly FluidParser parser = new();

        public async Task<string> ParseTemplateAsync(string template, IBaseTemplateModel model)
        {
            if (!parser.TryParse(template, out var fluidTemplate, out var error))
            {
                throw new InvalidOperationException($"Template parsing failed: {error}");
            }

            var context = new TemplateContext(model);
            context.SetValue("model", new ObjectValue(model));

            return await fluidTemplate.RenderAsync(context);
        }
    }
}