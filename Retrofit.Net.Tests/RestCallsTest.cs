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

            [Post("people")]
            RestResponse<Person> AddPerson([Body] Person person);
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
            var personResponse = new RestResponse<Person> {Data = new Person {Name = "name_1"}};
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.GET && request.Resource == "people/{id}" && request.Parameters[0].Name == "id" && request.Parameters[0].Value.Equals("2")
                )).Returns(personResponse);

            RestAdapter adapter = new RestAdapter(restClient);
            IRestInterface client = adapter.Create<IRestInterface>();
            var people = client.GetPerson(2);
            people.Data.Should().Be(personResponse.Data);
        }

        [Test]
        public void TestAddPerson()
        {
            var restClient = Substitute.For<IRestClient>();
            var person = new Person {Name = "name_1"};
            var personResponse = new RestResponse<Person> { Data = person };
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.POST && request.Resource == "people" && 
                request.Parameters[0].Type == ParameterType.RequestBody && request.Parameters[0].Value.ToString() == "{\"Name\":\"name_1\"}"
                )).Returns(personResponse);

            RestAdapter adapter = new RestAdapter(restClient);
            IRestInterface client = adapter.Create<IRestInterface>();
            var people = client.AddPerson(person);
            people.Data.Should().Be(personResponse.Data);
        }
    }
}
