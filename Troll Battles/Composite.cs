using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using Globals;


namespace BehaviourTreeKit 
{
	[XmlRoot("RootNode")]
	public abstract class Composite : Behavior
	{
		[XmlArray("children")]
		public List<Behavior> children 
		{
			get 
			{
				return m_children;
			}
		}

		protected List<Behavior> m_children = new List<Behavior>();

		protected abstract override void OnReset();
		protected abstract override void OnSuspend();
		protected abstract override BehaviorStatus Update();
		protected abstract override void Terminate(BehaviorStatus a_status);
		
		public void AddChild(Behavior a_child)
		{
			if(m_children != null) 
			{
				if (a_child != null)
				{
					m_children.Add(a_child);
				}
				else 
				{
					Debug.LogError("Attempting to add a null child to the children list, " +
						"please check if the child is correctly intialized");
				}
			}
			else 
			{
				Debug.LogError("List of children is currently null," +
					"please make sure it is correctly intialized");
			}
		}
		
		public void RemoveChild(Behavior a_child)
		{
			if(m_children != null) 
			{
				if (a_child != null)
				{
					m_children.Remove(a_child);
				}
				else 
				{
					Debug.LogError("Attempting to remove a null child reference from the list," +
						"please check if the reference passed is intialized correctly");
				}
			}
			else 
			{
				Debug.LogError("Attempting to remove a child node from a null list of nodes," +
				               "please make sure the list is properly intialized");
			}
		}

		public void RemoveChild(int a_index) 
		{
			try 
			{
				m_children.RemoveAt(a_index);
			}
			catch (System.Exception ex) 
			{
				Debug.LogException(ex);
				Debug.LogError("Attempting to remove a child node at an invalid index," +
				               "please check the current number of child nodes before removing");
			}
		}
		
		public void ClearChildren()
		{
			m_children.Clear();
		}
	}
}