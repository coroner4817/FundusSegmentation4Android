using System;

using Org.Opencv.Android;
using Org.Opencv.Core;
using Org.Opencv.Utils;
using Org.Opencv.Imgproc;


namespace librarytest2
{
	public class PreCalculated
	{
		public PreCalculated ()
		{ 
		}

		public Mat GetStrelLength9(float angle)
		{
			Scalar s = new Scalar (0);

			if (angle == 15.0)
			{

				Mat retMat=new Mat(3, 9, CvType.Cv8u, s);

				retMat.Put(2, 0, 1);
				retMat.Put(2, 1, 1);
				retMat.Put(2, 2, 1);

				retMat.Put(1, 3, 1);
				retMat.Put(1, 4, 1);
				retMat.Put(1, 5, 1);

				retMat.Put(0, 6, 1);
				retMat.Put(0, 7, 1);
				retMat.Put(0, 8, 1);

				return retMat;
			}

			if (angle == 30.0)
			{
				Mat retMat=new Mat(5, 7, CvType.Cv8u, s);

				retMat.Put(4, 0, 1);
				retMat.Put(3, 1, 1);
				retMat.Put(3, 2, 1);

				retMat.Put(2, 3, 1);
				retMat.Put(1, 4, 1);
				retMat.Put(1, 5, 1);

				retMat.Put(0, 6, 1);

				return retMat;
			}

			if (angle == 60.0)
			{
				Mat retMat=new Mat(7, 5, CvType.Cv8u, s);

				retMat.Put(6, 0, 1);
				retMat.Put(5, 1, 1);
				retMat.Put(4, 1, 1);

				retMat.Put(3, 2, 1);
				retMat.Put(2, 3, 1);
				retMat.Put(1, 3, 1);

				retMat.Put(0, 4, 1);

				return retMat;
			}

			if (angle == 75.0)
			{
				Mat retMat=new Mat(9, 3, CvType.Cv8u, s);

				retMat.Put(8, 0, 1);
				retMat.Put(7, 0, 1);
				retMat.Put(6, 0, 1);

				retMat.Put(5, 1, 1);
				retMat.Put(4, 1, 1);
				retMat.Put(3, 1, 1);

				retMat.Put(2, 2, 1);
				retMat.Put(1, 2, 1);
				retMat.Put(0, 2, 1);

				return retMat;
			}

			if (angle == 105.0)
			{
				Mat retMat=new Mat(9, 3, CvType.Cv8u, s);

				retMat.Put(0, 0, 1);
				retMat.Put(1, 0, 1);
				retMat.Put(2, 0, 1);

				retMat.Put(3, 1, 1);
				retMat.Put(4, 1, 1);
				retMat.Put(5, 1, 1);

				retMat.Put(6, 2, 1);
				retMat.Put(7, 2, 1);
				retMat.Put(8, 2, 1);

				return retMat;
			}

			if (angle == 120.0)
			{
				Mat retMat=new Mat(7, 5, CvType.Cv8u, s);

				retMat.Put(0, 0, 1);
				retMat.Put(1, 1, 1);
				retMat.Put(2, 1, 1);

				retMat.Put(3, 2, 1);
				retMat.Put(4, 3, 1);
				retMat.Put(5, 3, 1);

				retMat.Put(6, 4, 1);

				return retMat;
			}

			if (angle == 150.0)
			{
				Mat retMat=new Mat(5, 7, CvType.Cv8u, s);

				retMat.Put(0, 0, 1);
				retMat.Put(1, 1, 1);
				retMat.Put(1, 2, 1);

				retMat.Put(2, 3, 1);
				retMat.Put(3, 4, 1);
				retMat.Put(3, 5, 1);

				retMat.Put(4, 6, 1);

				return retMat;
			}

			if (angle == 165.0)
			{
				Mat retMat=new Mat(3, 9, CvType.Cv8u, s);;

				retMat.Put(0, 0, 1);
				retMat.Put(0, 1, 1);
				retMat.Put(0, 2, 1);

				retMat.Put(1, 3, 1);
				retMat.Put(1, 4, 1);
				retMat.Put(1, 5, 1);

				retMat.Put(2, 6, 1);
				retMat.Put(2, 7, 1);
				retMat.Put(2, 8, 1);

				return retMat;
			}


			Mat reMat=new Mat(9, 9, CvType.Cv8u, s);

			return reMat;

		}
	}
}

