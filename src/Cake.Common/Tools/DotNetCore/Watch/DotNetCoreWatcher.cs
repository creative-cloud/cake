// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Tooling;

namespace Cake.Common.Tools.DotNetCore.Watch
{
    /// <summary>
    /// .NET Core project runner.
    /// </summary>
    public sealed class DotNetCoreWatcher : DotNetCoreTool<DotNetCoreWatchSettings>
    {
        private readonly ICakeEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetCoreWatcher" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="processRunner">The process runner.</param>
        /// <param name="tools">The tool locator.</param>
        public DotNetCoreWatcher(
            IFileSystem fileSystem,
            ICakeEnvironment environment,
            IProcessRunner processRunner,
            IToolLocator tools) : base(fileSystem, environment, processRunner, tools)
        {
            _environment = environment;
        }

        /// <summary>
        /// Build the project using the specified path and settings.
        /// </summary>
        /// <param name="project">The target project path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="settings">The settings.</param>
        public void Watch(string project, ProcessArgumentBuilder arguments, DotNetCoreWatchSettings settings)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project));
            }
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            RunCommand(settings, GetArguments(project, arguments, settings));
        }

        private ProcessArgumentBuilder GetArguments(string project, ProcessArgumentBuilder arguments, DotNetCoreWatchSettings settings)
        {
            var builder = CreateArgumentBuilder(settings);

            builder.Append("watch");

            if (project != null)
            {
                builder.Append("--project");
                builder.AppendQuoted(project);
            }

            // Quiet
            if (settings.Quiet)
            {
                builder.Append("--quiet");
            }

            // Verbose
            if (settings.Verbose)
            {
                builder.Append("--verbose");
            }

            // List
            if (settings.List)
            {
                builder.Append("--list");
            }

            // Version
            if (settings.Version)
            {
                builder.Append("--version");
            }

             // Arguments
            if (!arguments.IsNullOrEmpty())
            {
                builder.Append("--");
                arguments.CopyTo(builder);
            }

            return builder;
        }
    }
}
