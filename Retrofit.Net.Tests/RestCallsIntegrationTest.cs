using System.Collections.Generic;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;


namespace Retrofit.Net.Tests
{
    [TestFixture]
    public class TestRestCallsIntegration
    {
        private RestAdapter adapter;
        private IPeopleService service;

        public class Person
        {
            protected bool Equals(Person other)
            {
                return Id == other.Id && string.Equals(Name, other.Name);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Id*397) ^ (Name != null ? Name.GetHashCode() : 0);
                }
            }

            public int Id { get; set; }
            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Person) obj);
            }
        }

        [SetUp]
        public void SetUp()
        {
            adapter = new RestAdapter("http://jordanthoms.apiary.io/");
            service = adapter.Create<IPeopleService>();
        }

        [Test]
        public void TestGetPeople()
        {
            var persons = new List<Person>() {new Person {Id= 1, Name = "Person Name"}, new Person {Id = 2, Name = "Person Name 2"}};
            var people = service.GetPeople();
            people.Data.Should().Equal(persons);
        }


        [Test]
        public void TestGetPerson()
        {
            var person = new Person { Id = 3, Name = "Person Name" };
            var personResponse = service.GetPerson(3);
            personResponse.Data.Should().Be(person);
        }

        [Test]
        public void TestGetPersonQuery()
        {
            var person = new Person { Id = 3, Name = "Person Name" };
            var personResponse = service.GetPerson(3, 100, "tsst");
            personResponse.Data.Should().Be(person);
        }


        [Test]
        public void TestAddPerson()
        {
            var person = new Person { Id = 5,  Name = "Person Name" };
            var personResponse = service.AddPerson(person);
            personResponse.Data.Should().Be(person);
        }
    }
}
