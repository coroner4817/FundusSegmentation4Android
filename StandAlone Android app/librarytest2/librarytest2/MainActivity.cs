using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using librarytest2;
using Android.Graphics;

using Org.Opencv.Android;
using Org.Opencv.Core;
using Org.Opencv.Utils;
using Org.Opencv.Imgproc;
using Org.Opencv.Highgui;
using Java.IO;


namespace librarytest2
{
	[Activity (Label = "librarytest2", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		private FundusSegment mFundusSegment=new FundusSegment();



		public MainActivity()
		{
			if (!OpenCVLoader.InitDebug ()) {
				System.Console.WriteLine ("Init Opencv Failed");
			}
		}


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			Button button = FindViewById<Button> (Resource.Id.myButton);
			ImageView iView = FindViewById<ImageView> (Resource.Id.imageView1);
			iView.SetImageResource (Resource.Drawable.fundus);


			#region testCode
			//float t=0.0f;
			//t=MatlabFunctionsTest.mlab_imadjust ();
			// Get our button from the layout resource,
			// and attach an event to it
			//Mat test = new Mat (5, 5, CvType.Cv32f, s);
//			test.Put (0, 0, 0.1f);
//			test.Put (0, 1, 0.3f);
//			test.Put (1, 0, 0.4f);
//			test.Put (1, 1, 0.5f);

//			test=MatlabFunctionsTest.mlab_imadjust (test);
//			float[] temp = new float[1];
//			byte[] tempInt = new byte[1];
//			test.Get (1, 0, temp);
//			t = temp [0];


//			test = MatlabFunctionsTest.mlab_strelLine (7, -45.0f);
//			byte[] temp = new byte[1];
//			test.Get (3, 3, temp);
//
//			Mat testMat = new Mat (500, 500, CvType.Cv32f, s);
//			float[] temp = new float[1];
//
//			for (int i = 0; i < 500; i++)
//				for (int j = 0; j < 500; j++) {
//					testMat.Put (i, j, 0);
//				
//				}
//
//			testMat = write1ToMat (5, 6, testMat);
//			testMat = write1ToMat (50, 60, testMat);
//			testMat = write1ToMat (150, 160, testMat);
//			testMat = write1ToMat (250, 260, testMat);
//			testMat = write1ToMat (354, 56, testMat);
//			testMat = write1ToMat (35, 26, testMat);
//			testMat = write1ToMat (225, 236, testMat);
//			testMat = write1ToMat (189, 67, testMat);
//			testMat = write1ToMat (400, 32, testMat);
//			testMat = write1ToMat (400, 400, testMat);
//
//			testMat=MatlabFunctionsTest.mlab_bwareaopen (testMat, 50);
//			float sum = 0;
//			for (int i = 0; i < 500; i++)
//				for (int j = 0; j < 500; j++) {
//					testMat.Get (i, j, temp);
//					sum = sum + temp [0];
//				}
			#endregion
			Scalar mScalar = new Scalar (0);
//			Mat testa = new Mat (2, 2, CvType.Cv8uc1, mScalar);
//			Mat testb = new Mat (1,1, CvType.Cv8uc1, mScalar);
//
//			testa.Put (0, 0, 1);
//			testa.Put (0, 1, 1);
//			testa.Put (1, 0, 1);
//			testa.Put (1, 1, 1);
//
//			testb.Put (0, 0, 5);
//
//
//			var tupleReturn = tupleTest1.getTuple (testa, testb);
//			testa = tupleReturn.Item1;
//			testb = tupleReturn.Item2;

//			Mat matTest = new Mat (500, 500, CvType.Cv64fc1, mScalar);
//			double[] temp = new double[1];
//			matTest.Put (0, 0, 1.2);
//			matTest.Get (0, 0, temp);

			button.Click += delegate {

				//button.Text=string.Format("{0}",temp[0],ToString());



				using (Bitmap img = BitmapFactory.DecodeResource (Resources, Resource.Drawable.fundus)) {
					if (img != null) {
						Mat m = new Mat ();
						Utils.BitmapToMat (img, m);

						MatOfByte bytemat=new MatOfByte();


						mFundusSegment.startup();



						bytemat = mFundusSegment.segmentImage(m);

						byte[] bytes=bytemat.ToArray();

					

						Bitmap bm= BitmapFactory.DecodeByteArray(bytes,0,bytes.Length);
//
//						Mat AddBm=new Mat();
//						Utils.BitmapToMat(bm,AddBm);
//
//
//
//						Mat dst=new Mat();
//						Core.AddWeighted(m,0.5,AddBm,0.5,,dst);
//
//						using (Bitmap bm2 = Bitmap.CreateBitmap (dst.Cols (), dst.Rows (), Bitmap.Config.Argb8888)) {
//							Utils.MatToBitmap (dst, bm2);
//
//
//
//							iView.SetImageBitmap (bm2);
//
//						}

						iView.SetImageBitmap (bm);

						m.Release();

					}
				}
//


				#region Test mlab_bwareaopen
//				//button.Text = string.Format ("{0}",sum.ToString());
//
//				//SetImage();
//				using (Bitmap img = BitmapFactory.DecodeResource (Resources, Resource.Drawable.testC)) {
//					if (img != null) {
//						Mat m = new Mat (),
//						mAfterBW = new Mat (),
//						garyM=new Mat();
//
//						Utils.BitmapToMat (img, m);
//						Imgproc.CvtColor (m, garyM, Imgproc.ColorBgr2gray);
//						garyM.ConvertTo(garyM,CvType.Cv32fc1);
//
//						for(int i=0;i<garyM.Rows();i++)
//							for(int j=0;j<garyM.Cols();j++)
//							{
//								garyM.Get(i,j,temp);
//								garyM.Put(i,j,temp[0]/255.0f);
//
//							}
//
//
//						float sum1=0;
//						for(int i=0;i<garyM.Rows();i++)
//							for(int j=0;j<garyM.Cols();j++)
//							{
//								garyM.Get(i,j,temp);
//								sum1=sum1+temp[0];
//							}
//
//
//						mAfterBW = MatlabFunctionsTest.mlab_bwareaopen (garyM, 50);
//
//						float sum2=0;
//						for(int i=0;i<garyM.Rows();i++)
//							for(int j=0;j<garyM.Cols();j++)
//							{
//								garyM.Get(i,j,temp);
//								sum2=sum2+temp[0];
//							}
//
//						for(int i=0;i<garyM.Rows();i++)
//							for(int j=0;j<garyM.Cols();j++)
//							{
//								mAfterBW.Get(i,j,temp);
//								mAfterBW.Put(i,j,temp[0]*255.0f);
//
//							}
//						mAfterBW.ConvertTo(mAfterBW,CvType.Cv8u);
//
//
//
//						Imgproc.CvtColor (mAfterBW, m, Imgproc.ColorGray2bgra);
//
//						using (Bitmap bm = Bitmap.CreateBitmap (m.Cols (), m.Rows (), Bitmap.Config.Argb8888)) {
//							Utils.MatToBitmap (m, bm);
//
//							iView.SetImageBitmap (bm);
//						}
//
//						button.Text=string.Format("{0} {1} {2}",garyM.Size().ToString(),sum1.ToString(),sum2.ToString());
//
//						m.Release();
//						mAfterBW.Release();
//						garyM.Release();
//
//					}
//				}
				#endregion


			};
		}


		public Mat write1ToMat(int i,int j,Mat testMat)
		{
			testMat.Put (i+0, j+0, 1);
			testMat.Put (i+0, j+1, 1);
			testMat.Put (i+0, j+2, 1);
			testMat.Put (i+0, j+3, 1);

			testMat.Put (i+1, j+0, 1);
			testMat.Put (i+1, j+1, 1);
			testMat.Put (i+1, j+2, 1);
			testMat.Put (i+1, j+3, 1);

			testMat.Put (i+2, j+0, 1);
			testMat.Put (i+2, j+1, 1);
			testMat.Put (i+2, j+2, 1);
			testMat.Put (i+2, j+3, 1);

			testMat.Put (i+3, j+0, 1);
			testMat.Put (i+3, j+1, 1);
			testMat.Put (i+3, j+2, 1);
			testMat.Put (i+3, j+3, 1);

			return testMat;
		}


		void SetImage()
		{
			ImageView iView = FindViewById<ImageView> (Resource.Id.imageView1);

			using (Bitmap img = BitmapFactory.DecodeResource (Resources, Resource.Drawable.mika)) {
				if (img != null) {
					Mat m = new Mat (),
					graryM = new Mat ();

					Utils.BitmapToMat (img, m);
					Imgproc.CvtColor (m, graryM, Imgproc.ColorBgr2gray);

					Imgproc.CvtColor (graryM, m, Imgproc.ColorGray2bgra);

					using (Bitmap bm = Bitmap.CreateBitmap (m.Cols (), m.Rows (), Bitmap.Config.Argb8888)) {
						Utils.MatToBitmap (m, bm);



						iView.SetImageBitmap (bm);
					
					}

					m.Release();
					graryM.Release();
				}
			}

		}

//		public static Bitmap bytesToUIImage (byte[] bytes)
//		{
//
//			if (bytes == null)
//				return null;
//
//			Bitmap bitmap;
//
//
//			var documentsFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
//
//			//Create a folder for the images if not exists
//			System.IO.Directory.CreateDirectory(System.IO.Path.Combine (documentsFolder, "images"));
//
//			string imatge = System.IO.Path.Combine (documents, "images", "image.jpg");
//
//
//			System.IO.File.WriteAllBytes(imatge, bytes.Concat(new Byte[]{(byte)0xD9}).ToArray());
//
//			bitmap = BitmapFactory.DecodeFile(imatge);
//
//			return bitmap;
//
//		}
	}
}


