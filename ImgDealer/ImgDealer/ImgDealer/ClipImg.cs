using CoreGraphics;
using System;
using System.Drawing;
using UIKit;

namespace ImgDealer
{
    public partial class ClipImg : UIViewController
    {
        private CGRect clearArea;
        UIImageView img;
        nfloat sw;
        nfloat totalScale = 1.0f;
        UIImageView ClipResult;
        ShelterView shv;
        public ClipImg() : base("ClipImg", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AutoresizingMask = UIViewAutoresizing.All;
            View.Frame = new CoreGraphics.CGRect(0, 20, 300, 600);
            clearArea = new CGRect(60, 120, 200, 200);
            shv = new ShelterView(clearArea,View.Bounds);
            shv.AutoresizingMask = UIViewAutoresizing.All;
           

            img = new UIImageView(View.Bounds);
            img.AutoresizingMask = UIViewAutoresizing.All;
            img.Image = UIImage.FromBundle("1.jpg");
            img.UserInteractionEnabled = true;

            sw = img.Frame.Width;

            View.AddSubview(img);
            View.AddSubview(shv);

            //��ͼ��ť
            UIBarButtonItem clipBtn = new UIBarButtonItem();
            clipBtn.Title = "��ȡ";
            clipBtn.Clicked += ClipBtn_Clicked;
            NavigationItem.RightBarButtonItem = clipBtn;

            

            //��������
            UIPinchGestureRecognizer ges = new UIPinchGestureRecognizer(DealPinchGesture);
            ges.Delegate = new PinchGestureRecognizerDelegate();

            shv.AddGestureRecognizer(ges);

            //ƽ������
            UIPanGestureRecognizer pan_ges = new UIPanGestureRecognizer(DealPanGesture);
            pan_ges.Delegate = new PinchGestureRecognizerDelegate();
            shv.AddGestureRecognizer(pan_ges);


        }

        private void ClipBtn_Clicked(object sender, EventArgs e)
        {
            var res = GetSelectedArea(clearArea,img.Frame);
            var si = UIImage.FromBundle("1.jpg");
            if (ClipResult == null)
            {
                ClipResult = new UIImageView();
                ClipResult.AutoresizingMask = UIViewAutoresizing.All;
                ClipResult.Frame = clearArea;
                View.AddSubview(ClipResult);
            }
            if (res.Item1 + res.Item2 + res.Item3 + res.Item4 == 0)
            {
                ClipResult.Image = si;
            }else
            {
                //si.Size.Width
                var x = si.Size.Width * res.Item1;
                var y = si.Size.Height * res.Item2;
                nfloat wd = si.Size.Width * res.Item3;
                nfloat ht = si.Size.Height * res.Item4;

                shv.Alpha = 0;
                img.Alpha = 0;
                ClipResult.Image = CropImage(si,x,y,wd,ht);
            }
        }

        //����ƽ��
        private void DealPanGesture(UIPanGestureRecognizer panGestureRecognizer)
        {
            UIView view = img;

            if(panGestureRecognizer.State == UIGestureRecognizerState.Began || panGestureRecognizer.State == UIGestureRecognizerState.Changed)
            {
                   CGPoint point =  panGestureRecognizer.TranslationInView(view.Superview);

                var center_x = view.Center.X + point.X;
                var center_y = view.Center.Y + point.Y;

                bool fc = center_x >= 0 && center_y >= 0;
                bool sc = (center_x <= UIScreen.MainScreen.Bounds.Width) && (center_y <= UIScreen.MainScreen.Bounds.Height);

                if (fc && sc)
                {
                    view.Center = new CGPoint(view.Center.X + point.X, view.Center.Y + point.Y);
                    panGestureRecognizer.SetTranslation(CGPoint.Empty, view.Superview);
                }else
                {
                    return;
                }
                
                 Console.WriteLine("x:{0},y:{1}",center_x,center_y);
               
            }
        }

      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceImage"></param>
        /// <param name="crop_x"></param>
        /// <param name="crop_y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private UIImage CropImage(UIImage sourceImage, nfloat crop_x, nfloat crop_y, nfloat width, nfloat height)
        {
            var imgSize = sourceImage.Size;
            UIGraphics.BeginImageContextWithOptions(new SizeF((float)width, (float)height), false, UIScreen.MainScreen.Scale);
            var context = UIGraphics.GetCurrentContext();
            var clippedRect = new CGRect(0, 0, (float)width, (float)height);

            //context.SetStrokeColor(UIColor.Red.CGColor);
            //context.SetLineWidth (1.0f);
            //context.StrokeRect(clippedRect);
            context.ClipToRect(clippedRect);
            var drawRect = new CGRect((float)-crop_x, (float)-crop_y, (float)imgSize.Width, (float)imgSize.Height);
            sourceImage.Draw(drawRect);
            var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return modifiedImage;
        }

        //��ȡѡ���������ͼƬ��λ��
        public Tuple<nfloat,nfloat,nfloat,nfloat> GetSelectedArea(CGRect limitArea,CGRect PicArea)
        {
            //��ȡ�������εĽ���
            CGRect intersectArea = CGRect.Intersect(limitArea,PicArea);
            //��ȡָ����������
            nfloat limitAcr = limitArea.Width * limitArea.Height;
            //���ν��������
            nfloat intersectArc = intersectArea.Width * intersectArea.Height;
            //��ȡͼƬ�����Ͻǵ��λ��
            CGPoint cp = new CGPoint(PicArea.X,PicArea.Y);

            //��ȡѡ����������Ͻ�λ��
            CGPoint scp = new CGPoint(intersectArea.X,intersectArea.Y);

            //��ȡѡ���������ͼƬ���ĵ��λ�ã��ٷֱȣ�

           

            if (intersectArea.IsEmpty)
            {
                return Tuple.Create<nfloat, nfloat, nfloat, nfloat>(0,0.25f,1,0.5f);
            }else
            {
                //�ж����������Ƿ����ڰ�����ϵ��ѡ��ͼƬ�������С��ָ�����������1/3
                if (CGRect.Equals(intersectArea, PicArea) || intersectArc < limitAcr / 3)
                {
                    //��������ͼƬ����
                    return Tuple.Create<nfloat, nfloat, nfloat, nfloat>(0, 0, 0, 0);
                }
                
                else
                {
                    var x_per = (scp.X - cp.X) / PicArea.Width;
                    var y_per = (scp.Y - cp.Y) / PicArea.Height;
                    var wd_per = intersectArea.Width / PicArea.Width;
                    var ht_per = intersectArea.Height / PicArea.Height;

                    Tuple<nfloat, nfloat, nfloat, nfloat> result = Tuple.Create<nfloat, nfloat, nfloat, nfloat>(x_per, y_per, wd_per, ht_per);

                    return result;
                }
            }

            

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            shv.Alpha = 1;
            img.Alpha = 1;
        }


        //��������
        public void DealPinchGesture(UIPinchGestureRecognizer pinchGestureRecognizer)
        {
            
            UIView view = img;
            if (pinchGestureRecognizer.State == UIGestureRecognizerState.Began || pinchGestureRecognizer.State == UIGestureRecognizerState.Changed)
            {
                if (pinchGestureRecognizer.Scale > 1)
                {
                    if (totalScale > 2.5)
                        return;
                }
                if (pinchGestureRecognizer.Scale < 1)
                {
                    if (totalScale < 0.5)
                        return;
                }
                pinchGestureRecognizer.DelaysTouchesEnded = true;
                    
                view.Transform = CGAffineTransform.Scale(view.Transform, pinchGestureRecognizer.Scale, pinchGestureRecognizer.Scale);
                totalScale *= pinchGestureRecognizer.Scale;
                Console.WriteLine(pinchGestureRecognizer.Scale);
                pinchGestureRecognizer.Scale = 1;
                
            }
        }

        public nfloat GetScale(nfloat sw,nfloat nw)
        {
            return nw / sw;
        }
        public class PinchGestureRecognizerDelegate : UIGestureRecognizerDelegate
        {
            public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
            {
                return true;
            }
        }



    }
}