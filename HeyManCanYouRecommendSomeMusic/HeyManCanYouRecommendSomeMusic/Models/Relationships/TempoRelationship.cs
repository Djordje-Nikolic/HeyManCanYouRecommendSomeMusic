using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models.Relationships
{
    public class TempoRelationship : Relationship
    {
        public enum TempoType
        {
            SLOW, MODERATE, FAST, EXTRAFAST
        }

        public TempoType Type { get; private set; }

        public TempoRelationship(TempoType tempoType)
        {
            Type = tempoType;
        }

        public override string GetName()
        {
            return Type.ToString();
        }

        public static Relationship TestSong(Song song)
        {
            bool success = double.TryParse(song.bpm, out double bpm);

            if (!success || bpm <= 0)
            {
                return null;
            }

            if (bpm < 80)
                return new TempoRelationship(TempoType.SLOW);
            else if (bpm <= 125)
                return new TempoRelationship(TempoType.MODERATE);
            else if (bpm <= 180)
                return new TempoRelationship(TempoType.FAST);
            else
                return new TempoRelationship(TempoType.EXTRAFAST);
        }
    }
}
