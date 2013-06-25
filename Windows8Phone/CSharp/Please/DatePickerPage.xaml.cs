// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Primitives;

using Please.DatePicker;

namespace Please
{
    /// <summary>
    /// Represents a page used by the DatePicker control that allows the user to choose a date (day/month/year).
    /// </summary>
    public partial class DatePickerPage : CustomDateTimePickerPageBase
    {
        /// <summary>
        /// Initializes a new instance of the DatePickerPage control.
        /// </summary>
        public DatePickerPage()
        {
            InitializeComponent();
       
            // Hook up the data sources
            PrimarySelector.DataSource = new YearDataSource();
            SecondarySelector.DataSource = new MonthDataSource();
            TertiarySelector.DataSource = new DayDataSource();

            // TODO: need to be able to set this dynamically
            DateTime start = DateTime.Now;

            InitializeDateTimePickerPage(PrimarySelector, SecondarySelector, TertiarySelector, start);
        }
        
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            App.userChoice = Value.Value.ToString("MMMM d yyyy");
        }
     
        /// <summary>
        /// Gets a sequence of LoopingSelector parts ordered according to culture string for date/time formatting.
        /// </summary>
        /// <returns>LoopingSelectors ordered by culture-specific priority.</returns>
        protected override IEnumerable<LoopingSelector> GetSelectorsOrderedByCulturePattern()
        {
            string pattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern.ToUpperInvariant();

            if (DateShouldFlowRTL())
            {
                char[] reversedPattern = pattern.ToCharArray();
                Array.Reverse(reversedPattern);
                pattern = new string(reversedPattern);
            }

            return GetSelectorsOrderedByCulturePattern(
                pattern,
                new char[] { 'Y', 'M', 'D' },
                new LoopingSelector[] { PrimarySelector, SecondarySelector, TertiarySelector });
        }

        /// <summary>
        /// Handles changes to the page's Orientation property.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (null == e)
            {
                throw new ArgumentNullException("e");
            }

            base.OnOrientationChanged(e);
            SystemTrayPlaceholder.Visibility = (0 != (PageOrientation.Portrait & e.Orientation)) ?
                Visibility.Visible :
                Visibility.Collapsed;
        }

        /// <summary>
        /// Sets the selectors and title flow direction.
        /// </summary>
        /// <param name="flowDirection">Flow direction to set.</param>
        internal override void SetFlowDirection(FlowDirection flowDirection)
        {
            HeaderTitle.FlowDirection = flowDirection;

            PrimarySelector.FlowDirection = flowDirection;
            SecondarySelector.FlowDirection = flowDirection;
            TertiarySelector.FlowDirection = flowDirection;
        }

        /// <summary>
        /// Date should flow from right to left for arabic and persian.
        /// </summary>
        internal static bool DateShouldFlowRTL()
        {
            string lang = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return lang == "ar" || lang == "fa";
        }
    }
}