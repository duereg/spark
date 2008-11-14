﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Scripting.Hosting;

namespace Spark.Scripting
{
    public interface IScriptingSparkView : ISparkView
    {
        string ScriptSource { get; }
        CompiledCode CompiledCode { get; set; }
    }
}
