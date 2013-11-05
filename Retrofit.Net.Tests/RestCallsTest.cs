using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;


namespace Retrofit.Net.Tests
{
    [TestFixture]
    public class TestRestCalls
    {
        public interface IRestInterface
        {
            [Get("people")]
            RestResponse<List<Person>> GetPeople();

            [Get("people/{id}")]
            RestResponse<Person> GetPerson([Path("id")] int id);
        }

        public class Person
        {
            public string Name { get; set; }
        }

        [Test]
        public void TestGetPeople()
        {
            var restClient = Substitute.For<IRestClient>();
            var persons = new RestResponse<List<Person>> { Data = new List<Person>() { new Person { Name = "name_1" }, new Person { Name = "name_2" } } };
            restClient.Execute<List<Person>>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.GET && request.Resource == "people"
                )).Returns(persons);

            RestAdapter adapter = new RestAdapter(restClient);
            IRestInterface client = adapter.Create<IRestInterface>();
            RestResponse<List<Person>> people = client.GetPeople();
            people.Data.Should().Equal(persons.Data);
        }


        [Test]
        public void TestGetPerson()
        {
            var restClient = Substitute.For<IRestClient>();
            var personResponse = new RestResponse<Person>();
            personResponse.Data = new Person { Name = "name_1" };
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.GET && request.Resource == "people/{id}" && request.Parameters[0].Name == "id" && request.Parameters[0].Value.Equals("2")
                )).Returns(personResponse);

            RestAdapter adapter = new RestAdapter(restClient);
            IRestInterface client = adapter.Create<IRestInterface>();
            var people = client.GetPerson(2);
            people.Data.Should().Be(personResponse.Data);
        }
    }
}
