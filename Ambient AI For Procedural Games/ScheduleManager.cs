using System;
using System.Collections.Generic;
using Assets.Project_Assets.Code.Generic;
using Assets.Project_Assets.Code.Scheduling.Actions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Project_Assets.Code.Scheduling
{
    /// <summary>
    /// The schedule manager for the background npcs behaving as farmers.
    /// </summary>
    public class ScheduleManager : Singleton<ScheduleManager>
    {
        protected readonly List<Schedule> schedules;

        public ScheduleManager()
        {
            instance = this;
            schedules = new List<Schedule>();
        }

        public void SetupNpc(EntityType type, INpcData data)
        {
            var schedule = GetSchedule(type, data);
            if (schedule != null)
            {
                ((SimpleNPC) data).currentSchedule = schedule as SimpleSchedule;
                ((SimpleNPC) data).InitCurrentSchedule();
                ((SimpleNPC) data).enableUpdates = true;
            }
        }

        protected Schedule GetSchedule(EntityType type, INpcData data)
        {
            Schedule agentSchedule = CreateSchedule(type, data);
            schedules.Add(agentSchedule);
            
            return agentSchedule;
        }

        protected Schedule CreateSchedule(EntityType type, INpcData agentData)
        {
            switch (type)
            {
                case EntityType.Farmer:
                    return GetFarmerSchedule(type.ToString(),agentData);
              
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        protected Schedule GetFarmerSchedule(string id, INpcData data)
        {
            var npcSchedule = new SimpleSchedule(id);

            var exitHouseEntry = new SimpleScheduleEntry(Constants.FARMER_EXIT_HOUSE_TIME, Constants.FARMER_EXIT_HOUSE_SCHEDULE_ENTRY_ID);
            var farmingEntry = new SimpleScheduleEntry(Constants.FARMER_FARMING_START_TIME,Constants.FARMER_FARMING_SCHEDULE_ENTRY_ID);
            var enterHouseEntry = new SimpleScheduleEntry(Constants.FARMER_STOP_FARMING_TIME,Constants.FARMER_ENTER_HOUSE_SCHEDULE_ENTRY_ID);

            var farmingPoint = data.navigator.gameObject.FindNearestGameObjectWithTag(data.navigator.gameObject, Constants.FARM_FARMING_POINT_TAG).transform;
            exitHouseEntry.AddAction(new MoveToDestination(0,farmingPoint,
                data.navigator));

            farmingEntry.AddAction(new LoopAnimation(0, Constants.FARMER_FARMING_TRIGGER_NAME,
                Random.Range(Constants.FARMER_FARMING_DURATION_MIN,Constants.FARMER_FARMING_DURATION_MAX),data.navigator.GetComponent<Animator>()));

            var farmerHousePoint = data.navigator.gameObject.FindNearestGameObjectWithTag(data.navigator.gameObject,Constants.FARM_START_POINT_TAG).transform;
            enterHouseEntry.AddAction(new MoveToDestination(0,farmerHousePoint,
                data.navigator.GetComponent<NavMeshAgent>()));

            npcSchedule.AddEntry(exitHouseEntry);
            npcSchedule.AddEntry(farmingEntry);
            npcSchedule.AddEntry(enterHouseEntry);

            return npcSchedule;
        }
    }
}