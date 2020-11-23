// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Core.IO;

namespace Cake.Common.Tools.DotNetCore.Watch
{
    /// <summary>
    /// Contains settings used by <see cref="DotNetCoreWatcher" />.
    /// </summary>
    public sealed class DotNetCoreWatchSettings : DotNetCoreSettings
    {
        // TODO: change comments to suit quiet
        /// <summary>
        /// Gets or sets a value indicating whether or not to use verbose level logging.
        /// </summary>
        public bool Quiet { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use verbose level logging.
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use verbose level logging.
        /// </summary>
        public bool List { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use verbose level logging.
        /// </summary>
        public bool Version { get; set; }

        /// <summary>
        /// Gets or sets additional arguments to be passed to MSBuild.
        /// </summary>
        public DotNetCoreMSBuildSettings MSBuildSettings { get; set; }
        // TODO: check to see missing functions
    }
}
