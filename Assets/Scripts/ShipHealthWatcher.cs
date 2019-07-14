using UnityEngine;

namespace Assets.Scripts
{
	public class ShipHealthWatcher : MonoBehaviour
	{
		public IntVariable ShipHealth;
		public GameEvent DestroyedEvent;
		private void Awake()
		{
			ShipHealth.Changed += ShipHealth_Changed;
		}

		private void ShipHealth_Changed(object sender, ReferenceChangedEventHandler<int> e)
		{
			if(e.NewValue <=0)
			{
				DestroyedEvent.Raise();
			}
		}
	}
}
