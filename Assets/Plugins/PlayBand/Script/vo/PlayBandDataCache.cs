using UnityEngine;

public class PlayBandDataCache
{
		public PlayBandData data = new PlayBandData ();
		public  float force = 0f;
		public Vector2 direction = Vector2.zero;
		public  float angel = 0f;
		//
		public PlayBandDataCache ()
		{
		}
		//
		public void Update (PlayBandData data, float currentForce, Vector2 forceDirection, float forceAngel)
		{
				this.data.Update (data);
				force = currentForce;
				direction.Set (forceDirection.x, forceDirection.y);
				angel = forceAngel;
		}

		public void Update (PlayBandDataCache data)
		{
				this.data.Update (data.data);
				force = data.force;
				direction.Set (data.direction.x, data.direction.y);
				angel = data.angel;
		}

}
