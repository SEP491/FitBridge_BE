namespace FitBridge_Application.Dtos.Templates
{
    public class ReportStatusUpdatedModel : IBaseTemplateModel
    {
        public string TitleReportTitle { get; set; }

        public string BodyReportTitle { get; set; }

        public string BodyStatus { get; set; }

        public string BodyNote { get; set; }
    }
}