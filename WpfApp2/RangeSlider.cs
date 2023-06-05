using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    [TemplatePart(Name = PART_StartThumb, Type = typeof(System.Windows.Controls.Primitives.Thumb))]
    [TemplatePart(Name = PART_EndThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = PART_StartRegion, Type = typeof(System.Windows.Controls.Primitives.RepeatButton))]
    [TemplatePart(Name = PART_MiddleRegion, Type = typeof(System.Windows.Controls.Primitives.RepeatButton))]
    [TemplatePart(Name = PART_EndRegion, Type = typeof(System.Windows.Controls.Primitives.RepeatButton))]
    public class RangeSlider : Control
    {
        private enum ThumbKind
        {
            Start,

            End,
        }

        static RangeSlider()
        {
            EventManager.RegisterClassHandler(typeof(RangeSlider), Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStartedEvent));
            EventManager.RegisterClassHandler(typeof(RangeSlider), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnThumbDragDelta));
            EventManager.RegisterClassHandler(typeof(RangeSlider), Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompletedEvent));
        }

        public RangeSlider()
        {
            Loaded += RangeSlider_Loaded;
            LowerValueChanged += RangeSlider_LowerValueChanged;
            UpperValueChanged += RangeSlider_UpperValueChanged;
        }

        private const string PART_StartThumb = nameof(PART_StartThumb);

        private const string PART_EndThumb = nameof(PART_EndThumb);

        private const string PART_StartRegion = nameof(PART_StartRegion);

        private const string PART_MiddleRegion = nameof(PART_MiddleRegion);

        private const string PART_EndRegion = nameof(PART_EndRegion);

        private System.Windows.Controls.Primitives.Thumb _StartThumb;

        private System.Windows.Controls.Primitives.Thumb _EndThumb;

        private System.Windows.Controls.Primitives.RepeatButton _StartRegion;

        private System.Windows.Controls.Primitives.RepeatButton _MiddleRegion;

        private System.Windows.Controls.Primitives.RepeatButton _EndRegion;

        #region Step

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(double), typeof(RangeSlider),
                new FrameworkPropertyMetadata(0.1d, OnStepChanged, OnCoerceStep));

        public double Step
        {
            get => (double)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        private static object OnCoerceStep(DependencyObject d, object baseValue)
        {
            var num = (double)baseValue;
            if (num < 0)
            {
                throw new ArgumentException("step must >=0");
            }
            return num;
        }

        private static void OnStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion Step

        #region Maximum

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(RangeSlider),
                new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnMaximumChanged, OnCoerceMaximum));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        private static object OnCoerceMaximum(DependencyObject d, object baseValue)
        {
            var slider = (RangeSlider)d;
            var num = (double)baseValue;
            if (num < slider.Minimum)
            {
                throw new ArgumentException("maximum must be >= minimum ");
            }
            return baseValue;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion Maximum

        #region Minimum

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(RangeSlider),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure, OnMinimumChanged, OnCoerceMinimum));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        private static object OnCoerceMinimum(DependencyObject d, object baseValue)
        {
            var slider = (RangeSlider)d;
            var num = (double)baseValue;
            if (num > slider.Maximum)
            {
                throw new ArgumentException("minimum must be <= maximum");
            }
            return baseValue;
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion Minimum

        #region LowerValue

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register(nameof(LowerValue), typeof(double), typeof(RangeSlider),
                new FrameworkPropertyMetadata(25d, FrameworkPropertyMetadataOptions.AffectsArrange, OnLowerValueChanged, OnCoerceLowerValue));

        public double LowerValue
        {
            get => (double)GetValue(LowerValueProperty);
            set => SetValue(LowerValueProperty, value);
        }

        private static object OnCoerceLowerValue(DependencyObject d, object baseValue)
        {
            var slider = (RangeSlider)d;
            var num = (double)baseValue;
            if (num > slider.UpperValue)
            {
                throw new ArgumentException("lowerValue must be <= upperValue");
            }
            if (num < slider.Minimum)
            {
                throw new ArgumentException("lowerValue must be >= minimum");
            }
            return num;
        }

        private static void OnLowerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (RangeSlider)d;
            var args = new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue, RangeSlider.LowerValueChangedEvent);
            slider.RaiseEvent(args);
        }

        #endregion LowerValue

        #region UpperValue

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register(nameof(UpperValue), typeof(double), typeof(RangeSlider),
                new FrameworkPropertyMetadata(75d, FrameworkPropertyMetadataOptions.AffectsArrange, OnUpperValueChanged, OnCoerceUpperValue));

        public double UpperValue
        {
            get => (double)GetValue(UpperValueProperty);
            set => SetValue(UpperValueProperty, value);
        }

        private static object OnCoerceUpperValue(DependencyObject d, object baseValue)
        {
            var slider = (RangeSlider)d;

            var num = (double)baseValue;

            if (num < slider.LowerValue)
            {
                throw new ArgumentException("upperValue must be >= lowerValue ");
            }
            if (num > slider.Maximum)
            {
                throw new ArgumentException("upperValue must be <= maximum ");
            }
            return num;
        }

        private static void OnUpperValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (RangeSlider)d;
            var args = new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue, RangeSlider.UpperValueChangedEvent);
            slider.RaiseEvent(args);
        }

        #endregion UpperValue

        #region Orientation

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(RangeSlider), new PropertyMetadata(default(Orientation)));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        #endregion Orientation

        #region TickPlacement

        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register(nameof(TickPlacement), typeof(TickPlacement), typeof(RangeSlider), new PropertyMetadata(default(TickPlacement)));

        public TickPlacement TickPlacement
        {
            get => (TickPlacement)GetValue(TickPlacementProperty);
            set => SetValue(TickPlacementProperty, value);
        }

        #endregion TickPlacement

        #region Override

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _StartThumb = GetTemplateChild(PART_StartThumb) as System.Windows.Controls.Primitives.Thumb;
            _EndThumb = GetTemplateChild(PART_EndThumb) as System.Windows.Controls.Primitives.Thumb;
            _StartRegion = GetTemplateChild(PART_StartRegion) as System.Windows.Controls.Primitives.RepeatButton;
            _MiddleRegion = GetTemplateChild(PART_MiddleRegion) as System.Windows.Controls.Primitives.RepeatButton;
            _EndRegion = GetTemplateChild(PART_EndRegion) as System.Windows.Controls.Primitives.RepeatButton;
        }

        #endregion Override

        #region LowerValueChanged

        public static readonly RoutedEvent LowerValueChangedEvent =
            EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(RangeSlider));

        public event RoutedPropertyChangedEventHandler<double> LowerValueChanged
        {
            add { AddHandler(LowerValueChangedEvent, value); }
            remove { RemoveHandler(LowerValueChangedEvent, value); }
        }

        #endregion LowerValueChanged

        #region UpperValueChanged

        public static readonly RoutedEvent UpperValueChangedEvent =
            EventManager.RegisterRoutedEvent("UpperValueChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(RangeSlider));

        public event RoutedPropertyChangedEventHandler<double> UpperValueChanged
        {
            add { AddHandler(UpperValueChangedEvent, value); }
            remove { RemoveHandler(UpperValueChangedEvent, value); }
        }

        #endregion UpperValueChanged

        #region HandleDragEvent

        private static void OnDragStartedEvent(object sender, DragStartedEventArgs e)
        {
            if (sender is RangeSlider rs)
            {
                rs.OnDragStartedEvent(e);
            }
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is RangeSlider rs)
            {
                rs.OnThumbDragDelta(e);
            }
        }

        private static void OnDragCompletedEvent(object sender, DragCompletedEventArgs e)
        {
            if (sender is RangeSlider rs)
            {
                rs.OnDragCompletedEvent(e);
            }
        }

        private void OnDragStartedEvent(DragStartedEventArgs e)
        {
        }

        private void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            if (!CanUpdate())
            {
                return;
            }
            var offset = 0d;

            if (Orientation == Orientation.Horizontal)
            {
                offset = e.HorizontalChange / ActualWidth * (Math.Abs(Maximum - Minimum));
            }
            else
            {
                offset = e.VerticalChange / ActualHeight * (Math.Abs(Maximum - Minimum));
            }

            var thumb = e.OriginalSource as Thumb;

            if (thumb == _StartThumb)
            {
                LowerValue = Math.Max(Minimum, Math.Min(LowerValue + offset, UpperValue));
                UpdateThumbPosition(ThumbKind.Start);
            }
            else if (thumb == _EndThumb)
            {
                UpperValue = Math.Min(Maximum, Math.Max(UpperValue + offset, LowerValue));
                UpdateThumbPosition(ThumbKind.End);
            }
        }

        private void OnDragCompletedEvent(DragCompletedEventArgs e)
        {
        }

        #endregion HandleDragEvent

        #region Private

        private bool CanUpdate()
        {
            return _StartThumb != null && _EndThumb != null && _StartRegion != null && _MiddleRegion != null && _EndRegion != null;
        }

        private void UpdateThumbPosition(ThumbKind thumbKind)
        {
            if (!CanUpdate())
            {
                return;
            }
            var total = CalculateTotalSize(Orientation);
            double scale = 0d;

            if (Orientation == Orientation.Horizontal)
            {
                switch (thumbKind)
                {
                    case ThumbKind.Start:
                        scale = MapValueToRange(LowerValue, Minimum, Maximum, 0d, 1d);
                        _StartRegion.Width = scale * total;
                        break;

                    case ThumbKind.End:

                        scale = MapValueToRange(Maximum - UpperValue + Minimum, Minimum, Maximum, 0d, 1d);
                        _EndRegion.Width = scale * total;
                        break;

                    default:
                        break;
                }
            }
            else
            {
                //switch (thumbKind)
                //{
                //    case ThumbKind.Start:
                //        _StartRegion.Height = value / scale * total;
                //        break;

                //    case ThumbKind.End:
                //        _EndRegion.Height = (scale - value) / scale * total;
                //        break;

                //    default:
                //        break;
                //}
            }
        }

        private double MapValueToRange(double num, double originalMin, double originalMax, double targetMin, double targetMax)
        {
            double relativePosition = (num - originalMin) / (originalMax - originalMin);
            return targetMin + relativePosition * (targetMax - targetMin);
        }

        private double CalculateTotalSize(Orientation orientation)
        {
            if (!CanUpdate())
            {
                return -1;
            }
            switch (orientation)
            {
                case Orientation.Horizontal:
                    return _StartRegion.ActualWidth + _MiddleRegion.ActualWidth + _EndRegion.ActualWidth;

                case Orientation.Vertical:

                    return _StartRegion.ActualHeight + _MiddleRegion.ActualHeight + _EndRegion.ActualHeight;

                default:
                    return -1d;
            }
        }

        private void RangeSlider_LowerValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateThumbPosition(ThumbKind.Start);
        }

        private void RangeSlider_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateThumbPosition(ThumbKind.Start);
            UpdateThumbPosition(ThumbKind.End);
        }

        private void RangeSlider_UpperValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateThumbPosition(ThumbKind.End);
        }

        #endregion Private
    }
}