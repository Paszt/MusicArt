using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace MusicArt.Resources
{
    /// <summary>
    /// Very simple Grid Length animation implementation that only handles To type animations, 
    /// the Pixel GridUnitType, and no easing functions.
    /// </summary>
    internal class GridLengthAnimation : AnimationTimeline
    {
        /// <summary>
        /// Static ctor for GridLengthAnimation registers dependency properties.
        /// </summary>
        static GridLengthAnimation() =>
            ToProperty = DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        /// <summary>
        /// Creates a new DoubleAnimation with all properties set to
        /// their default values.
        /// </summary>
        public GridLengthAnimation() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GridLengthAnimation"/> class that animates 
        /// to the specified value over the specified duration. 
        /// <para>
        /// The starting value for the animation is the base value of the property being animated or 
        /// the output from another animation.</para>
        /// </summary>
        /// <param name="toValue">The destination value of the animation.</param>
        /// <param name="duration">The length of time the animation takes to play from start to finish, once. See
        /// the <see cref="Timeline.Duration"/> property for more information.
        /// </param>
        public GridLengthAnimation(double toValue, Duration duration) : this()
        {
            To = new GridLength(toValue);
            Duration = duration;
        }

        public override Type TargetPropertyType => typeof(GridLength);

        protected override Freezable CreateInstanceCore() => new GridLengthAnimation();

        public static readonly DependencyProperty ToProperty;
        public GridLength To { get => (GridLength)GetValue(ToProperty); set => SetValue(ToProperty, value); }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            double fromVal = ((GridLength)defaultOriginValue).Value;
            double toVal = ((GridLength)GetValue(ToProperty)).Value;
            return fromVal > toVal
                ? new GridLength(((1 - animationClock.CurrentProgress.Value) * (fromVal - toVal)) + toVal, GridUnitType.Pixel)
                : (object)new GridLength((animationClock.CurrentProgress.Value * (toVal - fromVal)) + fromVal, GridUnitType.Pixel);
        }
    }
}
