namespace FitBridge_Application.Dtos.Templates
{
    public class NewReportModel : IBaseTemplateModel
    {
        public string TitleReporterName { get; set; }

        public string BodyReporterName { get; set; }

        public string BodyReportTitle { get; set; }

        public string BodyReportType { get; set; }
    }
}