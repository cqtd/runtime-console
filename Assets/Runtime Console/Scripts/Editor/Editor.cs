namespace RuntimeConsole
{
	namespace Editor
	{
		namespace Context
		{
			class Editor
			{
#if LANG_KR
				public const string initialize_alert = "컴포넌트 이상 없음";
				public const string find_button_text = "컴포넌트 초기화 하기";

				public const string label_assembly = "어셈블리";
				public const string label_namespace = "네임스페이스";
				
				public const string menu_item_0 = "도구/콘솔/서브씬 로드";
				public const string menu_item_1 = "도구/콘솔/서브씬 빌드 설정에 추가";
				
				public const string load_alert = "이미 로드되어 있습니다.";

				public const string add_caption = "빌드 세팅 오류";
				public const string add_msg = "빌드 세팅에 씬을 추가할까요?";
				public const string add_yes = "추가";
				public const string add_no = "아니요";

				public const string load_caption = "알림";
				public const string load_msg_0 = "추가하였습니다.\nPlay Mode를 재시작해주세요.";
				public const string load_msg_1 = "씬을 추가하고 재시작 해주세요.";
				public const string load_msg_2 = "확인";
#else
				public const string initialize_alert = "All components are initialized well.";
				public const string find_button_text = "Find Components";

				public const string label_assembly = "Assembly";
				public const string label_namespace = "Namespace";
				
				public const string menu_item_0 = "Tools/Console/Load Subscene";
				public const string menu_item_1 = "Tools/Console/Add SubScene To Build Settings";
				
				public const string load_alert = "Already loaded";

				public const string add_caption = "Build Settings Exception";
				public const string add_msg = "Add scene to build settings automatically?";
				public const string add_yes = "Add";
				public const string add_no = "No";

				public const string load_caption = "Alert";
				public const string load_msg_0 = "Added.\nRestart PlayMode.";
				public const string load_msg_1 = "Restart PlayMode after Add scene to build setting.";
				public const string load_msg_2 = "OK";
#endif
			}
		}
	}
}