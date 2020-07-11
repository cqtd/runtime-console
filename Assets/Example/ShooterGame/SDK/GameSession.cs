using System.Collections.Generic;
using ShooterGame.Core;
using UnityEngine;

namespace ShooterGame
{
	namespace SDK
	{
		public class GameSession
		{
			readonly long sessionId;
			readonly List<ShooterGame.Core.Player> players;
			
			public GameSession(long sessionId)
			{
				this.sessionId = sessionId;
				this.players = new List<ShooterGame.Core.Player>();

				for (int i = 0; i < 8; i++)
				{
					var player = new GameObject($"Player_{i}");
					var playerComp = player.AddComponent<ShooterGame.Core.Player>();
					playerComp.SetIndex(i);
					playerComp.transform.position = Vector3.zero;

					players.Add(playerComp);
				}
			}
		}
	}
}