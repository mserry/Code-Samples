using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Globals.Enums;

/// <summary>
/// a class used as an interface for any type of "AI- Agent" within the game.
/// any AI controller implementation derives from this class.
/// </summary>
public class IAgent : MonoBehaviour 
{
	/// <summary>
	/// this property is inherited by all derived implementations of the IAgent Interface.
	/// it is called by gameplay components to know what's the AI's current target.
	/// </summary>
	/// <value> returns an Idamadageble interface which is the target.</value>
	public IDamageable target 
	{
		get 
		{
			return SetTarget();
		}
	}

	/// <summary>
	/// this member variable is inherited by all implementations of IAgent Interface.
	/// it is a reference to the current ingame entity that the Agent is controlling.
	/// </summary>
	protected IPlayer p_player;

	/// <summary>
	/// this method is the entry point for any implementation for a targeting algorthim.
	/// it is called by outside gameplay components, when the property's getter is invoked.
	/// </summary>
	/// <returns>The target.</returns>
	protected virtual IDamageable SetTarget() 
	{
		return null;
	}

	/// <summary>
	/// Starts the processing of world data to derive utility values for the agent's possible actions.
	/// </summary>
	/// <returns>the optimum action the agent should take.</returns>
	/// <param name="a_playersIngame">the players currently in the game.</param>
	/// <param name="a_agent"> a reference to the character the agent is controlling</param>
	public virtual Actions StartProcessing(List<IPlayer> a_playersIngame,IPlayer a_agent) 
	{
		return Actions.None;
	}
}
