namespace FitBridge_Application.MappingProfiles
{
    using AutoMapper;
    using FitBridge_Application.Dtos.GoalTrainings;
    using FitBridge_Domain.Entities.Accounts;

    public class GoalTrainingMappingProfile : Profile
    {
        public GoalTrainingMappingProfile()
        {
            CreateMap<GoalTraining, GoalTrainingResponsDto>();
        }
    }
}