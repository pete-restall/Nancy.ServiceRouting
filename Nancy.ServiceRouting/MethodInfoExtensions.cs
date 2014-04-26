using System;
using System.Reflection;

namespace Restall.Nancy.ServiceRouting
{
	internal static class MethodInfoExtensions
	{
		public static Type TypeOfFirstParameter(this MethodInfo method)
		{
			return method.GetParameters()[0].ParameterType;
		}
	}
}
