using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromocionesAPI.Settings
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string DbName { get; set; }
        public string CollectionName { get; set; }
    }
}
