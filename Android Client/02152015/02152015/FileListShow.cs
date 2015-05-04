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
using System.Xml.Serialization;
using System.IO;
using Android.Content.PM;

namespace Application
{
	[Activity (Label = "FileListShow",ScreenOrientation = ScreenOrientation.Portrait)]
	public class FileListShow : Activity
	{
		public List<Friend> mFriends = new List<Friend>();
		public List<Friend> mFriendsTemp = new List<Friend>();

		public ListView mListView;
		private EditText mSearch;
		private LinearLayout mContainer;
		private bool mAnimateDown;
		private bool mIsAnimating;
		public FriendsAdapter mAdapter;

		private TextView mTxtHeaderFirstName;
		private TextView mTxtHeaderLastName;
		private TextView mTxtHeaderAge;
		private TextView mTxtHeaderGender;

		private bool mFirstNameAscending;
		private bool mLastNameAscending;
		private bool mAgeAscending;
		private bool mGenderAscending;

		private string firstname;
		private string lastname;
		private string age;
		private string gender;

		private string filepath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath+"/FundusSegmentation/PatientList.xml";



		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.FileList);


			mListView = FindViewById<ListView>(Resource.Id.listView);
			mSearch = FindViewById<EditText> (Resource.Id.etSearch);
			mContainer = FindViewById<LinearLayout> (Resource.Id.llContainer);

			mTxtHeaderFirstName = FindViewById<TextView> (Resource.Id.txtHeaderFirstName);
			mTxtHeaderLastName = FindViewById<TextView> (Resource.Id.txtHeaderLastName);
			mTxtHeaderAge = FindViewById<TextView> (Resource.Id.txtHeaderAge);
			mTxtHeaderGender = FindViewById<TextView> (Resource.Id.txtHeaderGender);


			mTxtHeaderFirstName.Click += mTxtHeaderFirstName_Click;
			mTxtHeaderLastName.Click += mTxtHeaderLastName_Click;
			mTxtHeaderAge.Click += mTxtHeaderAge_Click;
			mTxtHeaderGender.Click += mTxtHeaderGender_Click;


			mSearch.Alpha = 0;
			mContainer.BringToFront();

			mSearch.TextChanged += mSearch_TextChanged;

			//mFriends = new List<Friend>();
//			mFriends.Add(new Friend { FirstName = "Bob", LastName = "Smith", Age = "33", Gender = "Male" });
//			mFriends.Add(new Friend { FirstName = "Tom", LastName = "Smith", Age = "45", Gender = "Male" });
//			mFriends.Add(new Friend { FirstName = "Julie", LastName = "Smith", Age = "2020", Gender = "Unknown" });
//			mFriends.Add(new Friend { FirstName = "Molly", LastName = "Smith", Age = "21", Gender = "Female" });
//			mFriends.Add(new Friend { FirstName = "Joe", LastName = "Lopez", Age = "22", Gender = "Male" });
//			mFriends.Add(new Friend { FirstName = "Ruth", LastName = "White", Age = "81", Gender = "Female" });
//			mFriends.Add(new Friend { FirstName = "Sally", LastName = "Johnson", Age = "54", Gender = "Female" });
//			mFriends.Add(new Friend { FirstName = "Mark", LastName = "Smith", Age = "43", Gender = "Male" });
//			mFriends.Add(new Friend { FirstName = "Peter", LastName = "Smith", Age = "35", Gender = "Male" });
//			mFriends.Add(new Friend { FirstName = "Kirk", LastName = "Smith", Age = "222", Gender = "Unknown" });
//			mFriends.Add(new Friend { FirstName = "Matt", LastName = "Smith", Age = "20", Gender = "Female" });
//			mFriends.Add(new Friend { FirstName = "John", LastName = "Lopez", Age = "2", Gender = "Male" });
//			mFriends.Add(new Friend { FirstName = "Rubio", LastName = "White", Age = "1", Gender = "Female" });
//			mFriends.Add(new Friend { FirstName = "Susan", LastName = "Johnson", Age = "14", Gender = "Female" });

			Java.IO.File folder = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/FundusSegmentation");
			Boolean success = true;
			if (!folder.Exists()) {
				success = folder.Mkdir();

				if (success) {
					// Do something on success
					mFriends.Add(new Friend { FirstName = "Bob", LastName = "Smith", Age = "33", Gender = "Male" });

					#region Seriliazing
					//var filepath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath+"/VesselSegmentationCache/PatientList.xml";
					XmlSerializer serializer = new XmlSerializer(typeof(List<Friend>));//initialises the serialiser
					Stream writer = new FileStream(filepath, FileMode.Create);//initialises the writer

					serializer.Serialize(writer, mFriends);//Writes to the file
					writer.Close ();//Closes the writer
					#endregion


				} 
			}


			#region Deserializing List

			XmlSerializer serializerDeserializing = new XmlSerializer(typeof(List<Friend>));//initialises the serialiser
			Stream reader = new FileStream (filepath, FileMode.Open); //Initialises the reader
			 

			mFriends = (List<Friend>)serializerDeserializing.Deserialize (reader); //reads from the xml file and inserts it in this variable
			reader.Close (); //closes the reader

			#endregion



			if (Intent.GetStringExtra("firstname")!=null) 
			{
				firstname=Intent.GetStringExtra("firstname");
				lastname=Intent.GetStringExtra("lastname");
				age=Intent.GetStringExtra("age");
				gender=Intent.GetStringExtra("gender");

				mFriends.Add (new Friend{ FirstName = firstname, LastName = lastname, Age = age, Gender = gender });

				#region Seriliazing
				//var filepath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath+"/VesselSegmentationCache/PatientList.xml";
				XmlSerializer serializer = new XmlSerializer(typeof(List<Friend>));//initialises the serialiser
				Stream writer = new FileStream(filepath, FileMode.Create);//initialises the writer

				serializer.Serialize(writer, mFriends);//Writes to the file
				writer.Close ();//Closes the writer
				#endregion
			}

			mFriendsTemp = mFriends.ToList ();


			mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, mFriends);
			mListView.Adapter = mAdapter;



			mListView.ItemClick += mListView_Click;

			mListView.ItemLongClick += mListView_LongClick;

		}

		void mListView_LongClick (object sender, AdapterView.ItemLongClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = mFriendsTemp[e.Position];

			// On "Call" button click, try to dial phone number.
			var callDialog = new AlertDialog.Builder(this);
			callDialog.SetMessage(string.Format("Are you sure that you want to permanently delete {0} {1}'s file?",t.FirstName,t.LastName));
			callDialog.SetNeutralButton("Delete", delegate {

				mFriends.Remove(t);

				XmlSerializer serializer = new XmlSerializer(typeof(List<Friend>));//initialises the serialiser
				Stream writer = new FileStream(filepath, FileMode.Create);//initialises the writer

				serializer.Serialize(writer, mFriends);//Writes to the file
				writer.Close ();//Closes the writer

				mAdapter = new FriendsAdapter(this, Resource.Layout.row_friend, mFriends);
				mListView.Adapter = mAdapter;



			});
			callDialog.SetNegativeButton("Cancel", delegate { });

			// Show the alert dialog to the user and wait for response.
			callDialog.Show();

		}

		void mListView_Click (object sender, AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = mFriendsTemp[e.Position];

			var intent = new Intent(this,typeof(ImageProcessing));
			intent.PutExtra("firstname1",t.FirstName);
			intent.PutExtra("lastname1",t.LastName);
			intent.PutExtra("age1",t.Age);
			intent.PutExtra("gender1",t.Gender);

			//mFriendsTemp = mFriends.ToList ();


			StartActivity(intent);

		}


		void mTxtHeaderFirstName_Click (object sender, EventArgs e)
		{
			List<Friend> filteredFriend;

			if (!mFirstNameAscending) {
				filteredFriend = (from friend in mFriends
					orderby friend.FirstName
					select friend).ToList<Friend> ();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}
			else
			{

				filteredFriend=(from friend in mFriends
					orderby friend.FirstName descending
					select friend).ToList<Friend>();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}

			mFriendsTemp = filteredFriend.ToList ();

			mListView.ItemClick += mListView_Click;

			mListView.ItemLongClick += mListView_LongClick;

			mFirstNameAscending=!mFirstNameAscending;
		}

		void mTxtHeaderLastName_Click (object sender, EventArgs e)
		{
			List<Friend> filteredFriend;

			if (!mLastNameAscending) {
				filteredFriend = (from friend in mFriends
					orderby friend.LastName
					select friend).ToList<Friend> ();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}
			else
			{

				filteredFriend=(from friend in mFriends
					orderby friend.LastName descending
					select friend).ToList<Friend>();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}

			mFriendsTemp = filteredFriend.ToList ();

			mListView.ItemClick += mListView_Click;

			mListView.ItemLongClick += mListView_LongClick;

			mLastNameAscending=!mLastNameAscending;
		}

		void mTxtHeaderAge_Click (object sender, EventArgs e)
		{
			List<Friend> filteredFriend;

			if (!mAgeAscending) {
				filteredFriend = (from friend in mFriends
					orderby friend.Age
					select friend).ToList<Friend> ();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}
			else
			{

				filteredFriend=(from friend in mFriends
					orderby friend.Age descending
					select friend).ToList<Friend>();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}

			mFriendsTemp = filteredFriend.ToList ();

			mListView.ItemClick += mListView_Click;

			mListView.ItemLongClick += mListView_LongClick;

			mAgeAscending=!mAgeAscending;
		}

		void mTxtHeaderGender_Click (object sender, EventArgs e)
		{
			List<Friend> filteredFriend;

			if (!mGenderAscending) {
				filteredFriend = (from friend in mFriends
					orderby friend.Gender
					select friend).ToList<Friend> ();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}
			else
			{

				filteredFriend=(from friend in mFriends
					orderby friend.Gender descending
					select friend).ToList<Friend>();

				mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, filteredFriend);
				mListView.Adapter = mAdapter;
			}

			mFriendsTemp = filteredFriend.ToList ();

			mListView.ItemClick += mListView_Click;

			mListView.ItemLongClick += mListView_LongClick;

			mGenderAscending=!mGenderAscending;
		}



		void mSearch_TextChanged (object sender, Android.Text.TextChangedEventArgs e)
		{
			List<Friend> searchFriends = (from friend in mFriends
				where friend.FirstName.Contains (mSearch.Text,StringComparison.OrdinalIgnoreCase) || 
				friend.LastName.Contains (mSearch.Text,StringComparison.OrdinalIgnoreCase) ||
				friend.Age.Contains (mSearch.Text,StringComparison.OrdinalIgnoreCase) ||
				friend.Gender.Contains (mSearch.Text,StringComparison.OrdinalIgnoreCase)
				select friend).ToList<Friend> ();
			mAdapter = new FriendsAdapter (this, Resource.Layout.row_friend, searchFriends);
			mListView.Adapter = mAdapter;

			mFriendsTemp = searchFriends.ToList ();

			mListView.ItemClick += mListView_Click;

			mListView.ItemLongClick += mListView_LongClick;

		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.actionbar, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) 
			{
			case Resource.Id.search:
				mSearch.Visibility = ViewStates.Visible;
				if (mIsAnimating)
				{
					return true;
				}

				if (!mAnimateDown) {
					//Listview is up
					MyAnimation anim = new MyAnimation (mListView, mListView.Height - mSearch.Height);
					anim.Duration = 500;
					mListView.StartAnimation (anim);
					anim.AnimationStart += anim_AnimationStartDown;
					anim.AnimationEnd += anim_AnimationEndDown;
					mContainer.Animate ().TranslationYBy (mSearch.Height).SetDuration (500).Start ();

				} 

				else {
					//listview is down
					MyAnimation anim = new MyAnimation (mListView, mListView.Height + mSearch.Height);
					anim.Duration = 500;
					mListView.StartAnimation (anim);
					anim.AnimationStart += anim_AnimationStartUp;
					anim.AnimationEnd += anim_AnimationEndUp;
					mContainer.Animate ().TranslationYBy (-mSearch.Height).SetDuration (500).Start ();

				}

				mAnimateDown = !mAnimateDown;

				return true;

			default:

				return base.OnOptionsItemSelected (item);
			}
		}

		void anim_AnimationEndDown (object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e)
		{
			mIsAnimating = false;
			//			mSearch.RequestFocus ();
			//			InputMethodManager inputManager = (InputMethodManager)this.GetSystemService (Context.InputMethodService);
			//			inputManager.ShowSoftInput (mSearch,InputMethodManager.ShowForced);
		}

		void anim_AnimationEndUp (object sender, Android.Views.Animations.Animation.AnimationEndEventArgs e)
		{
			mIsAnimating = false;
			mSearch.ClearFocus ();
			InputMethodManager inputManager = (InputMethodManager)this.GetSystemService (Context.InputMethodService);
			inputManager.HideSoftInputFromWindow (this.CurrentFocus.WindowToken, HideSoftInputFlags.NotAlways);
			//mSearch.Text = null;
		}

		void anim_AnimationStartDown (object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e)
		{
			mIsAnimating = true;
			mSearch.Animate ().AlphaBy (1.0f).SetDuration (500).Start ();

		}

		void anim_AnimationStartUp (object sender, Android.Views.Animations.Animation.AnimationStartEventArgs e)
		{
			mIsAnimating = true;
			mSearch.Animate ().AlphaBy (-1.0f).SetDuration (300).Start ();
		}


	}
}

