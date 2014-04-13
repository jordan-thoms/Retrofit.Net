Retrofit.Net
============
Retrofit.Net turns your REST API into a C# interface.
```c#
public interface IPeopleService
{
    [Get("people")]
    RestResponse<List<Person>> GetPeople();

    [Get("people/{id}")]
    RestResponse<Person> GetPerson([Path("id")] int id);

    [Get("people/{id}")]
    RestResponse<Person> GetPerson([Path("id")] int id, [Query("limit")] int limit, [Query("test")] string test);

    [Post("people")]
    RestResponse<Person> AddPerson([Body] TestRestCallsIntegration.Person person);
}
```
The RestAdapter class generates an implementation of the IPeopleService interface.
```c#
RestAdapter adapter = new RestAdapter("http://jordanthoms.apiary.io/");
IPeopleService service = adapter.Create<IPeopleService>();
RestResponse<Person> personResponse = service.GetPerson(3, 100, "tsst");
Person person = personResponse.Data;
```
Each call on the generated PeopleService makes an HTTP request to the remote webserver. 
It returns a RestResponse<Person>, which is the same format as the responses from RestSharp.

Annotations are used to describe how to make each request.

Take a look at https://github.com/paulcbetts/refit also , it's more maintained. 

Acknowledgements
============
This library is heavily inspired by Retrofit ( https://github.com/square/retrofit )... I even stole the name!

RestSharp is used to do all the heavy lifting - this is essentially a Retrofit-style wrapper for Rest#.
