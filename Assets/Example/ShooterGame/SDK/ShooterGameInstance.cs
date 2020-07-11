using ShooterGame.Core;
using UnityEngine;

namespace ShooterGame
{
	namespace SDK
	{
		public class ShooterGameInstance
		{
			static ShooterGameInstance()
			{
				Debug.Log("static ctor");
			}
			
			public ShooterGameInstance()
			{
				Debug.Log("instance ctor");
			}

			public static void SendRequest(string msg)
			{
				Debug.Log($"[SendReq][127.0.0.1:8080]::{msg}");
			}

			public static GameSession CreateGameSession()
			{
				var newSession = new GameSession((long)UnityEngine.Random.Range(100000, 205290));
				return newSession;
			}
		}
	}
}