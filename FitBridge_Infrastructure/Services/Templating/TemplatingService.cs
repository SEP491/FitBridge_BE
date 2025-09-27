using FitBridge_Application.Dtos.Templates;
using FitBridge_Application.Interfaces.Repositories;
using FitBridge_Application.Interfaces.Services;
using FitBridge_Application.Specifications.Templates;
using FitBridge_Domain.Entities.MessageAndReview;
using FitBridge_Domain.Enums.MessageAndReview;
using FitBridge_Domain.Exceptions;
using FitBridge_Infrastructure.Services.Templating.Models;
using Fluid;
using Fluid.Values;

namespace FitBridge_Infrastructure.Services.Templating
{
    internal class TemplatingService
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

        public static IBaseTemplateModel GetTemplateModel(EnumContentType contentType, dynamic model)
        {
            switch (contentType)
            {
                //case EnumContentType.NewMessage:
                //    return "New message template content";

                //case EnumContentType.TrainingSlotCancelled:
                //    return "Training slot cancelled template content";

                //case EnumContentType.IncomingTrainingSlot:
                //    return "Incoming training slot template content";

                //case EnumContentType.NewGymFeedback:
                //    return "New gym feedback template content";

                //case EnumContentType.PaymentRequest:
                //    return "Payment request template content";

                //case EnumContentType.PackageBought:
                //    return "Package bought template content";

                default:
                    return new ExampleModel(model.username);
            }
        }
    }
}