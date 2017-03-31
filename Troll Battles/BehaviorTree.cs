using UnityEngine;
using System.Collections;
using Globals;

// the namespace for the rest of the behavior tree system.
namespace BehaviourTreeKit 
{
    /// <summary>
    /// A class representing first generation behavior trees.
    /// this class is responsible for the agent's behavior out-come.
    /// </summary>
	public class BehaviorTree
	{
	    /// <summary>
	    /// a delay for each behavior tree tick.
	    /// </summary>
		public float traverseDelay {get;set;}

	    /// <summary>
	    /// the tree's root node.
	    /// </summary>
		public Composite root {get;private set;}

	    /// <summary>
	    /// the current status of the tree: this can be running,ready,failure and so forth.
	    /// </summary>
		public BehaviorStatus status {get;private set;}

	    /// <summary>
	    /// the owner of this behavior tree instance
	    /// </summary>
		public IAgent associatedAgent {get; private set;}

	    /// <summary>
	    /// a delegate in charge of running the tree's update coroutine.
	    /// </summary>
		public System.Func<IEnumerator> traverseHandler {get;private set;}
		
		public BehaviorTree(Composite a_rootNode, IAgent a_agent) 
		{
			root = a_rootNode;
			status = BehaviorStatus.READY;
			associatedAgent = a_agent;

			traverseHandler += TraverseTree;
			associatedAgent.deathHandler += () => traverseHandler -= TraverseTree;
		}

	    /// <summary>
	    /// the tree's traversal co-routine. this is looped until the agent is dead every frame.
	    /// </summary>
	    /// <returns></returns>
		private IEnumerator TraverseTree() 
		{
			for(;associatedAgent.status != AgentStatus.Dead;) 
			{
				if(root != null) 
				{
					status = root.Execute();
					yield return new WaitForSeconds(traverseDelay);
				}
				else 
				{
					Debug.LogError("Attempting to traverse tree failed," +
						"please intilaise the root node or check other issues occuring at the coroutine.");
				}
			}
		}

	    /// <summary>
	    /// suspends the current behavior tree from executing.
	    /// </summary>
		public void Suspend() 
		{
			if(root.suspendHandler != null) 
			{
				root.suspendHandler();
			}
			else 
			{
				Debug.LogError("Attempting to suspend the tree root failed," +
				               "please make sure the root's suspend delegate is correctly initialized");
			}
		}

	    /// <summary>
	    /// resets the behavior tree's nodes to initialized state.
	    /// </summary>
		public void Reset() 
		{
			if(root.resetHandler != null) 
			{
				root.resetHandler();
			}
			else 
			{
				Debug.LogError("Attempting to invoke the reset handler of the root node while it is null," +
				               "please make sure functions are subscribed");
			}
		}
	}
}