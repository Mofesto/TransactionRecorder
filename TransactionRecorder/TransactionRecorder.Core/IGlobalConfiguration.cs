﻿using System;
using System.ComponentModel;

namespace TransactionRecorder
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IGlobalConfiguration<out T> : IGlobalConfiguration
    {
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        T Entry { get; }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IGlobalConfiguration
    {
    }
}
