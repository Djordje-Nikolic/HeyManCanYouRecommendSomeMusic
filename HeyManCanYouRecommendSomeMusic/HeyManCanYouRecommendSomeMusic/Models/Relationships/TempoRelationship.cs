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

        public static bool TestForRequirements(TempoType lengthType, Song song)
        {
            if (!int.TryParse(song.bpm, out int bpm))
            {
                return false;
            }

            switch (lengthType)
            {
                case TempoType.SLOW:
                    if (bpm < 80)
                        return true;
                    break;
                case TempoType.MODERATE:
                    if (bpm >= 80 && bpm <= 125)
                        return true;
                    break;
                case TempoType.FAST:
                    if (bpm > 125 && bpm <= 180)
                        return true;
                    break;
                case TempoType.EXTRAFAST:
                    if (bpm > 180)
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
                case TempoType.SLOW:
                    return new Tuple<int, int>(1, 79);
                case TempoType.MODERATE:
                    return new Tuple<int, int>(80, 125);
                case TempoType.FAST:
                    return new Tuple<int, int>(126, 180);
                case TempoType.EXTRAFAST:
                    return new Tuple<int, int>(181, int.MaxValue);
                default:
                    return null;
            }
        }
    }
}
