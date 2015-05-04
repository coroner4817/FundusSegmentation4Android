using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace Application
{
	[Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, Label= "Fundus Segmentation",ScreenOrientation = ScreenOrientation.Portrait)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			Thread.Sleep(5000); // Simulate a long loading process on app startup.
			StartActivity(typeof(MainActivity));
		}
	}
}

