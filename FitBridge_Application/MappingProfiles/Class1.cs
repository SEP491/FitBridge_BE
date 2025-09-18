namespace FitBridge_Application.MappingProfiles
{
    using AutoMapper;
    using FitBridge_Application.Dtos.GoalTrainings;
    using FitBridge_Domain.Entities.Accounts;
    using FitBridge_Domain.Entities.Trainings;

    public class GoalTrainingMappingProfile : Profile
    {
        public GoalTrainingMappingProfile()
        {
            CreateMap<GoalTraining, GoalTrainingResponsDto>();
        }
    }
}