using MongoDB.Driver;
using System.Threading.Tasks;
using Model;

namespace Model
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository()
        {
            var client = new MongoClient("mongodb://root:root@localhost:27018/?authMechanism=DEFAULT&authSource=admin"); // vores mongo conn string
            var database = client.GetDatabase("Vehicles"); // vores database
            _users = database.GetCollection<User>("Brugere");
        }


        public async Task<User> FindUserByUsernameAndPassword(string username, string password)
        {
            var filter = Builders<User>.Filter.Eq("Username", username) & Builders<User>.Filter.Eq("Password", password);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }
    }
}
