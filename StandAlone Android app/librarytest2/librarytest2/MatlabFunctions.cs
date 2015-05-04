using System;
using System.Collections.Generic;

using Org.Opencv.Android;
using Org.Opencv.Core;
using Org.Opencv.Utils;
using Org.Opencv.Imgproc;


namespace librarytest2
{
	public class MatlabFunctions
	{
		public MatlabFunctions ()
		{
		}

		public List<bool> bwSeenReturn;

		public Mat mlab_imadjust(Mat mat)
		{

			Size Matsize = mat.Size ();
			int rows = (int)Matsize.Height;
			int cols = (int)Matsize.Width;

			float[] sortedArray = new float[rows * cols];

			int k = 0;
			float[] temp = new float[1];
			for(int i = 0; i < rows; ++i)
			{
				for(int j = 0; j < cols; ++j)
				{
					mat.Get (i, j, temp);
					sortedArray [k] = temp [0];
					++k;
				}
			}

			Array.Sort (sortedArray, 0, k);

			int onePercentIndex =(int) (0.01*rows*cols);

			float onePercentMin = sortedArray[onePercentIndex];
			float onePercentMax = sortedArray[rows*cols-1-onePercentIndex];

			float diffInv = 1.0f/(onePercentMax - onePercentMin);

			for (int i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					mat.Get (i, j, temp);

					if (temp[0] <= onePercentMin)
					{
						mat.Put(i,j,0.0);
					}

					else if (temp[0] >= onePercentMax)
					{
						mat.Put(i,j,1.0);
					}

					else
					{
						float newValue = (temp[0] - onePercentMin) * diffInv;
						if (newValue > 1.0f)
						{
							newValue = 1.0f;
						}
						if (newValue < 0.0f)
						{
							newValue = 0.0f;
						}
						mat.Put(i,j,newValue);
					}
				}
			}

			return mat;
		}



		public Mat mlab_strelLine(int length, float angle)
		{
			Scalar s = new Scalar (0);
			int[] temp = new int[1]; 

			float effectiveAngle =(float) (angle%(360.0f));

			if (effectiveAngle == 0.0f || effectiveAngle == 180.0f || effectiveAngle == -180.0f)
			{
				Mat retMat=new Mat(1, length, CvType.Cv8u, s);
				return retMat;
			}

			else if (effectiveAngle == 90.0f || effectiveAngle == 270.0f
				|| effectiveAngle == -90.0f || effectiveAngle == -270.0f)
			{
				Mat retMat=new Mat(length, 1, CvType.Cv8u, s);
				return retMat;
			}

			else if (effectiveAngle == 45.0f || effectiveAngle == 225.0f)
			{
				int width =(int)(0.778 * length);
				Mat retMat=new Mat(width, width,CvType.Cv8u, s);
				//Write the diagonal
				for (int i = 0; i < width; ++i)
				{
					retMat.Put(width-1-i, i, 1);
				}
				return retMat;
			}

			else if (effectiveAngle == -45.0f || effectiveAngle == -225.0f
				|| effectiveAngle == 135f)
			{
				int width =(int)(0.778 * length);
				Mat retMat=new Mat(width, width, CvType.Cv8u, s);
				//Write the diagonal
				for (int i = 0; i < width; ++i)
				{
					retMat.Put(i,i,1);
				}
				return retMat;
			}

			else
			{
				if (angle%(15.0) == 0.0f && angle > 0.0f && angle <= 165.0f && length == 9)
				{
					//Use pre-calculated matrices.
					PreCalculated p = new PreCalculated ();

					return p.GetStrelLength9(angle);
				}

				Mat retMat =new Mat(length, length,CvType.Cv8u, s);
				return retMat;
			}

		}


		public Mat bwareaopenHelper(Mat BW, int P, List<bool> bwSeen, int startRowIndx, int startColIndx)
		{
			float[] temp = new float[1];

			int rows = BW.Rows();
			int cols = BW.Cols();

			List<int> whiteIndicesCheckNeighborRow=new List<int>();
			List<int> whiteIndicesCheckNeighborCol=new List<int>();
			List<int> whiteIndicesCheckedNeighborRow=new List<int>();
			List<int> whiteIndicesCheckedNeighborCol=new List<int>();

			whiteIndicesCheckNeighborRow.Add (startRowIndx);
			whiteIndicesCheckNeighborCol.Add (startColIndx);

			while(whiteIndicesCheckNeighborRow.Count > 0)
			{
				int currentRow = whiteIndicesCheckNeighborRow[whiteIndicesCheckNeighborRow.Count-1];
				int currentCol = whiteIndicesCheckNeighborCol[whiteIndicesCheckNeighborCol.Count-1];

				whiteIndicesCheckNeighborRow.RemoveAt(whiteIndicesCheckNeighborRow.Count-1);
				whiteIndicesCheckNeighborCol.RemoveAt(whiteIndicesCheckNeighborCol.Count-1);

				whiteIndicesCheckedNeighborRow.Add(currentRow);
				whiteIndicesCheckedNeighborCol.Add(currentCol);

				bwSeen[(currentRow)*cols+(currentCol)] = true;

				//Check all neighbors
				bool hasTop = currentRow > 0;
				bool hasBot = currentRow < (rows - 1);
				bool hasLeft = currentCol > 0;
				bool hasRight = currentCol < (cols - 1);

				if (hasTop)
				{
					if (hasLeft)
					{
						//Top-Left
						BW.Get (currentRow - 1, currentCol - 1, temp);

						if (bwSeen[(currentRow-1)*cols+(currentCol-1)] == false
							&& temp[0] == 1.0)
						{
							whiteIndicesCheckNeighborRow.Add(currentRow-1);
							whiteIndicesCheckNeighborCol.Add(currentCol-1);
						}
						bwSeen[(currentRow-1)*cols+(currentCol-1)] = true;
					}

					//Top
					BW.Get (currentRow - 1, currentCol, temp);

					if (bwSeen[(currentRow-1)*cols+(currentCol)] == false
						&& temp[0] == 1.0)
					{
						whiteIndicesCheckNeighborRow.Add(currentRow-1);
						whiteIndicesCheckNeighborCol.Add(currentCol);
					}
					bwSeen[(currentRow-1)*cols+(currentCol)] = true;

					if (hasRight)
					{
						//Top-Right
						BW.Get (currentRow - 1, currentCol + 1, temp);

						if (bwSeen[(currentRow-1)*cols+(currentCol+1)] == false
							&& temp[0] == 1.0)
						{
							whiteIndicesCheckNeighborRow.Add(currentRow-1);
							whiteIndicesCheckNeighborCol.Add(currentCol+1);
						}
						bwSeen[(currentRow-1)*cols+(currentCol+1)] = true;
					}
				}


				if (hasBot)
				{
					if (hasLeft)
					{
						//Bot-Left
						BW.Get (currentRow + 1, currentCol - 1, temp);

						if (bwSeen[(currentRow+1)*cols+(currentCol-1)] == false
							&& temp[0] == 1.0)
						{
							whiteIndicesCheckNeighborRow.Add(currentRow+1);
							whiteIndicesCheckNeighborCol.Add(currentCol-1);
						}
						bwSeen[(currentRow+1)*cols+(currentCol-1)] = true;
					}

					//Bot
					BW.Get (currentRow + 1, currentCol, temp);

					if (bwSeen[(currentRow+1)*cols+(currentCol)] == false
						&& temp[0] == 1.0)
					{
						whiteIndicesCheckNeighborRow.Add(currentRow+1);
						whiteIndicesCheckNeighborCol.Add(currentCol);
					}
					bwSeen[(currentRow+1)*cols+(currentCol)] = true;

					if (hasRight)
					{
						//Bot-Right
						BW.Get (currentRow + 1, currentCol + 1, temp);

						if (bwSeen[(currentRow+1)*cols+(currentCol+1)] == false
							&& temp[0] == 1.0)
						{
							whiteIndicesCheckNeighborRow.Add(currentRow+1);
							whiteIndicesCheckNeighborCol.Add(currentCol+1);
						}
						bwSeen[(currentRow+1)*cols+(currentCol+1)] = true;
					}
				}


				if (hasLeft)
				{
					//Left
					BW.Get (currentRow, currentCol - 1, temp);

					if (bwSeen[(currentRow)*cols+(currentCol-1)] == false
						&& temp[0] == 1.0)
					{
						whiteIndicesCheckNeighborRow.Add(currentRow);
						whiteIndicesCheckNeighborCol.Add(currentCol-1);
					}
					bwSeen[(currentRow)*cols+(currentCol-1)] = true;
				}


				if (hasRight)
				{
					//Right
					BW.Get (currentRow, currentCol + 1, temp);

					if (bwSeen[(currentRow)*cols+(currentCol+1)] == false
						&& temp[0] == 1.0)
					{
						whiteIndicesCheckNeighborRow.Add(currentRow);
						whiteIndicesCheckNeighborCol.Add(currentCol+1);                
					}
					bwSeen[(currentRow)*cols+(currentCol+1)] = true;
				}
			}


			//Check how many pixels were a part of this component
			//and if fewer than P pixels, remove them from BW.
			if (whiteIndicesCheckedNeighborRow.Count < P)
			{
				while (whiteIndicesCheckedNeighborRow.Count > 0)
				{
					int currentRow = whiteIndicesCheckedNeighborRow[whiteIndicesCheckedNeighborRow.Count-1];
					int currentCol = whiteIndicesCheckedNeighborCol[whiteIndicesCheckedNeighborCol.Count-1];
					whiteIndicesCheckedNeighborRow.RemoveAt(whiteIndicesCheckedNeighborRow.Count-1);
					whiteIndicesCheckedNeighborCol.RemoveAt(whiteIndicesCheckedNeighborCol.Count-1);

					BW.Put (currentRow, currentCol, 0.0f);
				}
			}


			bwSeenReturn = new List<bool> (bwSeen);

			return BW;
	
		}


		public Mat mlab_bwareaopen(Mat BW,int P)
		{
			float[] temp = new float[1];
			int rows = BW.Rows();
			int cols = BW.Cols();
			int totalPixels = rows*cols;

			List<bool> bwSeen = new List<bool>();

			int i;
			for (i = 0; i < totalPixels; i++)
			{
				bwSeen.Add(false);
			}

			for (i = 0; i < rows; ++i)
			{
				for (int j = 0; j < cols; ++j)
				{
					int currentIndex = i*cols + j;
					if (bwSeen[currentIndex] == false)
					{
						BW.Get (i, j, temp);
						if (temp[0] == 1.0f)
						{
							//Found the beginning of a component
							BW = bwareaopenHelper(BW, P, bwSeen, i, j);

							bwSeen = new List<bool>(bwSeenReturn);
						}
						else
						{
							bwSeen[currentIndex] = true;
						}
					}
				}
			}

			return BW;
				
		}
			
	}
}

