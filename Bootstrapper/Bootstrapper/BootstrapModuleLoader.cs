using System;
using System.Collections.Generic;

namespace Bootstrapper
{
    public interface BootstrapModuleLoader<out TBootstrapModule>
    {
        IEnumerable<String> SupportedExtensions { get; }

        IEnumerable<TBootstrapModule> LoadModules(IEnumerable<String> fileNames);
    }
}