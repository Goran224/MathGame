namespace MathGame_Service.Interfaces
{
    public interface IHangfireJobService
    {
        void ScheduleGameExpressionGenerationJob();
    }
}
