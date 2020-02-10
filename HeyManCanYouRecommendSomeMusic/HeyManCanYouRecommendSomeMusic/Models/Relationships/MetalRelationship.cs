using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models.Relationships
{
    public class MetalRelationship : Relationship
    {
        public enum MetalRelationshipType { DRONING, EPIC, ENERGETIC, DISTORTED, HEAVY }

        public MetalRelationshipType Type { get; private set; }

        public MetalRelationship(MetalRelationshipType type)
        {
            this.Type = type;
        }

        public override Tuple<int, int> GetLimits()
        {
            throw new NotImplementedException();
        }

        public override string GetName()
        {
            return this.Type.ToString();
        }
    }
}
