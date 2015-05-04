using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;


namespace Application
{
	public class OnSignUpEventArgs:EventArgs
	{
		private string mFirstName;
		private string mLastName;
		private string mAge;
		private string mGender;

		public string FirstName
		{
			get { return mFirstName;}
			set	{ mFirstName = value;}
		}
		public string LastName
		{
			get{ return mLastName;}
			set{ mLastName = value;}

		}
		public string Age
		{
			get{ return mAge;}
			set{ mAge = value;}
		}

		public string Gender
		{
			get { return mGender;}
			set { mGender = value;}
		}

		public OnSignUpEventArgs(string firstName,string lastName,string age,string gender):base()
		{
			FirstName = firstName;
			LastName = lastName;
			Age = age;
			Gender = gender;
		}
	}





	public class dialogCreate:DialogFragment
	{
		private EditText mTxtFirstName;
		private EditText mTxtLastName;
		private EditText mTxtAge;
		private EditText mTxtGender;
		private Button mBtnSignUp;
		private TextView mErrorText;

		//public event EventHandler<OnSignUpEventArgs> mOnSignUpComplete;


		public override View OnCreateView(LayoutInflater inflater, ViewGroup container,Bundle savedInstanceState)
		{
			base.OnCreateView (inflater, container, savedInstanceState);

			var view = inflater.Inflate (Resource.Layout.dialogCreatFile, container, false);

			mTxtFirstName = view.FindViewById<EditText> (Resource.Id.txtFirstName);
			mTxtLastName = view.FindViewById<EditText> (Resource.Id.txtLastName);
			mTxtAge = view.FindViewById<EditText> (Resource.Id.txtAge);
			mTxtGender = view.FindViewById<EditText> (Resource.Id.txtGender);
			mBtnSignUp = view.FindViewById<Button> (Resource.Id.btnCreate);
			mErrorText = view.FindViewById<TextView> (Resource.Id.txtError);


			mBtnSignUp.Click +=  (object sender, EventArgs e) => {
				//User has click the sign up btn

				if((mTxtFirstName.Text=="")||(mTxtLastName.Text=="")||(mTxtAge.Text=="")||(mTxtGender.Text=="")){

					//Toast.MakeText(mContext, "Patient's information is inclompleted!", ToastLength.Short).Show();
					mErrorText.Text=string.Format("Patient's information is inclomplete!");
				}

				else{
					var intent = new Intent(Activity,typeof(FileListShow));
					intent.PutExtra("firstname",mTxtFirstName.Text);
					intent.PutExtra("lastname",mTxtLastName.Text);
					intent.PutExtra("age",mTxtAge.Text);
					intent.PutExtra("gender",mTxtGender.Text);

					StartActivity(intent);

					this.Dismiss();
				}

			};


			return view;

		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			Dialog.Window.RequestFeature (WindowFeatures.NoTitle); //set the title bar to invisable
			base.OnActivityCreated (savedInstanceState);
			Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set teh animation
		}
	}
}

