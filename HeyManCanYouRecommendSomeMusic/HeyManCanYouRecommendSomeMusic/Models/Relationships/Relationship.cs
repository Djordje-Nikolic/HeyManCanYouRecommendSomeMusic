﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models.Relationships
{
    public abstract class Relationship
    {
        public abstract string GetName();

        public abstract Tuple<int, int> GetLimits();

        public override string ToString()
        {
            return GetName();
        }
    }
}
