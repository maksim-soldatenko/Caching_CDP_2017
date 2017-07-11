using System;
using CachingDemo.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Amazon.ElastiCacheCluster;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;

namespace CachingDemo.Controllers
{
    public class HomeController : Controller
    {
        private const string CollectionName = "my_entries_collection";
        private readonly MemcachedClient _client;

        public HomeController()
        {
            var config = new ElastiCacheClusterConfig("cachedemo.o3t7m9.cfg.euc1.cache.amazonaws.com", 11211);
            _client = new MemcachedClient(config);
        }

        public ActionResult Index()
        {
            var watch = new Stopwatch();
            watch.Start();

            List<Entry> model = null;

            var cached = _client.Get<string>(CollectionName);
            
            if (cached == null)
            {
                model = GetFromDb().ToList();
                var str = JsonConvert.SerializeObject(model);
                _client.Store(StoreMode.Set, CollectionName, str);
                watch.Stop();
                ViewBag.Title = String.Format("From cache: Cache Miss. Time: {0}", watch.ElapsedMilliseconds);
            }
            else
            {
                var obj = JsonConvert.DeserializeObject<List<Entry>>(cached);
                model = obj;
                watch.Stop();
                ViewBag.Title = String.Format("From cache: Cache HIT. Time: {0}", watch.ElapsedMilliseconds);
            }



            return View("DirectFromDb", model);
        }

        public ActionResult DirectFromDb()
        {
            var watch = new Stopwatch();
            watch.Start();
            var model = GetFromDb();
            ViewBag.Title = String.Format("Direct From Db. Time: {0}", watch.ElapsedMilliseconds);
            return View(model);
        }

        public EmptyResult ClearCache()
        {
            _client.Remove(CollectionName);

            return new EmptyResult();
        }

        private IEnumerable<Entry> GetFromDb()
        {
            var result = new List<Entry>();

            //const string connectionString =
            //    "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AdventureWorks2014;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            const string connectionString = "Data Source=cachingdemo.c7qyehdkea8c.eu-central-1.rds.amazonaws.com;Initial Catalog=AdventureWorks2014; User Id=Soldier; Password=Soldier_1131";


            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT TOP 5000 * FROM Sales.SalesOrderDetail s INNER JOIN Production.Product p ON s.ProductID = p.ProductID";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var entry = new Entry()
                            {
                                CarrierTrackingNumber = reader["CarrierTrackingNumber"].ToString(),
                                Color = reader["Color"].ToString(),
                                Name = reader["Name"].ToString(),
                                ProductId = (int) reader["ProductId"],
                                ProductNumber = reader["ProductNumber"].ToString(),
                                SalesOrderId = (int) reader["SalesOrderId"],
                                UnitPrice = (decimal) reader["UnitPrice"]
                            };

                            result.Add(entry);
                        }
                    }
                    
                }
            }
            return result;
        }
    }
}