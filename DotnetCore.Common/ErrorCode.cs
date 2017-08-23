using System;
using System.ComponentModel;

namespace DotnetCore.Common
{
	public enum ErrorCode
	{
		[Description("成功")]
		OK = 0,

		[Description("失败")]
		Failed = 10000,
	}
}
