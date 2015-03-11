// Reference http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=17266
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace iRobotGUI.Controls
{
    class DragAdorner : Adorner
    {
        #region Data

        private Rectangle child = null;
        private double offsetLeft = 0;
        private double offsetTop = 0;

        #endregion

        #region constructor

        public DragAdorner(UIElement adornedElement, Size size, Brush brush)
            : base(adornedElement)
        {
            Rectangle rect = new Rectangle();
            rect.Fill = brush;
            rect.Width = size.Width;
            rect.Height = size.Height;
            rect.IsHitTestVisible = false;
            this.child = rect;
        }

        #endregion

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.offsetLeft, this.offsetTop));
            return result;
        }

        public double OffsetLeft
        {
            get { return this.offsetLeft; }
            set
            {
                this.offsetLeft = value;
                UpdateLocation();
            }
        }

        public void SetOffsets(double left, double top)
        {
            this.offsetLeft = left;
            this.offsetTop = top;
            this.UpdateLocation();
        }

        public double OffsetTop
        {
            get { return this.offsetTop; }
            set
            {
                this.offsetTop = value;
                UpdateLocation();
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            this.child.Measure(constraint);
            return this.child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.child;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        private void UpdateLocation()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            if (adornerLayer != null)
                adornerLayer.Update(this.AdornedElement);
        }

    }
}