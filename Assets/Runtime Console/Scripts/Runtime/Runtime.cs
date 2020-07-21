namespace RuntimeConsole.Context
{
	public class Runtime
	{
#if LANG_KR
		public const string component = "컴포넌트";
		public const string whitelist_assembly = "어셈블리 화이트리스트";
		public const string blacklist_assembly = "어셈블리 블랙리스트";
		public const string whitelist_namespace = "네임스페이스 화이트리스트";
		public const string blacklist_namespace = "네임스페이스 블랙리스트";

		public const string context_menu_0 = "타겟 어셈블리 가져오기";
		public const string context_menu_1 = "컴포넌트 찾기";
		public const string context_menu_2 = "설정 초기화";
#else
		public const string component = "Component";
		public const string whitelist_assembly = "Whitelist of assembly";
		public const string blacklist_assembly = "Blacklist of assembly";
		public const string whitelist_namespace = "Whitelist of namespace";
		public const string blacklist_namespace = "Blacklist of namespace";

		public const string context_menu_0 = "Get Target Assembly";
		public const string context_menu_1 = "Find Components";
		public const string context_menu_2 = "Clear Preference";
#endif
	}
}