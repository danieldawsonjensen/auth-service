using MongoDB.Driver;
using System.Threading.Tasks;
using Model;
using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository()
        {
            var client = new MongoClient("mongodb://MyServiceUser:my_$ecure_pa$$word@localhost:27018/?authSource=admin"); // vores mongo conn string
            var database = client.GetDatabase("Vehicles"); // vores database
            _users = database.GetCollection<User>("Brugere");
        }


        public async Task<User> FindUserByUsernameAndPassword(string mongoId, string username, string password)
        {
            var filter = Builders<User>.Filter.Eq("_id", mongoId) & Builders<User>.Filter.Eq("Username", username) & Builders<User>.Filter.Eq("Password", password);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }
    }
}
