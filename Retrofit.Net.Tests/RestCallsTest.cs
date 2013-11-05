using System.Collections.Generic;
using System.Net;
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
        private IRestClient restClient;
        private RestAdapter adapter;
        private IRestInterface client;

        public interface IRestInterface
        {
            [Get("people")]
            RestResponse<List<Person>> GetPeople();

            [Get("people/{id}")]
            RestResponse<Person> GetPerson([Path("id")] int id);

            [Get("people/{id}")]
            RestResponse<Person> GetPerson([Path("id")] int id, [Query("q")] string query);

            [Post("people")]
            RestResponse<Person> AddPerson([Body] Person person);

            [Put("people/{id}")]
            RestResponse<Person> UpdatePerson([Path("id")] int id, [Body] Person person);

            [Head("people/{id}")]
            RestResponse<Person> HeadPerson([Path("id")] int id);

            [Delete("people/{id}")]
            RestResponse<Person> DeletePerson([Path("id")] int id);
        }

        public class Person
        {
            public string Name { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            restClient = Substitute.For<IRestClient>();
            adapter = new RestAdapter(restClient);
            client = adapter.Create<IRestInterface>();
        }

        [Test]
        public void TestGetPeople()
        {
            var persons = new RestResponse<List<Person>> { Data = new List<Person>() { new Person { Name = "name_1" }, new Person { Name = "name_2" } } };
            restClient.Execute<List<Person>>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.GET && request.Resource == "people"
                )).Returns(persons);

            RestResponse<List<Person>> people = client.GetPeople();
            people.Data.Should().Equal(persons.Data);
        }


        [Test]
        public void TestGetPerson()
        {
            var personResponse = new RestResponse<Person> {Data = new Person {Name = "name_1"}};
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.GET && request.Resource == "people/{id}" && request.Parameters[0].Name == "id" && request.Parameters[0].Value.Equals("2")
                )).Returns(personResponse);

            var people = client.GetPerson(2);
            people.Data.Should().Be(personResponse.Data);
        }

        [Test]
        public void TestGetPersonQuery()
        {
            var personResponse = new RestResponse<Person> { Data = new Person { Name = "name_1" } };
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.GET && request.Resource == "people/{id}" && 
                request.Parameters[0].Name == "id" && request.Parameters[0].Value.Equals("2") && request.Parameters[0].Type == ParameterType.UrlSegment &&
                request.Parameters[1].Name == "q" && request.Parameters[1].Value.Equals("blah") && request.Parameters[1].Type == ParameterType.GetOrPost
                )).Returns(personResponse); 

            var people = client.GetPerson(2, "blah");
            people.Data.Should().Be(personResponse.Data);
        }


        [Test]
        public void TestAddPerson()
        {
            var person = new Person {Name = "name_1"};
            var personResponse = new RestResponse<Person> { Data = person };
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.POST && request.Resource == "people" && 
                request.Parameters[0].Type == ParameterType.RequestBody && request.Parameters[0].Value.ToString() == "{\"Name\":\"name_1\"}"
                )).Returns(personResponse);

            var people = client.AddPerson(person);
            people.Data.Should().Be(personResponse.Data);
        }

        [Test]
        public void TestUpdatePerson()
        {
            var person = new Person { Name = "name_1" };
            var personResponse = new RestResponse<Person> { Data = person };
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.PUT && request.Resource == "people/{id}" &&
                request.Parameters[0].Type == ParameterType.UrlSegment && request.Parameters[0].Value.ToString() == "2" && request.Parameters[0].Name == "id" &&
                request.Parameters[1].Type == ParameterType.RequestBody && request.Parameters[1].Value.ToString() == "{\"Name\":\"name_1\"}"
                )).Returns(personResponse);

            var people = client.UpdatePerson(2, person);
            people.Data.Should().Be(personResponse.Data);
        }

        [Test]
        public void TestHeadPerson()
        {
            var personResponse = new RestResponse<Person> { StatusCode = HttpStatusCode.OK };
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.HEAD && request.Resource == "people/{id}" && request.Parameters[0].Name == "id" && request.Parameters[0].Value.Equals("2")
                )).Returns(personResponse);

            var people = client.HeadPerson(2);
            people.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void TestDeletePerson()
        {
            var personResponse = new RestResponse<Person> { StatusCode = HttpStatusCode.OK };
            restClient.Execute<Person>(Arg.Is<IRestRequest>(request =>
                request.Method == Method.DELETE && request.Resource == "people/{id}" && request.Parameters[0].Name == "id" && request.Parameters[0].Value.Equals("2")
                )).Returns(personResponse);

            var people = client.DeletePerson(2);
            people.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
