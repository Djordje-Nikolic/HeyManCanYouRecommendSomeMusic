﻿using Neo4j.Driver;
using Neo4jClient;
using System;
using Neo4jClient.Cypher;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HeyManCanYouRecommendSomeMusic.Models;
using HeyManCanYouRecommendSomeMusic.Models.Relationships;
using Relationship = HeyManCanYouRecommendSomeMusic.Models.Relationships.Relationship;

namespace HeyManCanYouRecommendSomeMusic.Services
{
    public interface IDBService
    {
        bool AddNewSong(Song s);
        List<Song> GetSongsWithSameArtist(Song song, int limit = 5);
        List<Song> GetSongsInSameGenre(Song song, int limit = 5);
        Song GetSongByNameAndArtist(string name, string artist);
        Song GetSongById(int id);
        void CreateRelationship(Song s1, Song s2, Models.Relationships.Relationship rel);
        List<Song> GetSongsInRelationship(Models.Relationships.Relationship rel, int count = 5);
        List<Song> GetSimilarSongs(Song song, Relationship rel, int depth = 0);
        List<Song> GetSongWithinDuration(int lowerLimit, int upperLimit);
        List<Song> GetSongWithinTempo(int lowerLimit, int upperLimit);
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
                song.id = (++max).ToString();
            }
            catch (Exception e)
            {
                song.id = "";
            }

            cypherDict.Add("id", song.id);
            cypherDict.Add("name", song.name);
            cypherDict.Add("band", song.band);
            cypherDict.Add("genre", song.genre);
            cypherDict.Add("duration", song.duration);
            cypherDict.Add("bpm", song.bpm);

            var cypher = new CypherQuery("CREATE(s: Song { id:'" + song.id + "', name: '" + song.name 
                                                              + "', band:'" + song.band + "', genre:'" + song.genre
                                                              + "', duration:'" + song.duration + "', bpm:'" + song.bpm + "'}) return s",
                                        cypherDict, CypherResultMode.Set);

            Song s = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).FirstOrDefault();

            if (s != null)
                return true;

            return false;
        }

        public List<Song> GetSongsWithSameArtist(Song song, int limit = 5)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();

            queryDict.Add("band", song.band);

            var cypher = new CypherQuery("MATCH (s:Song) WHERE s.band = {band} return s LIMIT " + limit,
                                          queryDict, CypherResultMode.Set);

            List<Song> songs = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).ToList();

            return songs;
        }

        public List<Song> GetSongsInSameGenre(Song song, int limit = 5)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("genre", song.genre);

            var cypher = new CypherQuery("MATCH (s:Song) WHERE s.genre = {genre} return s LIMIT " + limit,
                                          queryDict, CypherResultMode.Set);

            List<Song> songs = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).ToList();

            return songs;
        }

        public Song GetSongByNameAndArtist(string name, string artist)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("name", name);
            queryDict.Add("artist", artist);

            var cypher = new CypherQuery("MATCH (s:Song) WHERE s.name = {name} AND s.band = {artist} return s ",
                                          queryDict, CypherResultMode.Set);

            Song s = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).FirstOrDefault();
            return s;
        }

        public Song GetSongById(int id)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", id.ToString());

            var cypher = new CypherQuery("MATCH (s:Song) WHERE s.id = {id} return s ",
                                          queryDict, CypherResultMode.Set);

            Song s = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).FirstOrDefault();
            return s;
        }

        public void CreateRelationship(Song s1, Song s2, Models.Relationships.Relationship rel)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id1", s1.id);
            queryDict.Add("id2", s2.id);

            var cypher = new CypherQuery("MATCH (s1:Song), (s2:Song) " +
                                         "WHERE s1.id = {id1} AND s2.id = {id2}" +
                                          "CREATE (s1)-[:" + rel.ToString() + "]->(s2)", queryDict, CypherResultMode.Set);

            ((IRawGraphClient)client).ExecuteCypher(cypher);
           //((IRawGraphClient)client).ExecuteGetCypherResults<Models.Relationships.Relationship>(cypher);       
        }

        public List<Song> GetSongsInRelationship(Models.Relationships.Relationship rel, int count = 5)
        {
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            var cypher = new CypherQuery("MATCH (s)-[:" + rel.ToString() + "]-(s1) RETURN s LIMIT " + count, queryDict, CypherResultMode.Set);

            try
            {
                List<Song> songs = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).GroupBy(s => s.id).Select(y => y.First()).ToList();
                return songs;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public List<Song> GetSimilarSongs(Song song, Relationship rel, int depth = 0)
        {
            List<Song> songs = new List<Song>();
            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("id", song.id);

            string query = "MATCH (s0:Song)-[:" + rel.ToString() + "]-(s1:Song) WHERE s0.id = {id} RETURN s1";
            var cypher = new CypherQuery(query, queryDict, CypherResultMode.Set);
            
            try
            {
                songs.AddRange(((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).ToList());
            }
            catch(Exception e)
            {
                return null;
            }

            if(depth != 0)
            {
                string matchText = "MATCH (s0:Song)-[:" + rel.ToString() + "]-";
                string returnText = "(s1:Song) WHERE s0.id = {id} RETURN s1";

                for(int i = 0; i < depth; i++)
                {
                    matchText += "()-[:" + rel + "]-";
                    query = matchText + returnText;
                    cypher = new CypherQuery(query, queryDict, CypherResultMode.Set);
                    try
                    {
                        songs.AddRange(((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).ToList());
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                }
            }
            return songs;
        }

        private string GetMaxId()
        {
            var query = new Neo4jClient.Cypher.CypherQuery("start n=node(*) where exists(n.id) return max(n.id)",
                                                            new Dictionary<string, object>(), CypherResultMode.Set);

            string maxId = ((IRawGraphClient)client).ExecuteGetCypherResults<string>(query).ToList().FirstOrDefault();

            return maxId;
        }

        public List<Song> GetSongWithinDuration(int lowerLimit, int upperLimit)
        {
            if (lowerLimit <= 0) lowerLimit = 1;

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("lowerLimit", lowerLimit);
            queryDict.Add("upperLimit", upperLimit);

            var cypher = new CypherQuery("MATCH (s:Song) WHERE toInteger(s.duration) > {lowerLimit} AND toInteger(s.duration) < {upperLimit} RETURN s ",
                                            queryDict, CypherResultMode.Set);

            List<Song> songs = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).ToList();
            return songs;
        }

        public List<Song> GetSongWithinTempo(int lowerLimit, int upperLimit)
        {
            if (lowerLimit <= 0) lowerLimit = 1;

            Dictionary<string, object> queryDict = new Dictionary<string, object>();
            queryDict.Add("lowerLimit", lowerLimit);
            queryDict.Add("upperLimit", upperLimit);

            var cypher = new CypherQuery("MATCH (s:Song) WHERE toInteger(s.bpm) > {lowerLimit} AND toInteger(s.bpm) < {upperLimit} RETURN s ",
                                            queryDict, CypherResultMode.Set);

            List<Song> songs = ((IRawGraphClient)client).ExecuteGetCypherResults<Song>(cypher).ToList();
            return songs;
        }
    }
}
