using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Views.InputMethods;

namespace Application
{
	public static class ExtMethod
	{
		public static bool Contains(this string source,string toCheck, StringComparison ComparisonType)
		{
			return (source.IndexOf (toCheck, ComparisonType) >= 0);
		}
	}
}

