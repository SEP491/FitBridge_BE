namespace FitBridge_Application.Dtos.Templates
{
    public class NewMessageModel(string body, string title) : IBaseTemplateModel
    {
        public string Body { get; set; } = body;

        public string Title { get; set; } = title;
    }
}