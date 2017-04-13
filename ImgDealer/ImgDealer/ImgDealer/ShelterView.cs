using CoreGraphics;
using UIKit;

namespace ImgDealer
{
    public class ShelterView : UIView
    {
        private CGRect clearArea;
        public ShelterView(CGRect ClearArea, CGRect frame) : base(frame)
        {
            this.BackgroundColor = UIColor.Clear;
            this.Opaque = false;
            this.Alpha = 1;
            this.clearArea = ClearArea;

        }


        public override void Draw(CGRect rect)
        {
            var context = UIGraphics.GetCurrentContext();

            UIColor.FromRGBA(0, 0, 0, 0.3f).SetFill();
            context.FillRect(rect);

            context.ClearRect(clearArea);

            context.SetStrokeColor(UIColor.White.CGColor);
            context.SetLineWidth(1);
            //context.AddRect(clearArea);
            context.StrokeRect(clearArea);
            


        }
    }
}