using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Globals.Constants;
using Globals.Enums;
using Globals.Structs;

/// <summary>
/// The AI Controller Class. 
/// this class is used to process Utility Based Logic for the character in order to win the game.
/// it is contained as a script that is attached as a prefab to the agent.
/// </summary>
public class Agent : IAgent 
{
	/// <summary>
	/// The List of enemy agents in the game.
	/// </summary>
	private List<s_Enemy> m_enemyList;

	/// <summary>
	/// The possible actions associated with thier utility functions in the game.
	/// </summary>
	private List<s_Action> m_actions;

	/// <summary>
	/// the list of utility functions that will be used to calculate utilties for all the possible actions.
	/// </summary>
	private List<System.Func<float>> m_utilityFunctions;

	/// <summary>
	/// Starts the processing for the agent once it's the agent's turn..
	/// </summary>
	/// <returns> The Action with highest utility depending on the data.</returns>
	/// <param name="a_playersInGame"> players in the game.</param>
	/// <param name="a_agent">the player reference of the agent.</param>
	public override Actions StartProcessing(List<IPlayer> a_playersInGame, IPlayer a_agent) 
	{
		InitMemberVariables(a_playersInGame,a_agent);
		InitActions();
		return SelectedAction();
	}

	/// <summary>
	/// Sets the target for the agent depending on wiether the decision is to move or attack.
	/// </summary>
	/// <returns>The target.</returns>
	protected override IDamageable SetTarget() 
	{
		CalculateEnemyAccuracies();
		CalculateEnemyThreats();
		CalculateEnemyWorths();
		s_Enemy currentTarget = GetTarget();
		p_player.Weapon.Accuracy = GetAgentAccuracy(currentTarget.distanceToAgent);

		return (IDamageable) currentTarget.data;
	}

	/// <summary>
	/// Selects the final action for the agent based on the highest utility.
	/// </summary>
	/// <returns>The action.</returns>
	private Actions SelectedAction() 
	{
		Actions selectedAction = Actions.None;
		float highestUtility = GetHighestUtility(m_actions);
		//Debug.Log("Highest Utility: "+highestUtility);
		
		for(int i=0;i<m_actions.Count;i++) 
		{
			if(Mathf.Approximately(m_actions[i].utility,highestUtility)) 
			{
				selectedAction = m_actions[i].action;
			}
		}

		return selectedAction;
	}

	/// <summary>
	/// Initializes the member variables.
	/// </summary>
	/// <param name="a_enemyAgents"> enemy agents in the game.</param>
	/// <param name="a_agent"> the player reference of this agent.</param>
	private void InitMemberVariables(List<IPlayer> a_enemyAgents,IPlayer a_agent) 
	{
		p_player = a_agent;
		m_actions = new List<s_Action>();
		m_enemyList = new List<s_Enemy>();
		m_utilityFunctions = new List<System.Func<float>>();

		m_utilityFunctions.Add(GetAttackUtility);
		m_utilityFunctions.Add(GetDefendUtility);
		m_utilityFunctions.Add(GetHealingUtility);
		m_utilityFunctions.Add(GetMoveUtility);

		for(int i=0;i<a_enemyAgents.Count;i++) 
		{
			if(a_enemyAgents[i] != p_player) 
			{
				s_Enemy enemy = new s_Enemy (a_enemyAgents[i],0f,0f,0f,0f,0f,0f);
				enemy.distanceToAgent = Vector3.Distance(a_enemyAgents[i].Position.position,
				                                         p_player.Position.position);
				m_enemyList.Add(enemy);
			}
		}
	}

	/// <summary>
	/// Initialzies the possible actions in the game with the appropriate utility function.
	/// </summary>
	private void InitActions() 
	{
		var possibleActions = System.Enum.GetValues(typeof(Actions)) as Actions[];
	
		for(int i=0;i<possibleActions.Length-1;i++) 
		{
			m_actions.Add(new s_Action(possibleActions[i],m_utilityFunctions[i]));
			Debug.Log(m_actions[i].action+" Utility: "+m_actions[i].utility);
		}
	}

	/// <summary>
	/// Calculates the threat for each enemy to the agent.
	/// </summary>
	private void CalculateEnemyThreats() 
	{
		for(int i=0;i<m_enemyList.Count;i++) 
		{
			float myHealth = p_player.Health;
			float myAccuracyToTarget = GetAgentAccuracy(m_enemyList[i].distanceToAgent);
			p_player.Weapon.Accuracy = myAccuracyToTarget;
			float myDmg = p_player.Weapon.Damage;
			float enemyDmg =  m_enemyList[i].data.Weapon.Damage;
			float threatToMe = GetThreat(myHealth,enemyDmg);
			float threatToHim = GetThreat(m_enemyList[i].data.Health,myDmg);

			s_Enemy enemy = new s_Enemy(m_enemyList[i].data,
			                            threatToMe,m_enemyList[i].distanceToAgent,
			                            myAccuracyToTarget,threatToHim,0f,0f);
			m_enemyList[i] = enemy;
		}
	}

	/// <summary>
	/// Calculates the threat ratios of each enemy depending on the threat the agent poses to them and the threat they pose to the agent.
	/// </summary>
	private void CalculateThreatRatios() 
	{
		for(int i=0;i<m_enemyList.Count;i++) 
		{
			float ratio = GetOverallThreatRatio(m_enemyList[i]);
			m_enemyList[i] = new s_Enemy(m_enemyList[i].data,
			                             m_enemyList[i].threatToAgent,
			                             m_enemyList[i].distanceToAgent,
			                             m_enemyList[i].agentAccuracy,
			                             m_enemyList[i].agentThreat,
			                             ratio,m_enemyList[i].worth);
		}
	}

	/// <summary>
	/// Calculates the enemy accuracies depending on thier distances to the agent.
	/// </summary>
	private void CalculateEnemyAccuracies() 
	{
		for(int i=0;i<m_enemyList.Count;i++) 
		{
			float enemyAccuracy = GetEnemyAccuracy(m_enemyList[i].distanceToAgent);
			m_enemyList[i].data.Weapon.Accuracy = (enemyAccuracy >= 0f) ? enemyAccuracy : 0f;
		}
	}

	/// <summary>
	/// Calculates the enemy worths.
	/// </summary>
	private void CalculateEnemyWorths() 
	{
		for(int i=0;i<m_enemyList.Count;i++) 
		{
			float targetWorth = GetTargetWorth(m_enemyList[i]);

			m_enemyList[i] = new s_Enemy(m_enemyList[i].data,
			                             m_enemyList[i].threatToAgent,
			                             m_enemyList[i].distanceToAgent,
			                             m_enemyList[i].agentAccuracy,
			                             m_enemyList[i].agentThreat,
			                             0f,targetWorth);
		}
	}

	/// <summary>
	/// Gets the most viable target for our agent.
	/// </summary>
	/// <returns>The target for our agent based on the hight worth and overall threath ratio that the agent posses to the target.</returns>
	private s_Enemy GetTarget() 
	{
		CalculateThreatRatios();
		s_Enemy target = new s_Enemy();
		var worths = new float[GameplayConstants.MAX_ENEMIES];
		var ratios = new float[GameplayConstants.MAX_ENEMIES];

		for(int i=0;i<m_enemyList.Count;i++) 
		{
			worths[i] = m_enemyList[i].worth;
			ratios[i] = m_enemyList[i].threatRatio;
		}

		float highestThreatRatio = Mathf.Max(ratios);
		float highestWorth = Mathf.Max(worths);
		
		for(int i=0;i<m_enemyList.Count;i++) 
		{
			bool isMostViable = Mathf.Approximately(highestWorth, m_enemyList[i].worth);
			bool isMostAttackable = Mathf.Approximately(highestThreatRatio, m_enemyList[i].threatRatio);

			if(isMostViable && isMostAttackable) 
			{
				target = m_enemyList[i];
			}
		}
		
		return target;
	}

	/// <summary>
	/// Gets the closest enemy agent in distance to our agent.
	/// </summary>
	/// <returns>The closest enemy.</returns>
	private s_Enemy GetClosestEnemy () 
	{
		var enemy = new s_Enemy();
		var distancesToAgent = new List<float>();
		for(int i=0;i<m_enemyList.Count;i++) 
		{
			distancesToAgent.Add(m_enemyList[i].distanceToAgent);
		}

		float closestDist = Mathf.Min(distancesToAgent.ToArray());
		m_enemyList.ForEach(delegate(s_Enemy obj) 
			{
				if(Mathf.Approximately(obj.distanceToAgent,closestDist)) 
				{
					enemy = obj;
				}
			}
		);

		return enemy;
	}

	/// <summary>
	/// Gets the highest utitity out of all the utiltites.
	/// </summary>
	/// <returns>The highest utility.</returns>
	/// <param name="a_actions">the list of possible actions in the game.</param>
	private float GetHighestUtility(List<s_Action> a_actions) 
	{
		var utilities = new float[GameplayConstants.MAX_ACTIONS];
		
		for(int i=0;i<m_actions.Count;i++) 
		{
			utilities[i] = m_actions[i].utility;
		}
		
		return Mathf.Max(utilities);
	}

	/// <summary>
	/// Gets the final value for the attack utility based on the agent's rage level.
	/// By using a modified Natural Log Curve in the equation: Y = 0.5 x ln(rage + 0.15) + 0.92
	/// </summary>
	/// <returns>The final value for the attack utility.</returns>
	private float GetAttackUtility() 
	{
		float rage = GetRage();
		float attackUtility =  AgentConstants.ATTACK_UTILITY_CURVE_SCALE_FACTOR_Y * 
							   Mathf.Log(rage + AgentConstants.ATTACK_UTILITY_CURVE_TRANSLATION_X) +
							   AgentConstants.ATTACK_UTILITY_CURVE_TRANSLATION_Y;
	
		return Mathf.Clamp(attackUtility,0f,1f);
	}
	
	/// <summary>
	/// Gets the final value for the move utility based on the agent's closeness to the his closest target
	/// as well as his urge to move.
	/// By creating a combined utility between these two utilities we achieve the final utility value for moving.
	/// </summary>
	/// <returns>The  final value for the utility for movement.</returns>
	private float GetMoveUtility() 
	{
		float urgeToMove = GetUrgeToMove();
		float closeness = GetClosenessToGoal();
		float moveUtility = AgentConstants.MOVING_UTILITY_CLOSENESS_WEIGHT * closeness + 
		                     AgentConstants.MOVING_UTILITY_MOVE_URGE_WEIGHT * urgeToMove / 
							AgentConstants.MOVING_UTILITY_WEIGHTS_TOTAL;

		return Mathf.Clamp(moveUtility,0f,1f);
	}
	
	/// <summary>
	/// Gets the combined utility value for healing.
	/// </summary>
	/// <returns>The healing utility.</returns>
	private float GetHealingUtility() 
	{
		float healUtility = AgentConstants.HEALING_UTILITY_URGENCY_TO_HEAL_WEIGHT * 
							GetUrgencyToHeal(p_player.Health) +GetManaAbundancePerc(p_player.Mana);
		healUtility/= AgentConstants.HEALING_UTILITY_WEIGHTS_TOTAL;

		return Mathf.Clamp(healUtility,0f,1f);
	}

	/// <summary>
	/// Gets the final value for defending depending on the abundance of mana the agent currently posseses.
	/// By using a modified exponential curve to get the final value of the overall utility of defending.
	/// </summary>
	/// <returns>The defend utility.</returns>
	private float GetDefendUtility() 
	{
		float manaAbundunce = GetManaAbundancePerc(p_player.Mana);
		float defendUtility = Mathf.Exp(AgentConstants.DEFEND_UTILITY_EXPONENTIAL_CURVE_SCALE_FACTOR * manaAbundunce - 
		                                AgentConstants.DEFEND_UTILITY_EXPONENTIAL_CURVE_TRANSLATION_FACTOR);

		return Mathf.Clamp(defendUtility,0f,1f);
	}

	/// <summary>
	/// Gets the effeciency of the agent based on the average of his weapon efficiencies on the enemy agents.
	/// </summary>
	/// <returns>The effeciency of the agent</returns>
	private float GetAgentEfficency() 
	{
		float avgEfficiency = 0f;
		var myWepEfficencies = new List<float>();

		for(int i=0;i<m_enemyList.Count;i++) 
		{
			myWepEfficencies.Add(GetWepEfficiency(m_enemyList[i].distanceToAgent,
			                                      p_player.Weapon.Range));
			avgEfficiency += myWepEfficencies[i];
		}

		avgEfficiency/=myWepEfficencies.Count;

		return avgEfficiency;
	}

	/// <summary>
	/// Gets the strength of the agent based on his efficiency.
	/// The strength of the agent scales using the equation: Y = Sin (1.7 x average threat).
	/// </summary>
	/// <returns>The strength of the agent </returns>
	private float GetStrength() 
	{
		float agentEfficiency = GetAgentEfficency();
		float strength = Mathf.Sin(AgentConstants.STRENGTH_SIN_CURVE_SCALE_FACTOR * agentEfficiency);

		return Mathf.Clamp(strength,0f,1f);
	}

	/// <summary>
	/// Gets the rage of the agent based on his current strength and amount of health lost.
	/// </summary>
	/// <returns>The rage of the agent </returns>
	private float GetRage() 
	{
		float strength = GetStrength();
		float healthLost = 1 - p_player.Health / GameplayConstants.PLAYER_MAX_HEALTH;
		float rage = AgentConstants.RAGE_STRENGTH_WEIGHT * strength + 
					 AgentConstants.RAGE_HEALTHLOST_WEIGHT * healthLost / AgentConstants.RAGE_UTILITY_WEIGHTS_TOTAL;

		rage /= AgentConstants.RAGE_NORMALIZATION_CONSTANT;

		return Mathf.Clamp(rage,0f,1f);
	}
	
	/// <summary>
	/// Gets the overall threat ratio of each enemy.
	/// </summary>
	/// <returns>The overall threat ratio.</returns>
	/// <param name="a_enemy"> an enemy to our agent </param>
	private float GetOverallThreatRatio(s_Enemy a_enemy) 
	{
		float ratio = AgentConstants.OVERALL_THREAT_RATIO_AGENT_THREAT_WEIGHT * a_enemy.agentThreat + 
		              AgentConstants.OVERALL_THREAT_RATIO_THREAT_TO_AGENT_WEIGHT * a_enemy.threatToAgent / 
					  	AgentConstants.OVERALL_THREAT_RATIO_WEIGHTS_TOTAL;

		return Mathf.Clamp(ratio,0f,1f);
	}

	/// <summary>
	/// Gets the worth of an enemy to the agent based on the agent's accuracy on that enemy using a linear function.
	/// By using the modified linear function Y = accuracy ^ 1.5
	/// </summary>
	/// <returns>The target worth.</returns>
	/// <param name="a_enemy"> an enemy to our agent.</param>
	private float GetTargetWorth(s_Enemy a_enemy) 
	{
		float targetValue = Mathf.Pow(a_enemy.agentAccuracy, AgentConstants.TARGET_WORTH_LINEAR_CURVE_POWER);
		
		return Mathf.Clamp(targetValue,0f,1f);
	}

	/// <summary>
	/// Calulates the urge to move based on the average of the distances between our agent and the enemies.
	/// </summary>
	/// <returns>The urge to move.</returns>
	private float GetUrgeToMove() 
	{
		float urgeToMove = 0f;
		for(int i=0;i<m_enemyList.Count;i++) 
		{
			urgeToMove+= m_enemyList[i].distanceToAgent;
		}

		urgeToMove /= m_enemyList.Count;
		urgeToMove /= AgentConstants.NORMALIZATION_CONSTANT;

		return urgeToMove;
	}

	/// <summary>
	/// Gets the closeness of the agent to his ideal target.
	/// </summary>
	/// <returns> a value that represents how close the agent is to his ideal target.</returns>
	private float GetClosenessToGoal() 
	{
		float distToTarget = GetClosestEnemy().distanceToAgent / AgentConstants.NORMALIZATION_CONSTANT;
		float closeness = Mathf.Pow(distToTarget, AgentConstants.CLOSENESS_CURVE_POWER);

		return Mathf.Clamp(closeness,0f,1f);
	}

	/// <summary>
	/// Gets the agent accuracy based on the agent's distance to his enemies.
	/// The accuracy is calculated by using a modified linear function.
	/// The Equation is Y = 0.5 x (Weapon Efficiency ^ 1.5) + 0.5
	/// </summary>
	/// <returns>The agent accuracy.</returns>
	/// <param name="a_dist">the distance to an enemy.</param>
	private float GetAgentAccuracy(float a_dist) 
	{
		float accuracy = 0f;
		float wepEfficiency = GetWepEfficiency(a_dist,p_player.Weapon.Range);

		accuracy  = AgentConstants.ACCURACY_LINEAR_CURVE_SCALE_FACTOR_Y * 
					Mathf.Pow(wepEfficiency, AgentConstants.ACCURACY_LINEAR_CURVE_POWER) + 
					AgentConstants.ACCURACY_LINEAR_CURVE_TRANSLATION_Y;

		return Mathf.Clamp(accuracy,0f,1f);
	}

	/// <summary>
	/// Gets the enemy accuracy.
	/// Using the equation: Y = 1.1 - distance x 0.125
	/// This equation returns a value that decreases the higher distance gets.
	/// </summary>
	/// <returns>The enemy's accuracy.</returns>
	/// <param name="a_dist">The enemy's distance to the agent.</param>
	private float GetEnemyAccuracy(float a_dist) 
	{
		return Mathf.Clamp(1.1f - a_dist * 0.125f,0f,1f);
	}

	/// <summary>
	/// Gets the urgency to heal of our agent based on his health percentange.
	/// </summary>
	/// <returns>The urgency is calculated using the equation: 
	/// Y = 0.5 x Arctan ((-3 x health percentage)+1.5) + 0.5
	/// The equtation scales the urgency depending on the current health percentage.
	/// That is: the lower the health the higher the urgency.</returns>
	/// <param name="a_health">agent's health.</param>
	private float GetUrgencyToHeal(float a_health) 
	{
		float healthPerc = a_health/GameplayConstants.PLAYER_MAX_HEALTH;
		float urgency = AgentConstants.URGENCY_TO_HEAL_CURVE_SCALE_FACTOR_Y * 
						Mathf.Atan((AgentConstants.URGENCY_TO_HEAL_CURVE_SCALE_FACTOR_X * healthPerc) + 
			           				AgentConstants.URGENCY_TO_HEAL_CURVE_TRANSLATION_X) + 
						AgentConstants.URGENCY_TO_HEAL_CURVE_TRANSLATION_Y;

		return Mathf.Abs(Mathf.Clamp(urgency,0f,1f));
	}

	/// <summary>
	/// Gets the percentage of mana abundance the agent currently posseses based on the mana abundance rate.
	/// </summary>
	/// <returns>The mana abundance percentage.
	/// the value is based on the equation: Y = Arccos ((mana abundance rate ^ 2.1)) - 0.57 
	/// the mana abundance rate is the diffence in mana percentage of the agent from his full mana.</returns>
	/// <param name="a_mana"> the current mana value of the agent</param>
	private float GetManaAbundancePerc(float a_mana) 
	{
		float manaAbundanceRate = 1 - a_mana/GameplayConstants.PLAYER_MAX_MANA;
		float abundancePerc = Mathf.Acos(Mathf.Pow(manaAbundanceRate, AgentConstants.MANA_ABUNDANCE_CURVE_POWER)) - 
							  AgentConstants.MANA_ABUNDANCE_CURVE_TRANSLATION_Y;

		return Mathf.Clamp(abundancePerc,0f,1f);
	}

	/// <summary>
	/// Gets the threat of a unit based on the damage to health ratio.
	/// </summary>
	/// <returns>The threat of the unit.
 	/// the threat is calculated based on a modified linear equation: Y = damage to health ratio ^ 1.5 </returns>
	/// <param name="a_health">the target's health</param>
	/// <param name="a_dmg">the agent's damage to the target.</param>
	private float GetThreat(float a_health,float a_dmg) 
	{
		float dmgToHealth = GetDmgRelativeToHealthRatio(a_health,a_dmg);
		float threat = Mathf.Pow(dmgToHealth, AgentConstants.THREAT_LINEAR_CURVE_POWER);
	
		return threat;
	}

	/// <summary>
	/// Gets the dmg relative to health ratio of a unit to a target agent.
	/// </summary>
	/// <returns>The dmg relative to health ratio.
	/// </returns>
	/// <param name="a_health">the target's health.</param>
	/// <param name="a_dmg"> the agent's damage.</param>
	private float GetDmgRelativeToHealthRatio (float a_health,float a_dmg) 
	{
		float dmgRelToHealth = ((a_health / GameplayConstants.PLAYER_MAX_HEALTH) - 
		                        ((a_health - a_dmg) / GameplayConstants.PLAYER_MAX_HEALTH));

		return dmgRelToHealth;
	}

	/// <summary>
	/// Gets the wep efficiency of a unit depending on thier distance to thier target and thier current weapon range.
	/// </summary>
	/// <returns>The wep efficiency of the unit
	/// the weapon efficiency is calculated using a modified Cosec curve.
	/// The equation for the weapon efficiency is: Y = Cosec(distance to range ratio + 0.3) - 0.6
	/// </returns>
	/// <param name="a_dist"> distance to target.</param>
	/// <param name="a_wepRange">Unit/Agent's weapon range</param>
	private float GetWepEfficiency(float a_dist,Range a_wepRange) 
	{
	    float wepEfficiency = 0f;
		bool isEqual = Mathf.Approximately(a_dist,(float)a_wepRange);

		if(isEqual || a_dist<(float)a_wepRange)
		{
		    var distToRangeRatio = a_dist / (float)a_wepRange;
		    wepEfficiency = 1/Mathf.Sin(AgentConstants.ACCURACY_COSEC_CURVE_SCALE_FACTOR_X * distToRangeRatio +
			                            AgentConstants.ACCURACY_COSEC_CURVE_TRANSLATION_X) - 
										AgentConstants.ACCURACY_COSEC_CURVE_TRANSLATION_Y;
		}
	
		return wepEfficiency;
	}
}


