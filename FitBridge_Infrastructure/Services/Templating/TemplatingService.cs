using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Templates;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using Fluid;
using Fluid.Values;

namespace FitBridge_Infrastructure.Services.Templating
{
    internal class TemplatingService(IUnitOfWork unitOfWork) : ITemplatingService
    {
        private readonly FluidParser parser = new();

        public async Task<string> ParseTemplate<T>(EnumContentType templateType, T model)
        {
            var template = await GetTemplateString(templateType);
            if (!parser.TryParse(template, out var fluidTemplate, out var error))
            {
                throw new InvalidOperationException($"Template parsing failed: {error}");
            }

            var context = new TemplateContext(model);
            context.SetValue("model", new ObjectValue(model));

            return await fluidTemplate.RenderAsync(context);
        }

        private async Task<string> GetTemplateString(EnumContentType templateType)
        {
            var template = await unitOfWork.Repository<Template>().GetBySpecificationAsync(new GetByTemplateTypeSpecification(templateType));
            return template == null ? throw new NotFoundException(nameof(Template), templateType.ToString()) : template.TemplateBody;
        }
    }
}