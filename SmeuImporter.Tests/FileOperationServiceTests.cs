using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SmeuImporter.Domain;
using SmeuImporter.Services;
using SmeuImporter.Services.Implementation;

namespace SmeuImporter.Tests
{
    public class FileOperationServiceTests
    {
        private IFileOperationService fileOperationService;
        private const string Author1 = "xxxxx";
        [SetUp]
        public void Setup()
        {
            fileOperationService = new FileOperationService();
            File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "/userids.json",Json);
        }

        [Test]
        public void GetUserMap_WithJson_ExpectIdsParsed()
        {

            var result = fileOperationService.GetUserMap();

            result.Count().Should().Be(2);
            result.First().Id.Should().Be(221334891705401344);
            result.Last().Id.Should().Be(229655958622568448);
        }

        [Test]
        public void GetUserMap_WithJson_ExpectNamesParsed()
        {
            var result = fileOperationService.GetUserMap();
            
            result.First().Names.Count().Should().Be(2);
            result.Last().Names.Count().Should().Be(1);
            result.First().Names.Should().Contain("Luus");
            result.First().Names.Should().Contain("xxx");
            result.Last().Names.Should().Contain("Justin Dekkers");
        }

        [Test]
        public void SetUserMap_WithJson_ExpectJsonCorrectlyWritten()
        {
            var user1 = SmeuTestDataFactory.NewUser(3, new List<string> {Author1});
            var userMap = new List<User>()
            {
                user1
            };
            
            fileOperationService.SetUserMap(userMap);

            var file = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "/userids.json");
            var result = JsonConvert.DeserializeObject<List<User>>(file);
            result.Count.Should().Be(1);
            result.First().Id.Should().Be(user1.Id);
            result.First().Names.Count.Should().Be(1);
            result.First().Names.First().Should().Be(Author1);
        }
        
        private const string Json =
            "    {\r\n      \"users\": [ \r\n      {\r\n        \"id\": 221334891705401344,\r\n        \"names\": [\r\n          \"Luus\",\r\n          \"xxx\"\r\n        ]\r\n      },\r\n      {\r\n        \"id\": 229655958622568448,\r\n        \"names\": [\r\n          \"Justin Dekkers\"\r\n        ]\r\n      }\r\n]\r\n}";
    }
}