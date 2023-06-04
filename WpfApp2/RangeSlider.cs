using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        #endregion Maximum

        #region Minimum

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(RangeSlider),
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        #endregion Minimum

        #region LowerValue

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register(nameof(LowerValue), typeof(double), typeof(RangeSlider),
                new FrameworkPropertyMetadata(25d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnLowerValueChanged, OnCoerceLowerValue));

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
                return slider.UpperValue;
            }
            if (num < slider.Minimum)
            {
                return slider.Minimum;
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
                new FrameworkPropertyMetadata(75d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnUpperValueChanged, OnCoerceUpperValue));

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
                return slider.LowerValue;
            }
            if (num > slider.Maximum)
            {
                return slider.Maximum;
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

        private void StartRegion_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_StartThumb == null || _EndThumb == null || _StartRegion == null || _MiddleRegion == null || _EndRegion == null)
            {
                return;
            }

            var total = ActualWidth - _StartThumb.ActualWidth - _EndRegion.ActualWidth;
            if (e.WidthChanged)
            {
                LowerValue = e.NewSize.Width / total * (Maximum - Minimum);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _StartThumb = GetTemplateChild(PART_StartThumb) as System.Windows.Controls.Primitives.Thumb;
            _EndThumb = GetTemplateChild(PART_EndThumb) as System.Windows.Controls.Primitives.Thumb;
            _StartRegion = GetTemplateChild(PART_StartRegion) as System.Windows.Controls.Primitives.RepeatButton;
            _MiddleRegion = GetTemplateChild(PART_MiddleRegion) as System.Windows.Controls.Primitives.RepeatButton;
            _EndRegion = GetTemplateChild(PART_EndRegion) as System.Windows.Controls.Primitives.RepeatButton;

            if (_StartRegion != null)
            {
                _StartRegion.SizeChanged += StartRegion_SizeChanged;
            }
        }

        #endregion Override

        #region LowerValueChanged

        public static readonly RoutedEvent LowerValueChangedEvent =
            EventManager.RegisterRoutedEvent("LowerValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RangeSlider));

        public event RoutedPropertyChangedEventHandler<double> LowerValueChanged
        {
            add { AddHandler(LowerValueChangedEvent, value); }
            remove { RemoveHandler(LowerValueChangedEvent, value); }
        }

        #endregion LowerValueChanged

        #region UpperValueChanged

        public static readonly RoutedEvent UpperValueChangedEvent =
            EventManager.RegisterRoutedEvent("UpperValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RangeSlider));

        public event RoutedPropertyChangedEventHandler<double> UpperValueChanged
        {
            add { AddHandler(UpperValueChangedEvent, value); }
            remove { RemoveHandler(UpperValueChangedEvent, value); }
        }

        #endregion UpperValueChanged

        #region DragEvent

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
            var thumb = e.OriginalSource as Thumb;
            if (thumb == null)
            {
                return;
            }
            if (_StartRegion == null || _MiddleRegion == null || _EndRegion == null)
            {
                return;
            }

            var offset = 0d;
            if (Orientation == Orientation.Horizontal)
            {
                offset = e.HorizontalChange;
            }
            else
            {
                offset = e.VerticalChange;
            }

            if (thumb == _StartThumb)
            {
                if (e.HorizontalChange > 0)
                {
                    _StartRegion.Width += e.HorizontalChange;
                }
                else if (e.HorizontalChange < 0)
                {
                    if (_StartRegion.Width - Math.Abs(offset) < 0)
                    {
                        _StartRegion.Width = 0;
                        return;
                    }
                    _StartRegion.Width -= Math.Abs(e.HorizontalChange);
                }
            }
            else if (thumb == _EndThumb)
            {
                if (e.HorizontalChange > 0)
                {
                    _EndRegion.Width -= e.HorizontalChange;
                }
                else if (e.HorizontalChange < 0)
                {
                    _EndRegion.Width += Math.Abs(e.HorizontalChange);
                }
            }
        }

        private void OnDragCompletedEvent(DragCompletedEventArgs e)
        {
        }

        #endregion DragEvent
    }
}