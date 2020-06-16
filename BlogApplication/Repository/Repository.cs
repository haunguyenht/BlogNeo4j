using BlogApplication.Models;
using BlogApplication.ViewModel;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApplication.Repository
{
    public class Neo4jRepository : IRepository
    {
        private readonly IBoltGraphClient _graphClient;

        public Neo4jRepository(IBoltGraphClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task CreateUser(ApplicationUser data)
        {
            var query = _graphClient.Cypher.Write
                .Merge($"(u:User)")
                .OnCreate().Set($"u.Mail = '{data.Email}', " +
                $"u.PhoneNumber = '{data.PhoneNumber}', " +
                $"u.FirstName = '{data.FirstName}', " +
                $"u.LastName = '{data.LastName}'");

            await query.ExecuteWithoutResultsAsync();
        }
    }
}
