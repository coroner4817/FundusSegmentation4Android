
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Database;
using Android.Net;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using Java.Lang;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Android.Content.Res;
using Java.IO;
using Android.Content.PM;
using System.Threading;
using Android.Text.Format;
using Java.Util;

namespace Application
{
	[Activity (Label = "ImageProcessing",ScreenOrientation = ScreenOrientation.Portrait)]			
	public class ImageProcessing : Activity
	{
		public static readonly int PickImageId=1000;
		private PaintView _paintView;
		private PaintView _paintView2;
		//int count=0;


		private TextView txtUploadFirstName;
		private TextView txtUploadLastName;
		private TextView txtUploadAge;
		private TextView txtUploadGender;
		private TextView logTextView;
//		private TextView logTextView2;
//		private TextView logTextView3;
//		private TextView logTextView4;
//		private TextView logTextView5;
//		private TextView logTextView6;
//		private TextView logTextView7;
//		private TextView logTextView8;
	

		private Android.Net.Uri uri;

		//private Java.Util.Timer timer = new Java.Util.Timer();
		//private TimerTask timerTask;


		private int socketStatus=0;

		private Bitmap _bitmap;
		private Bitmap FinalBitmap;


		private Button GetVS;
		private Button _draw;
		private Button _erase;
		private Button _Dismiss;
		private Button _Save;

		private string msg;
		private string msgName;
		//private System.String status;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.upload_picture);

			_paintView = FindViewById<PaintView> (Resource.Id.paintView1);
			_paintView2 = FindViewById<PaintView> (Resource.Id.paintView2);
			GetVS = FindViewById<Button> (Resource.Id.btnImageProcess);
			_draw = FindViewById<Button> (Resource.Id.btnEnableDrawing);
			_erase = FindViewById<Button> (Resource.Id.btnEnableErasing);
			_Dismiss = FindViewById<Button> (Resource.Id.btnVSdismiss);
			_Save = FindViewById<Button> (Resource.Id.btnSave);

			txtUploadFirstName = FindViewById<TextView> (Resource.Id.txtUploadPicFirstName);
			txtUploadLastName = FindViewById<TextView> (Resource.Id.txtUploadPicLastName);
			txtUploadAge = FindViewById<TextView> (Resource.Id.txtUploadPicAge);
			txtUploadGender = FindViewById<TextView> (Resource.Id.txtUploadPicGender);
			logTextView = FindViewById<TextView> (Resource.Id.LOGtextView);
//			logTextView2 = FindViewById<TextView> (Resource.Id.LOGtextView2);
//			logTextView3 = FindViewById<TextView> (Resource.Id.LOGtextView3);
//			logTextView4 = FindViewById<TextView> (Resource.Id.LOGtextView4);
//			logTextView5 = FindViewById<TextView> (Resource.Id.LOGtextView5);
//			logTextView6 = FindViewById<TextView> (Resource.Id.LOGtextView6);
//			logTextView7 = FindViewById<TextView> (Resource.Id.LOGtextView7);
//			logTextView8 = FindViewById<TextView> (Resource.Id.LOGtextView8);


			logTextView.SetText ("Client log:\n", TextView.BufferType.Normal);

		

			//var myHandler = new Handler ();

			if (Intent.GetStringExtra ("firstname1") != null)
			{
				txtUploadFirstName.Text ="          " + "FirstName: " + Intent.GetStringExtra ("firstname1");
				txtUploadLastName.Text ="          " + "LastName: " + Intent.GetStringExtra ("lastname1");
				txtUploadAge.Text = "          " + "Age: " +Intent.GetStringExtra ("age1");
				txtUploadGender.Text = "          " + "Gender: " +Intent.GetStringExtra ("gender1");

				msg = string.Format ("Select the Fundus Image for {0} {1}", Intent.GetStringExtra ("firstname1"), Intent.GetStringExtra ("lastname1"));
				msgName = string.Format ("{0}{1}", Intent.GetStringExtra ("firstname1"), Intent.GetStringExtra ("lastname1"));
			}

			Intent = new Intent ();
			Intent.SetType ("image/*");
			Intent.SetAction (Intent.ActionGetContent);
			StartActivityForResult (Intent.CreateChooser (Intent, "Select Picture"), PickImageId);
		

			Toast mToast = Toast.MakeText (this, msg, ToastLength.Short);
			mToast.Show ();


			GetVS.Click += delegate {


				if(socketStatus==1)
				{


					_paintView2._Bmp=FinalBitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);
					_paintView2._Bmp=Bitmap.CreateScaledBitmap(_paintView2._Bmp,_paintView2.w,_paintView2.h,false);

					_paintView2._Canvas = new Canvas(_paintView2._Bmp);
					_paintView2.SetImageBitmap (_paintView2._Bmp);

				}

				else{


					RunOnUiThread(()=>logTextView.Append("Client is running...\n"));


					long tStart = Java.Lang.JavaSystem.CurrentTimeMillis();

					StartClient();

					if(FinalBitmap==null)
					{
						Toast.MakeText(this, "Please connect to the Internet and try again.", ToastLength.Short).Show();
					}

					else{

						_paintView2._Bmp=FinalBitmap.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);
						_paintView2._Bmp=Bitmap.CreateScaledBitmap(_paintView2._Bmp,_paintView2.w,_paintView2.h,false);


						_paintView2._Canvas = new Canvas(_paintView2._Bmp);
						_paintView2.SetImageBitmap (_paintView2._Bmp);


						long tEnd = Java.Lang.JavaSystem.CurrentTimeMillis();
						long tDelta = tEnd - tStart;
						double elapsedSeconds = tDelta / 1000.0;
						//RunOnUiThread(()=>logTextView.Text = logTextView.Text +  string.Format ("Total time elpased is: {0} second\n", elapsedSeconds.ToString ()));
						SetLogTextView( string.Format ("Total time elpased is: {0} second\n", elapsedSeconds.ToString ()));
					}

				}

			};


			_draw.Click += delegate {

				if(socketStatus==0)
				{
					Toast.MakeText(this, "Cannot enable drawing before click Get VS!", ToastLength.Short).Show();

				}

				if(socketStatus==1)
				{
					_paintView2.status=1;
				}
					

			};

			_erase.Click += delegate {

				if(socketStatus==0)
				{
					Toast.MakeText(this, "Cannot enable erasing before click Get VS!", ToastLength.Short).Show();

				}

				if(socketStatus==1)
				{
					_paintView2.status=0;
				}

			};

			_Dismiss.Click += delegate {

				_paintView2.SetImageBitmap(null);

			};

			_Save.Click += delegate {

				if(socketStatus==0)
				{
					Toast.MakeText(this, "Cannot enable saving before click Get VS!", ToastLength.Short).Show();

				}
				else{

					Bitmap bamp=overlay(_paintView._Bmp,_paintView2._Bmp);

					ExportBitmapAsPNG(bamp);

					Toast.MakeText(this, "The edit Image has already been save at the FundusSegmentation folder", ToastLength.Long).Show();

				}
					
			};

		}
	

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{

			if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null)) 
			{
				uri = data.Data;
				_paintView.SetImageURI (uri);

				string path = GetPathToImage (uri);
				Toast.MakeText (this, path, ToastLength.Long);
			}

			if (uri != null) {


				_paintView._Bmp = getThumbnailBitmap (GetPathToImage (uri), 500);

				_paintView._Bmp= _paintView._Bmp.Copy(Android.Graphics.Bitmap.Config.Argb8888, true);

				_paintView._Bmp=Bitmap.CreateScaledBitmap(_paintView._Bmp, _paintView.w, _paintView.h, false);
				_paintView._Canvas = new Canvas(_paintView._Bmp);
				_paintView.SetImageBitmap (_paintView._Bmp);

				uri.Dispose ();

				_paintView2.SetImageBitmap (null);

				_bitmap = _paintView._Bmp;
			}

		}

		private string GetPathToImage(Android.Net.Uri uri)
		{
			string path = null;

			string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };

			using (ICursor cursor = ManagedQuery (uri, projection, null, null, null))
			{
				if (cursor != null) 
				{
					int columnIndex = cursor.GetColumnIndexOrThrow (Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
					cursor.MoveToFirst ();
					path = cursor.GetString (columnIndex);
				}
			}

			return path;
		}


		public void StartClient() {

			//start client and sent a image to server


			// Connect to a remote device.
			try {

				#region setup socket
				Int32 port = 5037;
				IPAddress ipAddress = IPAddress.Parse("192.168.43.226");
				//IPAddress ipAddress = IPAddress.Parse("192.168.56.1");
				IPEndPoint ipe = new IPEndPoint(ipAddress, port); 

				// Create a TCP/IP  socket.
				Socket sock = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, ProtocolType.Tcp); 
				#endregion

				// Connect the socket to the remote endpoint. Catch any errors.
				try {

					//sock.Connect(ipe);

					try
					{
						sock.Connect(ipe);
					}
					catch (System.Exception ex)
					{
						if ((ex is SocketException) && ((SocketException)ex).ErrorCode == 10035) 
						{
							// Waiting for connection
							int time = 0;
							while (time < 2000) // If it was lower than 1 second
							{
								// Do what you want
								if (sock.Connected) // If connected then break
									break;
								System.Threading.Thread.Sleep(50); // Wait 50milisec
								time += 50;
							}
						}
						else
						{
							System.Console.WriteLine("Failed");
						}
					}


				


					//RunOnUiThread(()=>logTextView.Text= logTextView.Text + string.Format("Socket connected to {0}\n",sock.RemoteEndPoint.ToString()));

					SetLogTextView(string.Format("Socket connected to {0}\n",sock.RemoteEndPoint.ToString()));




					//替换为data from gallery
					#region read image into byte buffer

					if(_bitmap!=null)
					{


						var ms = new MemoryStream();
						_bitmap.Compress(Bitmap.CompressFormat.Png, 0, ms);
						var imageBytes = ms.ToArray();
						byte[] nullByte=Encoding.ASCII.GetBytes("\0");

						byte[] ImageByteForSend = new byte[imageBytes.Length + nullByte.Length];
						imageBytes.CopyTo(ImageByteForSend, 0);
						nullByte.CopyTo(ImageByteForSend,imageBytes.Length);

						//logTextView.Text= logTextView.Text + string.Format ("Start send image to server\n");
						SetLogTextView(string.Format ("Start send image to server\n"));

						#endregion


						#region Send Image Size
						byte[] imageSize = Encoding.ASCII.GetBytes(ImageByteForSend.Length.ToString()+"\0");
						int imageSizeSend=sock.Send(imageSize);
						#endregion



						#region receive confirm from server
						byte[] recvComfirm=new byte[100];
						int recvConfrimFromServer=sock.Receive(recvComfirm);
						string status=Encoding.ASCII.GetString(recvComfirm,0,recvConfrimFromServer);
						#endregion


						#region Send Image

						int sentByteCount=0;
						int off=0;
						int remainSendBufferCount=ImageByteForSend.Length;
						do{

							if(remainSendBufferCount>10240)
							{
								byte[] bytesPerBufferLong = new byte[10240];
								Array.Copy(ImageByteForSend,off,bytesPerBufferLong,0,10240);


								sentByteCount=sock.Send(bytesPerBufferLong);

								off+=sentByteCount;

								remainSendBufferCount=remainSendBufferCount-sentByteCount;

								System.Console.WriteLine("{0} bytes has been send",off.ToString());

							}

							//最后一次发送时的buffer size不定
							if(remainSendBufferCount<10240)
							{
								byte[] bytesPerBufferSmall = new byte[remainSendBufferCount];
								Array.Copy(ImageByteForSend,off,bytesPerBufferSmall,0,remainSendBufferCount);


								sentByteCount=sock.Send(bytesPerBufferSmall);

								off+=sentByteCount;
								remainSendBufferCount=remainSendBufferCount-remainSendBufferCount;
								System.Console.WriteLine("{0} bytes has been send",off.ToString());
							}

						}while(off<ImageByteForSend.Length);

						#endregion

						//RunOnUiThread(()=>logTextView.Text= logTextView.Text + string.Format ("Successfully send the image to server\n"));

						SetLogTextView(string.Format ("Successfully send the image to server\n"));
					}

					////////////////////////


					#region receive confirm from server
					byte[] opencvstartComfirm=new byte[100];
					int opencvStartFromServer=sock.Receive(opencvstartComfirm);
					string opencvStart=Encoding.ASCII.GetString(opencvstartComfirm,0,opencvStartFromServer);
					//RunOnUiThread(()=>logTextView.Text= logTextView.Text+ opencvStart+string.Format("\n"));
					SetLogTextView(opencvStart+string.Format("\n"));
					#endregion

					#region receive confirm from server
					byte[] opencvFinish=new byte[100];
					int opencvFinFromServer=sock.Receive(opencvFinish);
					string opencvfin=Encoding.ASCII.GetString(opencvFinish,0,opencvFinFromServer);
					//RunOnUiThread(()=>logTextView.Text= logTextView.Text + opencvfin+string.Format("\n"));
					SetLogTextView(opencvfin+string.Format("\n"));
					#endregion




					#region receive image size
					byte[] recvSize=new byte[100];
					int recvSizeFromServer=sock.Receive(recvSize);
					string sizeString=Encoding.ASCII.GetString(recvSize,0,recvSizeFromServer);
					int SizeRecv = int.Parse(sizeString);
					#endregion

					#region send confirm signal
					byte[] confirmSignal = Encoding.ASCII.GetBytes("Get the size, server please send the image"+"\0");
					int ConfirmSend=sock.Send(confirmSignal);
					#endregion

					#region receive image

					byte[] recvImage=new byte[SizeRecv];
					int totalSizeWrite=0;

					do{
						byte[] bytesPerbufferrecv=new byte[10240];
						int bytesRec = sock.Receive(bytesPerbufferrecv);

						Array.Copy(bytesPerbufferrecv,0,recvImage,totalSizeWrite,bytesRec);

						totalSizeWrite=totalSizeWrite+bytesRec;
					}while(totalSizeWrite<SizeRecv);

					#endregion

					#region set the recv image to imageView
					Bitmap recvByte2Bitmap=BitmapFactory.DecodeByteArray(recvImage,0,recvImage.Length);
					
						if(recvByte2Bitmap!=null)
						{
							Bitmap newRecv=Bitmap.CreateScaledBitmap(recvByte2Bitmap,_bitmap.Width,_bitmap.Height,true);

							FinalBitmap=newRecv;
						}
					

					#endregion

					//RunOnUiThread(()=>logTextView.Text=logTextView.Text + string.Format ("Successfully receive the image from server\n"));
					SetLogTextView(string.Format ("Successfully receive the image from server\n"));

					// Release the socket.
					sock.Shutdown(SocketShutdown.Both);
					sock.Close();

					socketStatus=1;


				} catch (ArgumentNullException ane) {
					System.Console.WriteLine("ArgumentNullException : {0}",ane.ToString());
					//logTextView.Text= logTextView.Text + string.Format ("Oops! An Error Happened!\n");
					SetLogTextView(string.Format ("Oops! An Error Happened!\n"));
				} catch (SocketException se) {
					System.Console.WriteLine("SocketException : {0}",se.ToString());
					//logTextView.Text= logTextView.Text + string.Format ("Oops! An Error Happened!\n");
					SetLogTextView(string.Format ("Oops! An Error Happened!\n"));
				} catch (System.Exception e) {
					System.Console.WriteLine("Unexpected exception : {0}", e.ToString());
					//logTextView.Text= logTextView.Text + string.Format ("Oops! An Error Happened!\n");
					SetLogTextView(string.Format ("Oops! An Error Happened!\n"));
				}

			} catch (System.Exception e) {
				System.Console.WriteLine( e.ToString());
				//logTextView.Text= logTextView.Text + string.Format ("Oops! An Error Happened!\n");
				SetLogTextView(string.Format ("Oops! An Error Happened!\n"));
			}
		}

		public byte[] addNullToTheLastOfBytes(byte[] original)
		{
			byte[] nullByte=Encoding.ASCII.GetBytes("\0");
			byte[] bytesArrayAddedNull = new byte[original.Length + nullByte.Length];
			original.CopyTo(bytesArrayAddedNull, 0);
			nullByte.CopyTo(bytesArrayAddedNull,original.Length);

			return bytesArrayAddedNull;
		}

		public Bitmap overlay(Bitmap bmp1, Bitmap bmp2) {
			Bitmap bmOverlay = Bitmap.CreateScaledBitmap(bmp1,bmp1.Width,bmp1.Height,true);
			bmOverlay = bmOverlay.Copy (Bitmap.Config.Argb8888, true);

			Canvas canvas = new Canvas(bmOverlay);

			bmp1 = bmp1.Copy (Bitmap.Config.Argb8888, true);
			bmp2 = bmp2.Copy (Bitmap.Config.Argb8888, true);

			canvas.DrawBitmap(bmp1, new Matrix(), null);
			canvas.DrawBitmap(bmp2, 0, 0, null);
			return bmOverlay;
		}


		public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
		{
			// Raw height and width of image
			float height = options.OutHeight;
			float width = options.OutWidth;
			double inSampleSize = 1D;

			if (height > reqHeight || width > reqWidth)
			{
				int halfHeight = (int)(height / 2);
				int halfWidth = (int)(width / 2);

				// Calculate a inSampleSize that is a power of 2 - the decoder will use a value that is a power of two anyway.
				while ((halfHeight / inSampleSize) > reqHeight && (halfWidth / inSampleSize) > reqWidth)
				{
					inSampleSize *= 2;
				}
			}

			return (int)inSampleSize;
		}

		private Bitmap getThumbnailBitmap(System.String path, int thumbnailSize) {
			Bitmap bitmap;
			BitmapFactory.Options bounds = new BitmapFactory.Options();
			bounds.InJustDecodeBounds = true;
			BitmapFactory.DecodeFile(path, bounds);
			if ((bounds.OutWidth == -1) || (bounds.OutHeight == -1)) {
				bitmap = null;
			}
			int originalSize = (bounds.OutHeight > bounds.OutWidth) ? bounds.OutHeight : bounds.OutWidth;
			BitmapFactory.Options opts = new BitmapFactory.Options();
			opts.InSampleSize = originalSize / thumbnailSize;
			bitmap = BitmapFactory.DecodeFile(path, opts);
			return bitmap;
		}
	

		void ExportBitmapAsPNG(Bitmap bitmap)
		{
			Time now = new Time();
			now.SetToNow();

			var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/FundusSegmentation";
			var filePath = System.IO.Path.Combine(sdCardPath, msgName+"EditAt"+now.Year.ToString()+(now.Month+1).ToString()+now.MonthDay.ToString()+now.Hour.ToString()+now.Minute.ToString()+".jpg");
			var stream = new FileStream(filePath, FileMode.Create);
			bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);


			stream.Close();
		}


		public override void OnBackPressed ()
		{
			base.OnBackPressed(); 

			if (_paintView._Bmp != null) {
				_paintView._Bmp.Recycle ();
				_paintView._Bmp = null;
			}

			if (_paintView2._Bmp != null) {
				_paintView2._Bmp.Recycle ();
				_paintView2._Bmp = null;
			}

			base.OnDestroy ();

			StartActivity(new Intent(this, typeof(MainActivity)));
			Finish();
		}


//		public void debugMsg(string msg) 
//		{
//			string str = msg;
//			runOnUiThread(new Runnable() {
//
//				public Override void run() {
//					mInfo.setText(str);
//				}
//			});
//		}

//		public void SetLogTextView(string msg)
//		{
//			using (var h = new Handler (Looper.MainLooper))
//				h.Post (() => {
//					/* invoked on UI thread */
//
//				logTextView.SetText(logTextView.Text+msg,TextView.BufferType.Normal);
//
//				});
//
//			timerTask = new TimerTask() { 
//
//				public override void run() { 
//					//refresh your textview
//
//					logTextView.SetText(logTextView.Text+msg,TextView.BufferType.Normal);
//				}
//			};
//		
//
//		}

		private void SetLogTextView (string msg)
		{

			RunOnUiThread (() => logTextView.Append(msg) );

		}

//		private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
//		{
//
//		}

	}  
}  

