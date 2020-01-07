using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HeyManCanYouRecommendSomeMusic.Models.Deezer
{
    public class DeezerObject : IEquatable<DeezerObject>
    {
        [JsonPropertyName("id")] public int ID { get; set; }
        [JsonPropertyName("type")] public string ObjectType { get; set; }

        public bool IsValidObject()
        {
            return ObjectType != null;
        }

        public bool Equals([AllowNull] DeezerObject other)
        {
            if (other == null) return false;
            return (other.ID.Equals(ID)) && (other.ObjectType.Equals(ObjectType));
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + ID.GetHashCode();
            hash = (hash * 7) + ObjectType.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            DeezerObject objAsDeezerObj = obj as DeezerObject;
            if (objAsDeezerObj == null) return false;
            else return Equals(objAsDeezerObj);
        }
    }
}
