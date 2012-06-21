using System;

namespace Simple.Web.Owin
{
	// Owin delegates break if you don't declare this. Stupid C#!
	static class wtf
	{
#pragma warning disable 169
		private static Action _1;
		private static Func<int, int, int> _2;
		private static Func<int, int> _3;
#pragma warning restore 169
	}
}