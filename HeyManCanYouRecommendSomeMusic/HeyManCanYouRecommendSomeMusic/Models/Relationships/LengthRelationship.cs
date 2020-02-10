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

        public static bool TestForRequirements(LengthType lengthType, Song song)
        {
            if (!int.TryParse(song.duration, out int duration))
            {
                return false;
            }

            switch (lengthType)
            {
                case LengthType.SHORT:
                    if (duration < 180)
                        return true;
                    break;
                case LengthType.MEDIUM:
                    if (duration >= 180 && duration < 300)
                        return true;
                    break;
                case LengthType.LONG:
                    if (duration >= 300 && duration < 480)
                        return true;
                    break;
                case LengthType.EXTRALONG:
                    if (duration >= 480)
                        return true;
                    break;
                default:
                    return false;
            }

            return false;
        }

        public override Tuple<int, int> GetLimits()
        {
            switch (Type)
            {
                case LengthType.SHORT:
                    return new Tuple<int, int>(1, 179);
                case LengthType.MEDIUM:
                    return new Tuple<int, int>(180, 299);
                case LengthType.LONG:
                    return new Tuple<int, int>(300, 479);
                case LengthType.EXTRALONG:
                    return new Tuple<int, int>(480, int.MaxValue);
                default:
                    return null;
            }
        }
    }
}
