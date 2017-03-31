namespace Assets.Project_Assets.Code.Scheduling
{
    public abstract class ScheduleEntry
    {
        public IAction currentAction { get; set; }
        public float entryStartTime { get; protected set; }
        public string id { get; protected set; }

        public abstract void AddAction(IAction newAction);
        public abstract IAction ChooseNewAction(IAction previousAction);
    }
}
