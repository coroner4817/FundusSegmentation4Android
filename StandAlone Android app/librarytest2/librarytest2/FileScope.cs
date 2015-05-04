using System;

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


namespace librarytest2
{
	public class FileScope
	{
		public FileScope ()
		{
		}

		// File locations on disk
		public string MASK_IMAGE_FILE="fovMask.jpg";
		public string POST_IMAGE_FILE="fovMask_2.jpg";

		// Image resolution for segmentation
		public int RESOLUTION_X = 500;
		public int RESOLUTION_Y = 500;

		// Features for GMM classifier
		public const int ATTRS = 6; // Number of features
		public const int VAL   = 0; // Pixel intensity
		public const int AVG   = 1; // Mean of neighborhood
		public const int VAR   = 2; // Variance of neighborhood
		public const int MAX   = 3; // Max of neighborhood
		public const int MIN   = 4; // Min of neighborhood
		public const int PCT   = 5; // Value percentile (ie. Percent of neighbors with higher intensities)

		// Segmentation parameters
		public int MINIMUM_NEIGHBORHOOD = 30;

		// Field-of-view mask image
		public Mat fovMask;
		public Mat postMask;

		// Trained GMM classifier
		public Org.Opencv.ML.EM classifier;

		// --------------------------------------------------------------------
		//  Enhances the value curve of the input image using a level factor t把矩阵每个元素之间的差距放大
		// --------------------------------------------------------------------
		public Mat enhancer(Mat mat, float t)
		{
			Scalar alpha = new Scalar(Math.Pow(10.0, t));
			Core.Multiply (mat, alpha, mat);
			//mat = mat * Math.Pow(10.0, t);//10的t次幂
			Core.Pow(mat, t, mat);//mat中每一个元素的t次幂然后存回mat

			Core.Normalize(mat, mat, 0.0, 1.0, 32);//把mat矩阵元素的值都化为0-1之间

			return mat;
		}

		// --------------------------------------------------------------------
		//  Computes GMM classifier attributes for each unclassified pixel
		//  Takes the mask of unclassified pixels and the image as input
		//  This is an interesting problem, you apply a series of convolution 
		//  kernels to only the masked elements of a matrix. Not sure how to
		//  do this with OpenCV so we will inefficiently do it ourselves.
		// --------------------------------------------------------------------
		public Mat computeGmmAttributes(Mat noclass, Mat image)//计算每个像素的六个属性
		{
			Scalar s = new Scalar (0);
			double[] temp = new double[1];

			int K = 7; // Width of the kernel

			int M = (K - 1) / 2;   //M=3
			for (int j = 0; j < noclass.Cols(); ++j)
				for (int i = 0; i < noclass.Rows(); ++i)
				{
	
					noclass.Get (i, j, temp);


					if (temp[0] == 0.0) continue; // already classified

					image.Get (i, j, temp);
					double pix = temp[0]; // Pixel intensity

					// Compute the bounds for the kernel mask 计算出取得小矩阵的边界 8x8的小矩阵
					int rMin = (i < M) ? 0 : i - M;
					int rMax = (i > noclass.Rows()-M) ? noclass.Rows() : i + M + 1;
					int cMin = (j < M) ? 0 : j - M;
					int cMax = (j > noclass.Cols()-M) ? noclass.Cols() : j + M + 1;

					// Compute the pixel's classification attributes
					double low  = 2.0;
					double high = -1.0;
					double sum  = 0.0;
					int   pct  = 0;
					for (int c = cMin; c < cMax; c++)
						for (int r = rMin; r < rMax; r++)
						{
							image.Get (r, c, temp);
							double val = temp[0];
							sum  += val;    //计算小矩阵所有元素的和
							low  =  Math.Min(low, val);    //找到小矩阵中元素的最小值
							high =  Math.Max(high, val);  //找到小矩阵中元素的最大值
							if (val > pix) pct++;   //如果在小矩阵中的某个元素的值大于中心元素则加1
						}

					// Compute the sample variance (Same way MATLAB trains it)
					double ksize = (double)(rMax - rMin) * (cMax - cMin);  //计算小矩阵的size
					double mean  = sum / ksize;  //所有元素的平均值
					double stat  = 0.0f;     
					for (int c = cMin; c < cMax; c++)
						for (int r = rMin; r < rMax; r++)
						{
							image.Get (r, c, temp);
							double val = temp[0];
							val -= mean;
							stat += val*val;    //计算方差
						}
					// Calculate the pixels classification attributes
					double variance = stat / (ksize-1);   //小矩阵的方差


					Mat attrs=new Mat(1, 6, CvType.Cv64fc1,s);       //定义每个像素的属性矩阵

					attrs.Put (0, FileScope.VAL, pix);
					attrs.Put (0, FileScope.AVG, mean);
					attrs.Put (0, FileScope.VAR, Math.Sqrt(variance));
					attrs.Put (0, FileScope.MAX, high);
					attrs.Put (0, FileScope.MIN, low);
					attrs.Put (0, FileScope.PCT, pct / ksize);


					// Calculate the probability of being a vessel

					Mat probs = new Mat();    //定义每个像素的可能成为vessel的可能性矩阵

					classifier.Predict(attrs, probs);		//EM算法计算attrs的可能性矩阵
					//返回两个值，probs.at<double>(0)是这个元素的可能性

					double[] tempDouble = new double[1];
					probs.Get (0,0,tempDouble);
					noclass.Put (i, j, tempDouble [0]);
		
				}

			return noclass;
		}


	}
}

