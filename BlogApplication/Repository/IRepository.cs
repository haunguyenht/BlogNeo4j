using BlogApplication.Models;
using BlogApplication.ViewModel;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogApplication.Repository
{
    public interface IRepository
    {
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task CreateUser(ApplicationUser data);
    }
}
