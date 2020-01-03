using Neo4j.Driver;
using Neo4jClient;
using System;
using Neo4jClient.Cypher;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyManCanYouRecommendSomeMusic.Models;

namespace HeyManCanYouRecommendSomeMusic.Services
{
    public interface IDBService
    {
        bool AddNewSong(Song s);
    }

    public class DBService : IDBService
    {
        private GraphClient client;

        public DBService()
        {
            client = new GraphClient(new Uri(Settings.DB_URI), Settings.DB_USERNAME, Settings.DB_PASSWORD);

            try
            {
                client.Connect();
            }
            catch(Exception e)
            {
                //ignored
            }
        }

        public bool AddNewSong(Song song)
        {
            Dictionary<string, object> cypherDict = new Dictionary<string, object>();

            string maxId = GetMaxId();

            try
            {
                int max = Int32.Parse(maxId);
                song.Id = (++max).ToString();
            }
            catch (Exception e)
            {
                song.Id = "";
            }

            cypherDict.Add("id", song.Id);
            cypherDict.Add("name", song.Name);
            cypherDict.Add("band", song.Band);
            cypherDict.Add("genre", song.Genre);

            var cypher = new CypherQuery("CREATE(s: Song { id:'" + song.Id + "', name: '" + song.Name 
                                                              + "', band:'" + song.Band + "', genre:'" + song.Genre
                                                              + "'}) return s",
                                        cypherDict, CypherResultMode.Set);

            Song s = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).FirstOrDefault();

            if (s != null)
                return true;

            return false;
        }

        private string GetMaxId()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where exists(n.id) return max(n.id)",
                                                            new Dictionary<string, object>(), CypherResultMode.Set);

            string maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query).ToList().FirstOrDefault();

            return maxId;
        }
    }
}
