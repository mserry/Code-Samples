using System.Collections.Generic;

namespace Assets.Project_Assets.Code.Scheduling
{
    public abstract class Schedule
    {
        public string id { get; protected set; }
        public int entryCount { get { return entries.Count; } }
        public int entryIndex { get; protected set; }

        protected List<ScheduleEntry> entries;
     
        public abstract bool IsFinished();
        public abstract int FindEntryForTime(float currentTime);
        public abstract int GetNextEntryIndex(int currentEntryIndex);

        public abstract float GetEndTime(int currentEntryIndex);
        public abstract float GetCurrentEndTime();

        public abstract void MoveToNextEntry();
        public abstract void ChooseEntryForTime(float currentTime);
        public abstract void AddEntry(ScheduleEntry newEntry);

        public abstract IAction ChooseNewAction(int currentEntryIndex, IAction previousAction);
        public abstract IAction ChooseNewAction(IAction previousAction);
        public abstract IAction ChooseNewAction();

    }

    public class SimpleSchedule : Schedule
    {
        public SimpleSchedule(string scheduleId)
        {
            id = scheduleId;
            entryIndex = -1;
            entries = new List<ScheduleEntry>();
        }

        public override bool IsFinished()
        {
            return entryIndex >= entries.Count - 1;
        }

        public override int FindEntryForTime(float currentTime)
        {
            if (entries.Count == 1) return 0;
            for (int i = 0; i < entries.Count; i++)
            {
                float starTime = entries[i].entryStartTime;
                float endTime = GetEndTime(i);

                if (endTime >= starTime)
                {
                    if (currentTime >= starTime && currentTime < endTime)
                        return i;
                }
                else
                {
                    if (currentTime >= starTime || currentTime < endTime)
                        return i;
                }
            }

            return 0;
        }

        public override int GetNextEntryIndex(int currentEntryIndex)
        {
            if ((currentEntryIndex + 1 >= entryCount) != true)
            {
                currentEntryIndex++;
            }
            else
            {
                return 0;
            }

            return currentEntryIndex;
        }

        public override float GetEndTime(int currentEntryIndex)
        {
            return entries[GetNextEntryIndex(currentEntryIndex)].entryStartTime;
        }

        public override float GetCurrentEndTime()
        {
            return GetEndTime(GetNextEntryIndex(entryIndex));
        }

        public override void MoveToNextEntry()
        {
            entryIndex = GetNextEntryIndex(entryIndex);
        }

        public override void ChooseEntryForTime(float currentTime)
        {
            entryIndex = FindEntryForTime(currentTime);
        }

        public override void AddEntry(ScheduleEntry newEntry)
        {
            if (newEntry == null || entries == null) return;
            entries.Add(newEntry);
        }

        public override IAction ChooseNewAction(int currentEntryIndex, IAction previousAction)
        {
            return entries[GetNextEntryIndex(currentEntryIndex)].ChooseNewAction(previousAction);
        }

        public override IAction ChooseNewAction(IAction previousAction)
        {
            return entries[entryIndex].ChooseNewAction(previousAction);
        }

        public override IAction ChooseNewAction()
        {
            return entries[entryIndex].currentAction;
        }
    }
}
