using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic
{
    public enum Relationship { DRONING, SLOW, HEAVY, DISTORTED }

    public static class Settings
    {
        public const string DB_URI = "http://localhost:7474/db/data";
        public const string DB_USERNAME = "neo4j";
        public const string DB_PASSWORD = "penicbagomal";
    }
}
