using HeyManCanYouRecommendSomeMusic.Models;
using HeyManCanYouRecommendSomeMusic.Models.Relationships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Helpers
{
    public static class SongAnalyzer
    {
        internal static IEnumerable<Relationship> AnalyzeForRelationships(Song current)
        {
            List<Relationship> result = new List<Relationship>();

            var lengthTestRes = LengthRelationship.TestSong(current);
            if (lengthTestRes != null)
                result.Add(lengthTestRes);

            var tempoTestRes = TempoRelationship.TestSong(current);
            if (tempoTestRes != null)
                result.Add(tempoTestRes);

            return result;
        }
    }
}
