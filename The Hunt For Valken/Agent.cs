using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A class representing an AI Agent in the game.
/// </summary>
public class Agent : MonoBehaviour
{
    /// <summary>
    /// the type of agent, this can be a normal guard patroling, or a mini-boss or other NPC types.
    /// </summary>
	public Guard UnitType { get; private set; }

    /// <summary>
    /// the agent's animation component.
    /// </summary>
    public Animator Anim {get;private set;}

    /// <summary>
    /// the agent's navigation component.
    /// </summary>
	public NavMeshAgent NavAgent {get;private set;}

    /// <summary>
    /// The entite's state machine, depending on entity type different states like patrolling, attacking..etc can be pushed.
    /// </summary>
    public Stack<IState> StateMachine { get; private set; }

	protected void Awake()
	{
		Anim = GetComponent<Animator>();
		NavAgent= GetComponent<NavMeshAgent>();
		StateMachine = new Stack<IState>();
	}

	protected void Start()
	{
		UnitType = GetComponent<Guard>();
		StateMachine.Push(new IdleState(this));
	}

	protected void Update()
	{
		StateMachine.Peek().UpdateState();
	}

	public void SetState(IState newState)
	{
		StateMachine.Push(newState);
	}

	public void PopState()
	{
		StateMachine.Pop();
	}
}
