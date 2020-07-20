using System;
using UnityEngine;

namespace RuntimeConsole.Command
{
	class Debugging
	{
		public static void ToggleVolumes()
		{
			Debug.Log("토글 볼륨");
		}
	}

	class Setting
	{
		public static void SetMaxFPS(int fps = 60)
		{
			Application.targetFrameRate = fps;
			Debug.Log($"Target frame rate is updated! FPS : {fps}");
		}
	}

	class GamePlay
	{
		public static void Exit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit(0);
#endif
			Debug.Log("static method 'Exit()' has been executed.");
		}

		public static void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit(0);
#endif
			Debug.Log("static method 'Quit()' has been executed.");
		}
	}

	class Memory
	{
		public static void ToggleMemory()
		{
			Debug.Log("토글 메모리");
		}

		public static void ForceGC()
		{
			GC.Collect();
			Debug.Log("Garbage Collected.");
		}
	}
}