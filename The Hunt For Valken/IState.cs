using UnityEngine;
using System.Collections;

/// <summary>
/// a public interface that is implemented by customs states that can go in the agent's state machine.
/// </summary>
public interface IState
{
    /// <summary>
    /// this method is called in the agent's AI controller. it updates the current state in the agent's state machine.
    /// it is updated once per frame.
    /// </summary>
	void UpdateState();
}
