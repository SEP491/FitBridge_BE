using FitBridge_Domain.Enums.MessageAndReview;

namespace FitBridge_Application.Interfaces.Services
{
    public interface ITemplatingService
    {
        /// <summary>
        /// Replaces placeholders in a template string with their corresponding values.
        /// </summary>
        /// <param name="templateType">The type of template to parse.</param>
        /// <param name="model">The model containing the data to be used in the template.</param>
        /// <returns>The template string with all placeholders replaced by their corresponding values.</returns>
        Task<string> ParseTemplate<T>(EnumContentType templateType, T model);
    }
}