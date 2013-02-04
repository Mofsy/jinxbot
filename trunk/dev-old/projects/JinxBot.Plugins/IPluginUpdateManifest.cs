using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JinxBot.Plugins
{
    /// <summary>
    /// When implemented, provides information to the JinxBot automatic updater about how to update a plugin.
    /// </summary>
    /// <remarks>
    /// <para>This interface will remain empty for JinxBot Beta 1, as automatic updating will not be 
    /// available.  However, plugin authors should prepare to implement it for JinxBot Beta 2.</para>
    /// </remarks>
    public interface IPluginUpdateManifest
    {
    }
}
