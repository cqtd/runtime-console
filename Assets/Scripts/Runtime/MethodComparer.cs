using System.Collections.Generic;
using System.Reflection;

namespace RuntimeConsole
{
	/// <summary>
	/// 편의상 사용되는 비교자 클래스
	/// </summary>
	public class MethodComparer : IComparer<MethodInfo>
	{
		readonly string keyword = default;
		
		public int Compare(MethodInfo x, MethodInfo y)
		{
			if (x == null && y == null) return 0; 
			if (x == null) return 1;
			if (y == null) return -1;

			bool isXStartWith = x.Name.ToLower().StartsWith(keyword.ToLower());
			bool isYStartWith = y.Name.ToLower().StartsWith(keyword.ToLower());
			
			if (isXStartWith && isYStartWith)
			{
				return string.CompareOrdinal(x.Name, y.Name);
			}

			if (isXStartWith)
			{
				return -1;
			}

			if (isYStartWith)
			{
				return 1;
			}

			return string.CompareOrdinal(x.Name, y.Name);
		}

		public MethodComparer(string keyword) : base()
		{
			this.keyword = keyword;
		}
	}
}