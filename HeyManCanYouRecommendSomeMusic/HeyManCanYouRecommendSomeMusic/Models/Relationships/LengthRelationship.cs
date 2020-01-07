using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models.Relationships
{
    public class LengthRelationship : Relationship
    {
        public enum LengthType
        {
            SHORT, MEDIUM, LONG, EXTRALONG
        }

        public LengthType Type { get; private set; }

        public LengthRelationship(LengthType lengthType)
        {
            Type = lengthType;
        }

        public override string GetName()
        {
            return Type.ToString();
        }

        public static Relationship TestSong(Song song)
        {
            bool success = int.TryParse(song.duration, out int duration);

            if (!success || duration <= 0)
            {
                return null;
            }

            if (duration < 180)
                return new LengthRelationship(LengthType.SHORT);
            else if (duration < 300)
                return new LengthRelationship(LengthType.MEDIUM);
            else if (duration < 480)
                return new LengthRelationship(LengthType.LONG);
            else
                return new LengthRelationship(LengthType.EXTRALONG);
        }
    }
}
