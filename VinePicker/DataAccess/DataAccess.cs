using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using VinePicker.Models;

using Dapper;
using System.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;
using System.IO;
using Remotion.Linq.Clauses;

namespace VinePicker.DataAccess
{
    public static class DataAccess
    {
        public static string CnnStr { get; set; }

        public static Vine GetVine()
        {
            using (IDbConnection connection = new SqlConnection(CnnStr))
            {
                // order by newid then get the first vine (random)
                return connection.QueryFirst<Vine>("SELECT TOP 1 * FROM Vine ORDER BY NEWID()");
            }
        }

        // Check if the vine with this permalink already exists in the db
        public static bool VineExists(string permalink)
        {
            using (IDbConnection connection = new SqlConnection(CnnStr))
            {
                return connection.ExecuteScalar<int>($"SELECT COUNT(*) FROM Vine WHERE Permalink = '{permalink}'") > 0;
            }
        }

        public static Vine GetSimilarVine(int vineId, int rating)
        {
            using (IDbConnection connection = new SqlConnection(CnnStr))
            {
                // Query for 8 of the closest ranked different vines, select one of those randomly
                return connection.QueryFirst<Vine>(
                    $"SELECT TOP 1 * FROM (SELECT TOP 8 * FROM VINE WHERE VineId <> {vineId} ORDER BY ABS({rating} - Rating)) AS Randoms ORDER BY NEWID()");
            }
        }

        public static void AddVine(Vine vine)
        {
            using (IDbConnection connection = new SqlConnection(CnnStr))
            {
                string sql =
                    "INSERT INTO Vine (Description, VideoUrl, Created, Permalink, Loops, Likes, Username, Rating, Submitter) " +
                    "VALUES (@Description, @VideoUrl, @Created, @Permalink, @Loops, @Likes, @Username, @Rating, @Submitter)";
                connection.Execute(sql, vine);
            }
        }

        public static int GetVineRating(int VineId)
        {
            using (IDbConnection connection = new SqlConnection(CnnStr))
            {
                return connection.QueryFirst($"SELECT Rating FROM Vine WHERE VineId = {VineId}");
            }
        }

        public static void SetVineRating(int VineId, int rating)
        {
            using (IDbConnection connection = new SqlConnection(CnnStr))
            {
                string sql = $"UPDATE Vine SET Rating = @Rating WHERE VineId = @VineId";
                connection.Execute(sql, new {Rating = rating, VineId = VineId});
            }
        }
    }
}
