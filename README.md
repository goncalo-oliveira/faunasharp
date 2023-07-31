# FaunaDB Client

This is an experimental FaunaDB client for .NET. It allows to directly use FQL queries through the database's API.

![dotnet workflow](https://github.com/goncalo-oliveira/faunasharp/actions/workflows/dotnet.yml/badge.svg)

## Usage

The client is available as a NuGet package. Since it's still in an experimental stage, it's only available as a pre-release package. You can install it using the following command

```bash
dotnet add package FaunaDB.MinimalClient --prerelease
```

To use the client, you need to create a `FaunaClient` instance and pass it an `HttpClient`. The `HttpClient` instance should have the `Authorization` header pre-populated in the default headers, with your FaunaDB API key.

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

If you know the type of the data you're expecting, you can use the generic method instead, which will deserialize the data for you using the provided type.

```csharp
IEnumerable<CoffeeBean> coffeeBeans = await client.QueryAsync<IEnumerable<CoffeeBean>>(
    "CoffeeBean.all()"
);
```

The only downside of this approach is that you lose the additional information that the `FaunaResponse` object provides, such as the query stats.

## Pagination

Results that are paginated are returned inside a page structure, containing the data and a reference to the next page. This needs to be considered when deserializing the data.

```yaml
CoffeeBean.all().paginate(10)
---
{
  data: [...],
  after: "9JHMp2IWi0Ph/rHhjWO8RA=="
}
```

If we know the results are paginated, we can use the `ToPage()` extension methods to deserialize the page.

```csharp
FaunaResponse response = await client.QueryAsync( "CoffeeBean.all().paginate(10)" );

Page page = response.ToPage();
```

As with `FaunaResponse`, the `Page` does not deserialize the data contents, since the client does not want to make any assumptions on how to do this. The `data` property is left as a byte array with the binary content, which you can deserialize yourself.

If you know the type of the data you're expecting, you can use the generic method instead, which will deserialize the data for you using the provided type.

```csharp
Page<CoffeeBean> page = response.ToPage<CoffeeBean>();

// page.Data is an IEnumerable<CoffeeBean>
```

If you do not need the other information that the `FaunaResponse` provides (such as the query stats) and you are only interested in the data, you can use these types with the generic method.

```csharp
// a page containing the data as a byte array
Page page = await client.QueryAsync<Page>( "CoffeeBean.all().paginate(10)" );

// a page containing the data as an IEnumerable<CoffeeBean>
Page<CoffeeBean> typedPage = await client.QueryAsync<Page<CoffeeBean>>(
    "CoffeeBean.all().paginate(10)"
);
```

## Using objects as parameters

When creating a query, you can use objects as parameters. The client will automatically serialize the object into a JSON string and pass it to the query.

For example, let's consider the following query to create a new document in a collection.

```json
CoffeeBean.create({
    "Species": "Arabica",
    "Owner": "metad plc",
    "Country_of_Origin": "Ethiopia",
    "Harvest_Year": 2014,
    "Quality_Parameters": {
        "Aroma": 8.67,
        "Flavor": 8.83,
        "Balance": 8.42
        },
    "Altitude": {
      "unit_of_measurement": "m",
      "mean": 2075
    }
})
```

So that we don't have to manually write all the object's JSON, we can use the `CoffeeBean` class to create the object and pass it to the query.

```csharp
var response = await client.QueryAsync(
    bean => $"CoffeeBean.create( {bean} )",
    new CoffeeBean
    {
        Species = "Arabica",
        Owner = "Jeo metad plc",
        CountryOfOrigin = "Ethiopia",
        HarvestYear = 2014,
        QualityParameters = new CoffeeBeanQuality
        {
            Aroma = 7.67,
            Flavor = 8.83,
            Balance = 8.42
        },
        Altitude = new CoffeeBeanAltitude
        {
            UnitOfMeasurement = "m",
            Mean = 2075
        }
    }
);
```

Since the query above returns the created document, we could also use the generic method to directly deserialize the response.

```csharp
var coffeeBean = await client.QueryAsync<CoffeeBean>( ... );
```

> CAUTION: if the `CofeeBean` contains any properties that are related to the document reference, such as the `id` or `coll` properties, these should be decorated with the `JsonIgnore` attribute when writing null, otherwise the query will fail.

```csharp
public class CoffeeBean
{
    [JsonPropertyName( "id" )]
    [JsonIgnore( Condition = JsonIgnoreCondition.WhenWritingNull )]
    public string? Id { get; set; }
    public string? Species { get; set; }
    public string? Owner { get; set; }
    ...
}
```

We can also use anonymous objects as parameters.

```csharp
var response = await client.QueryAsync(
  obj => $$"""
  CoffeeBean.byId("366190711733747780")!.update(
    {{obj}}
  )
  """,
  new { Owner = "Healthy Grounds, Inc." }
)
```
