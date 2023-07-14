# FaunaDB Client

This is an experimental FaunaDB client for .NET. It allows to directly use FQL queries through the database's API.

![dotnet workflow](https://github.com/goncalo-oliveira/faunasharp/actions/workflows/dotnet.yml/badge.svg)

## Usage

To use the client, you need to create a `FaunaClient` instance and pass it an `HttpClient` instance. The `HttpClient` instance should have the `Authorization` header pre-populated in the default headers, with your FaunaDB API key.

If you're using dependency injection, it's recommended to build the `HttpClient` instances using the `IHttpClientFactory` interface.

```csharp
var httpClient = ...;

/*
Add the API key as a bearer token.
If the httpClient came from the IHttpClientFactory interface,
this can be done once when registering the client in the DI container.
*/
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
    "Bearer",
    apiKey
);

var faunaClient = new FaunaClient( httpClient );
```

Once you have a `FaunaClient` instance, you can use it to execute queries written with FQL.

```csharp
FaunaResponse response = await client.QueryAsync( "Collection.all()" );
```

The example above returns a generic `FaunaResponse` instance, which does not deserialize the contents of the reponse's data. If you try to serialize the response object, you'll end up with something like this

```json
{
  "data": "W3sibmFtZSI6IkNvZmZlZUJlYW4iLCJjb2xsIjoiQ29sbGVjdGlvbiIsInRzIjoiMjAyMy0wNy0wN1QxNjowMDowMi40MDBaIiwiY29uc3RyYWludHMiOltdLCJpbmRleGVzIjp7fX1d",
  "summary": "",
  "timestamp": "2023-07-13T14:07:38.809234Z",
  "stats": {
    "computeOps": 1,
    "readOps": 9,
    "writeOps": 0,
    "queryTime": 11,
    "contentionRetries": 0,
    "storageBytesRead": 305,
    "storageBytesWrite": 0
  },
  "schemaVersion": 1688745602400000
}
```

This is by design, as the client does not want to make any assumptions on how to deserialize the data contents. Therefore, the `data` property is left as a byte array with the binary content, which you can deserialize yourself.

If you know the type of the data you're expecting, you can use the `QueryAsync<T>` method, which will deserialize the data for you using the provided type.

```csharp
IEnumerable<CoffeeBean> coffeeBeans = await client.QueryAsync<IEnumerable<CoffeeBean>>(
    "CoffeeBean.all()"
);
```

The only downside of this approach is that you lose the additional information that the `FaunaResponse` object provides, such as the query's stats.
