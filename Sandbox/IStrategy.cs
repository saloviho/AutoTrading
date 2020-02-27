﻿using System.Collections.Generic;
using QuotesHistory.Models;

namespace Sandbox
{
    public interface IStrategy
    {
        void LoadHistory(List<Candle> history);
        void OnTick(Candle tick);
    }
}