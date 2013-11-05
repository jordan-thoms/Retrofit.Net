using System.Collections.Generic;
using RestSharp;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace Retrofit.Net.Tests
{
    public interface IPeopleService
    {
        [Get("people")]
        RestResponse<List<TestRestCallsIntegration.Person>> GetPeople();

        [Get("people/{id}")]
        RestResponse<TestRestCallsIntegration.Person> GetPerson([Path("id")] int id);

        [Get("people/{id}")]
        RestResponse<TestRestCallsIntegration.Person> GetPerson([Path("id")] int id, [Query("limit")] int limit, [Query("test")] string test);

        [Post("people")]
        RestResponse<TestRestCallsIntegration.Person> AddPerson([Body] TestRestCallsIntegration.Person person);
    }
}