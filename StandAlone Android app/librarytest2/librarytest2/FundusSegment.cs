using System;
using System.Collections.Generic;
using System.Collections;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Librarytest1;
using Android.Graphics;

using Org.Opencv.Android;
using Org.Opencv.Core;
using Org.Opencv.Utils;
using Org.Opencv.Imgproc;
using Org.Opencv.Highgui;

using AU.Com.Bytecode.Opencsv;
using Android.Util;
using Java.IO;
using Java.Util;


namespace librarytest2
{
	[Activity]
	public class FundusSegment: Activity
	{


		public FundusSegment ()
		{

		}

		public FileScope mFileScope=new FileScope();
		public Scalar mScalar = new Scalar (0);
		public MatlabFunctions mMatlabFunctions=new MatlabFunctions();


		// --------------------------------------------------------------------
		//  Initializes the FOV masking image for performing segmentation
		// --------------------------------------------------------------------
		public int startup() 
		{

			int greyCheck = (int) 0.9*255;
			Size s = new Size (mFileScope.RESOLUTION_X, mFileScope.RESOLUTION_Y);
			// Load the mask image from the disk and extract one of the color
			// channels to use in computing the associated binary mask 

			Mat maskImage = new Mat ();
			Mat maskFile = new Mat ();
			//var maskFile = path + string.Format(mFileScope.MASK_IMAGE_FILE);
			Context mContext = Application.Context;

			using (Bitmap img = BitmapFactory.DecodeResource (mContext.Resources, Resource.Drawable.fovMask)) {
				if (img!=null) {
					Utils.BitmapToMat (img, maskFile);
				}
			}

			Imgproc.Resize (maskFile, maskImage, s, 0, 0, Imgproc.InterCubic);

//			Mat mat1 = new Mat ();
//			Mat mat2 = new Mat ();
//			Mat mat3 = new Mat ();
//			Mat mat4 = new Mat ();
//
//			List<Mat> channels = new List<Mat> (4);
//			channels.Add (mat1);
//			channels.Add (mat2);
//			channels.Add (mat3);
//			channels.Add (mat4);
//
//
//			Core.Split (maskImage, channels);
			Mat channels = new Mat ();
			Core.ExtractChannel (maskImage, channels, 1);

			mFileScope.fovMask = new Mat ();
			// Threshold the mask image to create a binary mask
			channels.ConvertTo (mFileScope.fovMask, CvType.Cv32fc1, 1.0);
			Imgproc.Threshold (mFileScope.fovMask, mFileScope.fovMask, greyCheck, 1.0, Imgproc.ThreshBinary);

			// Load the postprocessing mask image from the disk and extract one of the color
			// channels to use in computing the associated binary mask 
			Mat postImage = new Mat ();
			Mat postFile = new Mat ();
			//var postFile = path + string.Format(mFileScope.POST_IMAGE_FILE);
			using (Bitmap img = BitmapFactory.DecodeResource (mContext.Resources, Resource.Drawable.fovMask_2)) {
				if (img!=null) {
					Utils.BitmapToMat (img, postFile);
				}
			}

			Imgproc.Resize (postFile, postImage, s, 0, 0, Imgproc.InterCubic);

//			Mat post1 = new Mat ();
//			Mat post2 = new Mat ();
//			Mat post3 = new Mat ();
//			Mat post4 = new Mat ();
//
//			List<Mat> postChannels = new List<Mat> (4);
//			postChannels.Add (post1);
//			postChannels.Add (post2);
//			postChannels.Add (post3);
//			postChannels.Add (post4);
//
//			Core.Split (postImage, postChannels);
			Mat postChannels = new Mat ();
			Core.ExtractChannel (postImage, postChannels, 1);

			// Threshold the mask image to create a binary mask
			mFileScope.postMask = new Mat ();
			postChannels.ConvertTo(mFileScope.postMask, CvType.Cv32fc1, 1.0);
			Imgproc.Threshold (mFileScope.postMask, mFileScope.postMask, greyCheck, 1.0, Imgproc.ThreshBinary);

			// Load the training data for the GMM classifier from disk
			Mat gmmSamples1 = new Mat ();
			gmmSamples1=readCSVFileToMat("train_data1.csv");
			gmmSamples1.ConvertTo (gmmSamples1, CvType.Cv64fc1, 1.0f);

			Mat gmmSamples2 = new Mat ();
			gmmSamples2=readCSVFileToMat("train_data2.csv");
			gmmSamples2.ConvertTo (gmmSamples2, CvType.Cv64fc1, 1.0f);

			List<Mat> MatListTeamp = new List<Mat> ();
			MatListTeamp.Add (gmmSamples1);
			MatListTeamp.Add (gmmSamples2);
			Core.Hconcat (MatListTeamp, gmmSamples1);

			Mat gmmSamples3 = new Mat ();
			gmmSamples3=readCSVFileToMat("train_data3.csv");
			gmmSamples3.ConvertTo (gmmSamples3, CvType.Cv64fc1, 1.0f);

			List<Mat> MatListTeamp1 = new List<Mat> ();
			MatListTeamp1.Add (gmmSamples1);
			MatListTeamp1.Add (gmmSamples3);
			Core.Hconcat (MatListTeamp1, gmmSamples1);

			gmmSamples1 = gmmSamples1.T ();

			Org.Opencv.Core.TermCriteria termCrit = new TermCriteria (); 
			mFileScope.classifier = new Org.Opencv.ML.EM (2,Org.Opencv.ML.EM.CovMatGeneric,termCrit);
			mFileScope.classifier.Train (gmmSamples1);

			var means   = mFileScope.classifier.GetMat("means");
			var weights = mFileScope.classifier.GetMat("weights");
			var covs    = mFileScope.classifier.GetMatVector("covs");

			return 0;
		}


		// --------------------------------------------------------------------
		//  Performs segmentation on the input image and returns the result
		//  :TODO: Some of these image functions implemented manually are 
		//        probably already available in openCV, would be faster
		// --------------------------------------------------------------------

		public MatOfByte segmentImage(Mat data)
		{
			// ---
			//
			// Scale the input to the segment resolution and perform pre-adjustments
			//
			// ---

			// Decode the input image and scale to the segmentation resolution
			//data is already been decode into Mat before calling

			Mat colorIn = new Mat ();
			//colorIn = Highgui.Imdecode (data, Highgui.CvLoadImageColor);
			colorIn = data.Clone ();

			Mat HH0 = new Mat ();
			//List <Mat> channels = new List<Mat> (4);
//			Java.Util.IList <Mat> channels = new Java.Util.ArrayList <Mat>(4);
//
//			Mat mat1 = new Mat ();
//			Mat mat2 = new Mat ();
//			Mat mat3 = new Mat ();
//			Mat mat4 = new Mat ();
//
//
//			channels.Add (mat1);
//			channels.Add (mat2);
//			channels.Add (mat3);
//			channels.Add (mat4);
//
//			Core.Split (colorIn, channels);

			Core.ExtractChannel (colorIn, HH0,1);


			//HH0 = channels.Get(0);
			//cv::cvtColor(colorIn, HH0, CV_RGB2GRAY);
			Size s = new Size (mFileScope.RESOLUTION_X, mFileScope.RESOLUTION_Y);
			Imgproc.Resize (HH0, HH0, s, 0, 0, Imgproc.InterNearest);

			HH0.ConvertTo (HH0, CvType.Cv32fc1, (float)(1.0 / 255.0));

			// Contrast adjustment (Window-Level adjusted to [1%,99%])
			mMatlabFunctions.mlab_imadjust(HH0);

			// ---
			//
			// Extract vessels using a high pass filter
			//
			// ---

			Mat I_bg = new Mat ();
			Size s2 = new Size (10, 10);
			Imgproc.Blur (HH0, I_bg, s2, new Org.Opencv.Core.Point (-1, -1), Imgproc.BorderReplicate);

			Mat I_sc = new Mat ();
			Core.Subtract( HH0, I_bg, I_sc);

			Imgproc.Threshold (I_sc, I_sc,0.0,0.0, Imgproc.ThreshTrunc);
			Mat I_sc1 = new Mat ();
			Core.Absdiff (I_sc, Scalar.All (0), I_sc1);

			Mat I_sc3 = new Mat ();
			I_sc3 = I_sc1.Mul (mFileScope.fovMask);
			mMatlabFunctions.mlab_imadjust (I_sc3);

			// ---
			//
			// Extract vessels using a line feature detection
			//
			// ---

			// Extraction of morphologically tophat reconstructed image (T=Kn1)
			Mat I_neg = new Mat ();
			Core.Subtract (Mat.Ones (HH0.Size (), CvType.Cv32fc1), HH0, I_neg);
			I_neg = I_neg.Mul (mFileScope.fovMask);
			mMatlabFunctions.mlab_imadjust (I_neg);

			I_neg=mFileScope.enhancer (I_neg, 2.0f);

			// Feature detect lines at 15 degree intervals
			Mat Kn1 = new Mat ();
			Imgproc.MorphologyEx (I_neg, Kn1, Imgproc.MorphTophat, mMatlabFunctions.mlab_strelLine (9, 0), new Org.Opencv.Core.Point (-1, -1), 1, Imgproc.BorderConstant, mScalar);

			for(int h = 1; h < 12; ++h)
			{
				Mat partialLines=new Mat();
				Imgproc.MorphologyEx (I_neg, partialLines, Imgproc.MorphTophat, mMatlabFunctions.mlab_strelLine (9, (float)(h*15.0)), new Org.Opencv.Core.Point (-1, -1), 1, Imgproc.BorderConstant, mScalar);
				Core.Max(partialLines, Kn1, Kn1);
			}
			Core.Normalize(Kn1, Kn1, 0.0, 1.0, Core.NormMinmax);
			mMatlabFunctions.mlab_imadjust(Kn1);

			// ---
			//
			// Identify the major vessels as those in (High pass AND Line feature)
			//
			// ---

			// Extract binary mask from the high pass and line feature filtered images
			// Also, remove any dangling islands before selecting the major vessels

			Imgproc.Threshold (Kn1, Kn1, 0.2, 1.0, Imgproc.ThreshBinary);
			Kn1 = mMatlabFunctions.mlab_bwareaopen (Kn1, mFileScope.MINIMUM_NEIGHBORHOOD);
			Imgproc.Threshold (I_sc3, I_sc3, 0.2, 1.0, Imgproc.ThreshBinary);
			I_sc3 = mMatlabFunctions.mlab_bwareaopen (I_sc3, mFileScope.MINIMUM_NEIGHBORHOOD);

			// Mark the intersecting regions as major vessels
			Mat vessels = new Mat ();
			vessels=Kn1.Mul(I_sc3);
			Imgproc.MedianBlur (vessels, vessels, 3);
			Imgproc.Threshold (vessels, vessels, 0.0, 1.0, Imgproc.ThreshBinary);


			// ---
			//
			// Perform GMM classification on the disputed pixels (High pass XOR Line feature)
			//
			// ---

			// Finding the disputed pixels that need additional classification

			Mat UC1 = new Mat (); 
			Mat UC2 = new Mat ();
			Mat unclassedPix = new Mat ();
			Mat tempMat1 = new Mat ();
			Mat tempMat2 = new Mat ();

			Core.Subtract (Kn1, vessels, tempMat1);
			Imgproc.Threshold (tempMat1, UC1, 0.0, 1.0, Imgproc.ThreshBinary);

			Core.Subtract (I_sc3, vessels, tempMat2);
			Imgproc.Threshold (tempMat2, UC2, 0.0, 1.0, Imgproc.ThreshBinary);

			Core.Add (UC1, UC2, unclassedPix);


			// Create a new mask of disputed pixels with a high probability of being vessels
			double GMM_PROB_THRESH = 0.9;
			unclassedPix.ConvertTo (unclassedPix, CvType.Cv64fc1,1.0);
 			HH0.ConvertTo (HH0, CvType.Cv64fc1,1.0);

//			var GMMReturn = mFileScope.computeGmmAttributes (unclassedPix, HH0);
//			unclassedPix = GMMReturn.Item1;
//			HH0 = GMMReturn.Item2;

			unclassedPix = mFileScope.computeGmmAttributes (unclassedPix, HH0);

			unclassedPix.ConvertTo (unclassedPix, CvType.Cv32f);

			int mk = 0;
			Imgproc.Threshold (unclassedPix, unclassedPix, GMM_PROB_THRESH, 1.0, Imgproc.ThreshBinary);
			Core.Add (vessels, unclassedPix, vessels);
			vessels = mMatlabFunctions.mlab_bwareaopen (vessels, mFileScope.MINIMUM_NEIGHBORHOOD);
			vessels = vessels.Mul (mFileScope.postMask);

			// ---
			//
			// Compose the GMM pixels and the major vessels as the final vessel mask
			//
			// ---

			vessels.ConvertTo (vessels, CvType.Cv8uc1, 255);
			Mat outputImage = new Mat (vessels.Rows (), vessels.Cols (), CvType.Cv8uc4);
			MatOfInt fromTo = new MatOfInt();

			fromTo.FromArray (0, 0, 0, 1, 0, 2, 0, 3);

			List<Mat> vesselsMat = new List<Mat> ();
			vesselsMat.Add (vessels);
			List<Mat> outputImageMat = new List<Mat> ();
			outputImageMat.Add (outputImage);


			Core.MixChannels (vesselsMat, outputImageMat, fromTo);
			MatOfByte buf = new MatOfByte ();
			Highgui.Imencode (".png", outputImageMat[0], buf);

			return buf;
		}



		public Mat readCSVFileToMat(string fileName)
		{

			string[] next;
			List<string[]> list = new List<string[]>();

			Context mContext = Application.Context;

				try {
				CSVReader reader = new CSVReader (new InputStreamReader (mContext.Assets.Open(fileName)));
				while (true) {
					next = reader.ReadNext();
					if(next != null) {
						list.Add(next);
					}
					else {
						break;
					}
					}
				} 
				catch (IOException e) {
					e.PrintStackTrace ();
				}

			Mat matReturn = new Mat (list.Count,list [0].Length,CvType.Cv32fc1,mScalar);

		
			for (int i = 0; i < list.Count; i++)
				for (int j = 0; j < list [i].Length; j++) {
					matReturn.Put (i, j, Convert.ToSingle (list [i] [j]));
				}
					
			return matReturn;
		}

	}
}

