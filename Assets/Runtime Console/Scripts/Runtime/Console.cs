using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeConsole
{
	using Context;
	
	public class Console : MonoBehaviour
	{
		// @TODO FEATURES :: 
		// TextMeshPro Implementation
		// New Input System Implementation
		
		public enum EFilterType {
			WhiteList = 0,
			BlackList = 1,
		}

		#region Component
		[Header(Runtime.component)]
		[SerializeField] InputField inputField = default;
		[SerializeField] Text cellPrefab = default;
		[SerializeField] RectTransform root = default;
		[SerializeField] Canvas canvas = default;
		#endregion

		#region Filter
		[SerializeField] EFilterType assemblyFilterType = EFilterType.WhiteList;
		[Tooltip(Runtime.whitelist_assembly)]
		[SerializeField] string[] m_assemblyWhitelist = new string[] {"Assembly-CSharp", "Assembly-CSharp-firstpass"};
		[Tooltip(Runtime.blacklist_assembly)]
		[SerializeField] string[] m_assemblyBlacklist = new string[] {"Assembly-CSharp", "Assembly-CSharp-firstpass"};
		
		[SerializeField] EFilterType namespaceFilterType = EFilterType.WhiteList;
		[Tooltip(Runtime.whitelist_namespace)]
		[SerializeField] string[] m_namespaceWhitelist = new string[] {"RuntimeConsole.Command"};
		[Tooltip(Runtime.blacklist_namespace)]
		[SerializeField] string[] m_namespaceBlacklist = new string[] {"RuntimeConsole.Command"};
		#endregion

		#region Preference
		[SerializeField] KeyCode activationKey = KeyCode.BackQuote;
		#endregion

		#region Dictionaries
		readonly List<Text> cells = new List<Text>();
		readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
		readonly Dictionary<string, Type> methodOwnerMap = new Dictionary<string, Type>();
		#endregion

		#region Internal
		MethodInfo selected = default;
		bool activated = default;

		bool upArrowIsDown = false;
		bool downArrowIsDown = false;

		int currentIndex = -1;
		
		readonly UnityMethodInfoEvent onExecuteMethod = new UnityMethodInfoEvent();
		#endregion
		
		#region UNITY_EVENT_FUNCTION
		void Awake()
		{
			InitMethods();
			InitComponents();
			
			onExecuteMethod.AddListener(CacheMethodInfo);
			
			OnActivationChanged();
		}

		void Update()
		{
			if (Input.GetKeyDown(activationKey))
			{
				activated = !activated;
				OnActivationChanged();
			}
			
			if (!activated)
			{
				return;
			}
			
			if (inputField.isFocused)
			{
				if (Input.GetKeyDown(KeyCode.Tab))
				{
					if (selected != null)
					{
						inputField.text = selected.Name;
						inputField.caretPosition = inputField.text.Length;
						inputField.Select();
						inputField.ActivateInputField();
					}
				}


				if (Input.GetKeyDown(KeyCode.UpArrow))
				{
					if (!upArrowIsDown)
					{
						upArrowIsDown = true;
						int count = GetCachedMethodInfoCount();

						if (count > 0)
						{
							currentIndex = (currentIndex + 1) % count;
							Debug.Log("Up");

							inputField.text = GetCachedMethodInfoByIndex(currentIndex);
							inputField.caretPosition = inputField.text.Length;
							inputField.Select();
							inputField.ActivateInputField();
						}
					}
				}

				if (Input.GetKeyUp(KeyCode.UpArrow))
				{
					upArrowIsDown = false;
				}

				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					if (!downArrowIsDown)
					{
						downArrowIsDown = true;
						int count = GetCachedMethodInfoCount();

						if (count > 0)
						{
							if (currentIndex < 0)
							{
								currentIndex %= count;
							}
							else
							{
								currentIndex = (currentIndex - 1 + count) % count;
							}
					
							// @TODO : 이전 사용한 커맨드 기억
							Debug.Log("Down");
					
							inputField.text = GetCachedMethodInfoByIndex(currentIndex);
							inputField.caretPosition = inputField.text.Length;
							inputField.Select();
							inputField.ActivateInputField();
						}
					}
				}

				if (Input.GetKeyUp(KeyCode.DownArrow))
				{
					downArrowIsDown = false;
				}
			}
		}

		void OnValidate()
		{
			
		}

		#endregion

		#region INIT

		[ContextMenu(Runtime.context_menu_0)]
		void GetTargetAssembly()
		{
			var assemblies = FindTypes();
			
			foreach (var assembly in assemblies)
				Debug.Log(assembly);
		}

		List<Type> FindTypes()
		{
			var list = new List<Type>();
			
			var assemblies = AppDomain.CurrentDomain.GetAssemblies()
				.Where(t => { 
					switch(assemblyFilterType) {
						case EFilterType.WhiteList:
							return m_assemblyWhitelist.Contains(t.GetName().Name);
						case EFilterType.BlackList:
							return !m_assemblyBlacklist.Contains(t.GetName().Name);
						default:
							return false;
					}
				});
			
			foreach (var assembly in assemblies)
			{
				Debug.Log($"[Found Assembly]::{assembly.GetName().Name}");
				
				list.AddRange(
					assembly
						.GetTypes()
						.Where(t =>
						{
							switch(namespaceFilterType) {
								case EFilterType.WhiteList:
									return m_namespaceWhitelist.Contains(t.Namespace);
								case EFilterType.BlackList:
									return !m_namespaceBlacklist.Contains(t.Namespace);
								default:
									return false;
							}
						}
					)
				);
			}

			return list;
		}

		/// <summary>
		/// 메서드 데이터로 미리 맵을 만듭니다.
		/// </summary>
		void InitMethods()
		{
			var list = FindTypes();
			
			foreach (Type type in list)
			{
				var methodInfos = type.GetMethods()
					.Where(m => m.DeclaringType == type);

				foreach (MethodInfo methodInfo in methodInfos)
				{
					methods[methodInfo.Name] = methodInfo;
					methodOwnerMap[methodInfo.Name] = type;
				}
			}
		}

		/// <summary>
		/// 이벤트 리스너 등 컴포넌트를 할당합니다
		/// </summary>
		void InitComponents()
		{
			inputField.onValueChanged.AddListener(OnValueChanged);
			inputField.onEndEdit.AddListener(OnEndEdit);
			cellPrefab.gameObject.SetActive(false);
			
			DontDestroyOnLoad(gameObject);
			DontDestroyOnLoad(EventSystem.current.gameObject);
		}
		
		#endregion

		#region CALLBACK

		/// <summary>
		/// 콘솔 열기 키에 대응됩니다.
		/// </summary>
		void OnActivationChanged()
		{
			canvas.gameObject.SetActive(activated);
			EventSystem.current.SetSelectedGameObject(null);
			
			if (activated)
			{
				StartCoroutine(Focus());
			}
		}

		/// <summary>
		/// input field 에 포커스를 가져옵니다.
		/// </summary>
		/// <returns></returns>
		IEnumerator Focus()
		{
			yield return null;
			
			inputField.text = string.Empty;
			inputField.Select();
		}

		/// <summary>
		/// 콘솔 입력값이 변경될 때 마다 호출됩니다.
		/// @TODO :: 네임스페이스 표기
		/// </summary>
		/// <param name="input">auto filled</param>
		void OnValueChanged(string input)
		{
			// 편의상 매개변수는 스페이스로 구분할 것. 첫 블록은 메서드 이름
			MethodInfo[] filtered;
			string methodName = input.Split(' ').FirstOrDefault();

			// 필터링 로직
			if (string.IsNullOrEmpty(methodName))
			{
				filtered = new MethodInfo[0];
			}
			else
			{
				filtered = methods.Values
					.Where(e => e.Name.ToLower().Contains(methodName.ToLower()))
					.OrderBy(e => e, new MethodComparer(methodName)).ToArray();
			}

			// 가능한 엔트리에 대해 텍스트 박스 풀링
			int count = filtered.Length;
			for (int i = count; i < cells.Count; i++)
			{
				var cell = GetCell(i);
				cell.gameObject.SetActive(false);
			}

			for (int i = 0; i < count; i++)
			{
				var cell = GetCell(i);
				var method = filtered[i];

				string displayText = $"{method.Name}(";
				var parameters = method.GetParameters();
				for (int index = 0; index < parameters.Length; index++)
				{
					var parameter = parameters[index];
					displayText += $"{parameter.ParameterType.Name} {parameter.Name}";
					if (index != parameters.Length - 1)
					{
						displayText += ", ";
					}
				}

				displayText += ")";

				cell.text = displayText;
				cell.gameObject.SetActive(true);
			}

			selected = filtered.FirstOrDefault();
		}

		/// <summary>
		/// 콘솔 엔터 실행시
		/// </summary>
		/// <param name="input"></param>
		void OnEndEdit(string input)
		{
			if (selected == null)
				return;
			
			// 액티베이터로 클래스 인스턴스 생성
			object classInstance = Activator.CreateInstance(methodOwnerMap[selected.Name], null);

			// 파라미터 검증
			string[] blocks = input.Split(' ');
			int inputParameterCount = blocks.Length - 1;
			ParameterInfo[] parameters = selected.GetParameters();

			// 파라미터 개수 일치
			if (inputParameterCount == parameters.Length)
			{
				try
				{
					object[] convertedParameters = new object[inputParameterCount];
					for (int i = 0; i < inputParameterCount; i++)
					{
						var parameter = parameters[i];
						var inputValue = blocks[i + 1];

						// convert to types
						var converted = Convert.ChangeType(inputValue, parameter.ParameterType);
						convertedParameters[i] = converted;
					}

					// 메서드 컨슘
					selected.Invoke(classInstance, convertedParameters);
					onExecuteMethod?.Invoke(selected);
				}
				catch (Exception e)
				{
					Debug.LogError(e);
				}
			}
			else
			{
				Debug.LogError("The number of parameter is not compatible.");
			}

			// 입력 필드 초기화
			inputField.text = null;
			inputField.Select();
			inputField.ActivateInputField();
		}

		const string prefKey = "runtime-console.pref.key";
		void CacheMethodInfo(MethodInfo mi)
		{
			List<string> list = GetCachedMethodInfo();

			if (list.Contains(mi.Name))
			{
				list.Remove(mi.Name);
				Debug.Log($"[CachedMethodInfo]::{mi.Name} has removed from list.");
			}
			
			list.Insert(0, mi.Name);
			var serialized = JsonConvert.SerializeObject(list);

			Debug.Log($"[CachedMethodInfo]::{mi.Name} has been cached. \n{serialized}");
			PlayerPrefs.SetString(prefKey, serialized);
			
			currentIndex = -1;
		}

		int GetCachedMethodInfoCount()
		{
			return GetCachedMethodInfo().Count;
		}

		List<string> GetCachedMethodInfo()
		{
			string json = PlayerPrefs.GetString(prefKey, null);
			if (string.IsNullOrEmpty(json))
			{
				return new List<string>()
				{
					"",
				};
			}

			return JsonConvert.DeserializeObject<List<string>>(json);
		}

		string GetCachedMethodInfoByIndex(int index)
		{
			if (GetCachedMethodInfoCount() <= index)
			{
				return string.Empty;
			}

			return GetCachedMethodInfo()[index];
		}

		#endregion

		#region UTILITY

		/// <summary>
		/// 간단한 셀 풀링
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		Text GetCell(int index)
		{
			Text instance;
			if (cells.Count <= index)
			{
				instance = Instantiate(cellPrefab, root);
				cells.Add(instance);
			}
			else
			{
				instance = cells[index];
			}

			return instance;
		}
		#endregion

		#region EDITOR
		
#if UNITY_EDITOR
		[ContextMenu(Runtime.context_menu_1)]
		public void FindComponents()
		{
			if (inputField == null)
				inputField = GetComponentInChildren<InputField>();

			if (cellPrefab == null)
				cellPrefab = GetComponentsInChildren<Text>().FirstOrDefault(e => e.name == "TextPool");

			if (root == null)
				root = transform.GetChild(0) as RectTransform;

			if (canvas == null)
				canvas = GetComponentInChildren<Canvas>();

			UnityEditor.EditorUtility.DisplayDialog("Done!", "Components are found. save the scene or prefab.", "OK");
		}
#endif

		[ContextMenu(Runtime.context_menu_2)]
		public void ClearPreference()
		{
			PlayerPrefs.DeleteKey(prefKey);
		}
		#endregion
	}
}