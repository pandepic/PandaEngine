﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PandaEngine
{
    public class GameTimer
    {
        protected TimeSpan _frameTime;
        public TimeSpan FrameTime { get => _frameTime; }

        public GameTimer()
        {
            _frameTime = TimeSpan.Zero;
        }

        internal void SetFrameTime(TimeSpan frameTime)
        {
            _frameTime = frameTime;
        }
    }
}