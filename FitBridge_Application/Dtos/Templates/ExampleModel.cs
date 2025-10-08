namespace FitBridge_Application.Dtos.Templates
{
    public record ExampleModel(string username) : IBaseTemplateModel
    {
        public string Username { get; set; } = username;
    }
}