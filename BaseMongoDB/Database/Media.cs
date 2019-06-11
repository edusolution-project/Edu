using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace BaseMongoDB.Database
{
    public class Media
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public DateTime Created { get; set; }
        public double Size { get; set; }
    }
}
