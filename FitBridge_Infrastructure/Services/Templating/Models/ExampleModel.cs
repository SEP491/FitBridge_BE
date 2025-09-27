namespace FitBridge_Infrastructure.Services.Templating.Models
{
    internal record ExampleModel(string username) : IBaseTemplateModel
    {
        internal string Username { get; set; } = username;
    }
}