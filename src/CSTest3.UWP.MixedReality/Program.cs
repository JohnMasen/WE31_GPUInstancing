using System;
using Windows.ApplicationModel.Core;

namespace CSTest3.UWP.MixedReality
{
    /// <summary>
    /// UWP.MixedReality Holographic application using SharpDX.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        [MTAThread]
        private static void Main()
        {
            var exclusiveViewApplicationSource = new AppViewSource();
            CoreApplication.Run(exclusiveViewApplicationSource);
        }
    }
}