using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Util;

namespace Application
{
	public class PaintView : ImageView
	{
		public Paint _PaintWhite = new Paint();
		public Paint _PaintErase = new Paint();

		private const int THICKNESS = 3;
		private PointF _StartPt = new PointF();
		private PointF _EndPt = new PointF();
		public Bitmap _Bmp = null;
		public Canvas _Canvas = null;
		//public Canvas _Canvas2 = null;

		public int w;
		public int h;

		public int status = 5;

		public PaintView(Context context, IAttributeSet attrs)
			: base(context, attrs)
		{
			this.Initialize();
		}

		void Initialize()
		{
			_PaintWhite.Color = new Color(255, 255, 255);
			_PaintWhite.StrokeWidth = THICKNESS;
			_PaintWhite.StrokeCap = Paint.Cap.Round;
			//_PaintWhite.SetXfermode(new PorterDuffXfermode (PorterDuff.Mode.Xor));

			_PaintErase.Color =  new Color(0, 0, 0);
			_PaintErase.StrokeWidth = 7;
			_PaintErase.SetXfermode (new PorterDuffXfermode (PorterDuff.Mode.Clear));


		}

		override protected void OnMeasure(int wSpec, int hSpec)
		{
			base.OnMeasure(wSpec, hSpec);
			w = MeasureSpec.GetSize(wSpec);
			h = MeasureSpec.GetSize(hSpec);

		}

		override public bool OnTouchEvent(MotionEvent e)
		{ 
			if (status == 1) {
				switch (e.Action)
				{
				case MotionEventActions.Down: 
					_StartPt.Set(e.GetX() - 1, e.GetY() - 1);// for just a tapping
					DrawLine(e);
					break;
				case MotionEventActions.Move: 
					DrawLine(e);
					break;
				case MotionEventActions.Up: 
					DrawLine(e);
					break;
				}
			}

			if (status == 0) {
				switch (e.Action)
				{
				case MotionEventActions.Down: 
					_StartPt.Set(e.GetX() - 1, e.GetY() - 1);// for just a tapping
					EraseLine(e);
					break;
				case MotionEventActions.Move: 
					EraseLine(e);
					break;
				case MotionEventActions.Up: 
					EraseLine(e);
					break;
				}
			
			}

			if (status == 5) {
			
			}

			return true;
		}

		private void DrawLine(MotionEvent e)
		{
			_EndPt.Set(e.GetX(), e.GetY());
			_Canvas.DrawLine(_StartPt.X, _StartPt.Y, _EndPt.X, _EndPt.Y, _PaintWhite);
			_StartPt.Set(_EndPt);
			Invalidate();
		}

		private void EraseLine(MotionEvent e)
		{
			_EndPt.Set(e.GetX(), e.GetY());
			_Canvas.DrawLine(_StartPt.X, _StartPt.Y, _EndPt.X, _EndPt.Y, _PaintErase);
			//_Canvas.DrawColor (Color.Black, PorterDuff.Mode.Clear);
			_StartPt.Set(_EndPt);
			Invalidate();
		}

		~PaintView()
		{

		}

	}
}