
namespace Assets.Project_Assets.Code.Scheduling
{
    public enum ScheduleType
    {
        Default,
        SimpleSchedule
    }

    public interface IScheduleDataBase
    {
        SimpleSchedule CreateSchedule(string id);   
    }
}

