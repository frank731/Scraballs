// Copyright (c) AdHawk Microsystems Inc.
// All rights reserved.
namespace AdhawkApi.Numerics.Filters
{
    /// <summary>
    /// Data Filter types natively supported by the API
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// Moving average filter
        /// </summary>
        MovingAverage,
        /// <summary>
        /// Single-pole lowpass filter (for visualizing smooth data)
        /// </summary>
        ExponentialMovingAverage,
        /// <summary>
        /// One-Euro filter (for visualizing fast data)
        /// </summary>
        OneEuro,
        /// <summary>
        /// Median filter (for outlier rejection)
        /// </summary>
        Median,
        /// <summary>
        /// Raw filter (for unfiltered data)
        /// </summary>
        Raw
    };
}