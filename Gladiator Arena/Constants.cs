using UnityEngine;

namespace Globals
{
	namespace Constants
	{
	    /// <summary>
	    /// A static class representing constants used by the gameplay system. this is utilizied by all components.
	    /// </summary>
		public static class GameplayConstants
		{
			public const int PLAYER_DEFEND_DMG_REDUCTION_FACTOR = 2;
			public const int MAX_PLAYERS_INGAME = 4;
			public const int MANAGER_SCRIPT_SQUENCE_DELAY = 3;
			public const int MAX_PLAYER_ID_COUNT = 6;
			public const int NUMBER_OF_PLAYERS = MAX_PLAYER_ID_COUNT;
			public const int MAX_SPAWN_POINTS = 17;
			public const int MAX_WEAPONS = 3;
			public const int MAX_ACTIONS = MAX_PLAYERS_INGAME;
			public const int MAX_ENEMIES = MAX_WEAPONS;
			public const int LONGSWORD_BASE_DAMAGE = 800;
			public const int STAFF_BASE_DAMAGE = 1200;
			public const int DAGGER_BASE_DAMAGE = 500;
			
			public const float TILE_GAP_DISTANCE = 5f;
			public const float PLAYER_MAX_HEALTH = 5000f;
			public const float PLAYER_MAX_MANA = 2500f;
			public const float PLAYER_HEAL_ACTION_VALUE = 300f;
			public const float PLAYER_ATTACK_MANA_COST = 500f;
			public const float PLAYER_HEAL_MANA_COST = 300f;
			
			
			public const string SPAWN_POINT_TAG = "SpawnPoint";
			public const string PLAYER_TAG = "Player";
			public const string RESOURCES_FOLDER_PLAYER_WEAPONS = "Weapons/";
			public const string RESOURCES_FOLDER_PLAYER_PREFABS = "NewPrefabs/";
			public const string RESOURCES_FOLDER_PLAYER_ACTION_ICONS = "ActionAvatars/";
			public const string RESOURCES_FOLDER_PLAYER_AVATARS = "Avatars/";
			public const string RESOURCES_FOLDER_DEATH_AVATAR = "DeathAvatar";
		}


	    /// <summary>
	    /// A static class that represents the constant variables used by the utility Agents in the simulation.
	    /// </summary>
		public static class AgentConstants
		{
			public const float NORMALIZATION_CONSTANT = 100f;
			public const float STRENGTH_SIN_CURVE_SCALE_FACTOR = 1.7f;
			public const float HEALING_UTILITY_WEIGHTS_TOTAL = 5f;
			public const float HEALING_UTILITY_URGENCY_TO_HEAL_WEIGHT = 4f;
			public const float URGENCY_TO_HEAL_CURVE_SCALE_FACTOR_Y = ACCURACY_LINEAR_CURVE_SCALE_FACTOR_Y;
			public const float URGENCY_TO_HEAL_CURVE_TRANSLATION_Y = URGENCY_TO_HEAL_CURVE_SCALE_FACTOR_Y;
			public const float URGENCY_TO_HEAL_CURVE_SCALE_FACTOR_X = -3f;
			public const float URGENCY_TO_HEAL_CURVE_TRANSLATION_X = 1.5f;
			public const float MANA_ABUNDANCE_CURVE_POWER = 2.1f;
			public const float MANA_ABUNDANCE_CURVE_TRANSLATION_Y = 0.57f;
			public const float DEFEND_UTILITY_EXPONENTIAL_CURVE_SCALE_FACTOR = -2f;
			public const float DEFEND_UTILITY_EXPONENTIAL_CURVE_TRANSLATION_FACTOR = 0.1f;
			public const float MOVING_UTILITY_WEIGHTS_TOTAL = HEALING_UTILITY_WEIGHTS_TOTAL;
			public const float MOVING_UTILITY_CLOSENESS_WEIGHT = 3f;
			public const float MOVING_UTILITY_MOVE_URGE_WEIGHT = 4f;
			public const float CLOSENESS_CURVE_POWER = 1.2f;
			public const float RAGE_STRENGTH_WEIGHT = MOVING_UTILITY_CLOSENESS_WEIGHT;
			public const float RAGE_HEALTHLOST_WEIGHT = 2f;
			public const float RAGE_NORMALIZATION_CONSTANT = 5f;
			public const float RAGE_UTILITY_WEIGHTS_TOTAL = HEALING_UTILITY_WEIGHTS_TOTAL;
			public const float ATTACK_UTILITY_CURVE_SCALE_FACTOR_Y = 0.5f;
			public const float ATTACK_UTILITY_CURVE_TRANSLATION_X = 0.15f;
			public const float ATTACK_UTILITY_CURVE_TRANSLATION_Y = 0.92f;
			public const float TARGET_WORTH_LINEAR_CURVE_POWER = URGENCY_TO_HEAL_CURVE_TRANSLATION_X;
			public const float OVERALL_THREAT_RATIO_AGENT_THREAT_WEIGHT = URGENCY_TO_HEAL_CURVE_SCALE_FACTOR_X;
			public const float OVERALL_THREAT_RATIO_THREAT_TO_AGENT_WEIGHT = 2f;
			public const float OVERALL_THREAT_RATIO_WEIGHTS_TOTAL = 5f;
			public const float ACCURACY_LINEAR_CURVE_SCALE_FACTOR_Y = 0.5f;
			public const float ACCURACY_LINEAR_CURVE_TRANSLATION_Y = ACCURACY_LINEAR_CURVE_SCALE_FACTOR_Y;
			public const float ACCURACY_COSEC_CURVE_TRANSLATION_X = 0.5f;
			public const float ACCURACY_COSEC_CURVE_TRANSLATION_Y = 0.65f;
			public const float ACCURACY_COSEC_CURVE_SCALE_FACTOR_X = ACCURACY_COSEC_CURVE_TRANSLATION_X;
			public const float ACCURACY_LINEAR_CURVE_POWER = URGENCY_TO_HEAL_CURVE_TRANSLATION_X;
			public const float THREAT_LINEAR_CURVE_POWER = URGENCY_TO_HEAL_CURVE_TRANSLATION_X;
		}
	}
}