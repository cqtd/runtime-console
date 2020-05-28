using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RuntimeConsole
{
	public class Console : MonoBehaviour
	{
		// @TODO FEATURES :: 
		// Cache used commands
		// TextMeshPro Implementation
		// New Input System Implementation
		// Filter by Assembly
		
		[Header("Component")]
		[SerializeField] InputField inputField = default;
		[SerializeField] Text cellPrefab = default;
		[SerializeField] RectTransform root = default;
		[SerializeField] Canvas canvas = default;

		[Header("Setting")]
		[SerializeField] string[] m_namespaces = new string[] {"RuntimeConsole.Command"};
		[SerializeField] KeyCode activationKey = KeyCode.BackQuote;
		
		readonly List<Text> cells = new List<Text>();
		readonly Dictionary<string, MethodInfo> methods = new Dictionary<string, MethodInfo>();
		readonly Dictionary<string, Type> methodOwnerMap = new Dictionary<string, Type>();

		MethodInfo selected = default;
		bool activated = default;
		
		#region UNITY_EVENT_FUNCTION
		void Awake()
		{
			InitMethods();
			InitComponents();
			
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
					// @TODO : 이전 사용한 커맨드 기억 
				}

				if (Input.GetKeyDown(KeyCode.DownArrow))
				{
					// @TODO : 이전 사용한 커맨드 기억
				}
			}
		}
#endregion

		#region INIT

		/// <summary>
		/// 메서드 데이터로 미리 맵을 만듭니다.
		/// @TODO :: 어셈블리별 필터링 구현
		/// </summary>
		void InitMethods()
		{
			List<Type> list = Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(t => m_namespaces.Contains(t.Namespace))
				.ToList();

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
			DontDestroyOnLoad(canvas.gameObject);
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

		#region SCENE

		const string subsceneName = "ConsoleSubscene";
		const string subscenePath = "Assets/Scenes/" + subsceneName + ".unity";

		const string title = "빌드 세팅 오류";
		const string msg = "빌드 세팅에 씬을 추가할까요?";

		static bool loaded = false;
		
		#if UNITY_EDITOR
		[UnityEditor.MenuItem("Tools/Console/Load SubScene")]
		#endif
		public static void LoadConsole()
		{
			if (loaded)
			{
				Debug.LogWarning("이미 로드되어 있습니다.");
				return;
			}
			
			#if UNITY_EDITOR
			if (!HasRegistered())
			{
				bool add = UnityEditor.EditorUtility.DisplayDialog(title, msg, "추가", "아니요");
				if (add)
				{
					RegisterSceneToBuildSetting();
				}

				UnityEditor.EditorUtility.DisplayDialog("알림",
					add ? "추가하였습니다.\nPlay Mode를 재시작해주세요." : "씬을 추가하고 재시작 해주세요.", "확인");

				return;
			}
			#endif

			SceneManager.LoadScene(subsceneName, LoadSceneMode.Additive);
			loaded = true;
		}
		
#if UNITY_EDITOR
		[UnityEditor.MenuItem("Tools/Console/Add SubScene")]
#endif
		static void RegisterSceneToBuildSetting()
		{
#if UNITY_EDITOR
			var scenes = UnityEditor.EditorBuildSettings.scenes.ToList();
			scenes.Add(new UnityEditor.EditorBuildSettingsScene(subscenePath, true));
			UnityEditor.EditorBuildSettings.scenes = scenes.ToArray();
#endif
		}

		static bool HasRegistered()
		{
#if UNITY_EDITOR
			return UnityEditor.EditorBuildSettings.scenes.Any(e => e.path == subscenePath);
#else
			return true;
#endif
		}

		/// <summary>
		/// Fast Enter Play Mode 대응
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void ResetDomain()
		{
			loaded = false;
		}

		#endregion
	}
}