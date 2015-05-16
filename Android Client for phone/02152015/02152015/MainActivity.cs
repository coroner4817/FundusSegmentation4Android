using System;
using Android.Database;
using Android.Net;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;

namespace Application
{
	[Activity (Label = "MainActivity", Icon = "@drawable/icon",ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
	
		private Button mButtonCreatFile;
		private Button mButtonCheckFile;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			mButtonCreatFile = FindViewById<Button> (Resource.Id.btnCreatFile);
			mButtonCheckFile = FindViewById<Button> (Resource.Id.btnCheckFile);

			mButtonCreatFile.Click += delegate {

				//Pull a dialog
				FragmentTransaction transaction = FragmentManager.BeginTransaction ();
				dialogCreate signUpDialog = new dialogCreate ();
				signUpDialog.Show (transaction, "dialog fragment");

			};


			mButtonCheckFile.Click += delegate {

				StartActivity(typeof(FileListShow));
			
			};


		}

		public override void OnBackPressed ()
		{
			//base.OnBackPressed(); 
			MoveTaskToBack(true);
			//base.OnDestroy ();
			//Java.Lang.JavaSystem.Exit (0);
		}

	}
}


