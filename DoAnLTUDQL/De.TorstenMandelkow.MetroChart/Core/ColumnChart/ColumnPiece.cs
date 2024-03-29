﻿namespace De.TorstenMandelkow.MetroChart
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Windows.Input;

#if NETFX_CORE
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Shapes;
    using Windows.UI.Xaml.Markup;
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Core;
#else
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Controls;

#endif

    [TemplateVisualState(Name = ColumnPiece.StateSelectionUnselected, GroupName = ColumnPiece.GroupSelectionStates)]
    [TemplateVisualState(Name = ColumnPiece.StateSelectionSelected, GroupName = ColumnPiece.GroupSelectionStates)]
    public class ColumnPiece : Control
    {
        #region Fields

        internal const string StateSelectionUnselected = "Unselected";
        internal const string StateSelectionSelected = "Selected";
        internal const string GroupSelectionStates = "SelectionStates";

        public static readonly DependencyProperty ClientHeightProperty =
            DependencyProperty.Register("ClientHeight", typeof(double), typeof(ColumnPiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnSizeChanged)));
        public static readonly DependencyProperty ClientWidthProperty =
            DependencyProperty.Register("ClientWidth", typeof(double), typeof(ColumnPiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnSizeChanged)));        
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(ColumnPiece),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnPercentageChanged)));
        public static readonly DependencyProperty ColumnHeightProperty =
            DependencyProperty.Register("ColumnHeight", typeof(double), typeof(ColumnPiece),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ColumnPiece),
            new PropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));
        public static readonly DependencyProperty IsClickedByUserProperty =
            DependencyProperty.Register("IsClickedByUser", typeof(bool), typeof(ColumnPiece),
            new PropertyMetadata(false));

        private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ColumnPiece).DrawGeometry();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColumnPiece source = (ColumnPiece)d;
            bool oldValue = (bool)e.OldValue;
            bool newValue = (bool)e.NewValue;
            source.OnIsSelectedPropertyChanged(oldValue, newValue);
        }

        protected virtual void OnIsSelectedPropertyChanged(bool oldValue, bool newValue)
        {
            this.IsClickedByUser = false;
            VisualStateManager.GoToState(this, newValue ? StateSelectionSelected : StateSelectionUnselected, true);
        }

        #endregion Fields

        #region Constructors

        static ColumnPiece()        
        {
#if NETFX_CORE
                        
#elif SILVERLIGHT
    
#else
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColumnPiece), new FrameworkPropertyMetadata(typeof(ColumnPiece)));
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnPiece"/> class.
        /// </summary>
        public ColumnPiece()
        {
#if NETFX_CORE
            this.DefaultStyleKey = typeof(ColumnPiece);
#endif
#if SILVERLIGHT
            this.DefaultStyleKey = typeof(ColumnPiece);
#endif
            Loaded += ColumnPiece_Loaded;
        }

#if NETFX_CORE
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InternalOnApplyTemplate();
        }
#else
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            InternalOnApplyTemplate();
        }
#endif

        Border slice = null;
        private void InternalOnApplyTemplate()
        {
            slice = this.GetTemplateChild("Slice") as Border;
            if (slice != null)
            {
#if NETFX_CORE
                slice.PointerPressed += delegate
#else
                slice.MouseLeftButtonUp += delegate
#endif
                {
                    InternalMousePressed();
                };

#if NETFX_CORE
                slice.PointerMoved += delegate
#else
                slice.MouseMove += delegate
#endif
                {
                    //InternalMouseMoved();
                };
            }
        }

        private void InternalMousePressed()
        {
            SetValue(ColumnPiece.IsClickedByUserProperty, true);
        }

        private static void UpdatePie(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ColumnPiece).DrawGeometry();
        }

        void ColumnPiece_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGeometry();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the height of the client.
        /// </summary>
        /// <value>The height of the client.</value>
        public string Caption
        {
            get
            {
                return (this.DataContext as DataPoint).DisplayName;
            }
        }

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ColumnPiece).DrawGeometry();
        }
        
        /// <summary>
        /// Gets or sets the height of the client.
        /// </summary>
        /// <value>The height of the client.</value>
        public double ClientHeight
        {
            get { return (double)GetValue(ClientHeightProperty); }
            set { SetValue(ClientHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width of the client.
        /// </summary>
        /// <value>The width of the client.</value>
        public double ClientWidth
        {
            get { return (double)GetValue(ClientWidthProperty); }
            set { SetValue(ClientWidthProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public bool IsClickedByUser
        {
            get { return (bool)GetValue(IsClickedByUserProperty); }
            set { SetValue(IsClickedByUserProperty, value); }
        }

        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        public double ColumnHeight
        {
            get { return (double)GetValue(ColumnHeightProperty); }
            set { SetValue(ColumnHeightProperty, value); }
        }
 
        #endregion Properties

        #region Methods

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="context">The context.</param>
        private void DrawGeometry()
        {    
            try
            {
                if (this.ClientWidth <= 0.0)
                {
                    return;
                }
                if (this.ClientHeight <= 0.0)
                {
                    return;
                }

                double startHeight = 0;
                if (slice.Height > 0)
                {
                    startHeight = slice.Height;
                }

                DoubleAnimation scaleAnimation = new DoubleAnimation();
                scaleAnimation.From = startHeight;
                scaleAnimation.To = this.ClientHeight * Percentage;
                scaleAnimation.Duration = TimeSpan.FromMilliseconds(500);
                scaleAnimation.EasingFunction = new QuarticEase() { EasingMode = EasingMode.EaseOut };
                Storyboard storyScaleX = new Storyboard();
                storyScaleX.Children.Add(scaleAnimation);
                
                Storyboard.SetTarget(storyScaleX, slice);

#if NETFX_CORE
                scaleAnimation.EnableDependentAnimation = true;
                Storyboard.SetTargetProperty(storyScaleX, "Height");
#else
                Storyboard.SetTargetProperty(storyScaleX, new PropertyPath("Height"));
#endif
                storyScaleX.Begin();
         
                
                //SetValue(ColumnPiece.ColumnHeightProperty, this.ClientHeight * Percentage);
            }
            catch (Exception ex)
            {
            }
        }

        
#if NETFX_CORE
         protected override void OnPointerPressed(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            HandleMouseDown();
            e.Handled = true;
        }
#else
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            HandleMouseDown();
            e.Handled = true;
        }
#endif

         private void HandleMouseDown()
         {
             IsClickedByUser = true; // (ModifierKeys.None == (ModifierKeys.Control & Keyboard.Modifiers));
             /*if (IsSelectionEnabled)
            {
                IsSelected = (ModifierKeys.None == (ModifierKeys.Control & Keyboard.Modifiers));
                e.Handled = true;
            }
             * */
         }

        #endregion Methods
    }
}