# NSwag CLI (without nswag.json or NSwag package)

`npm install nswag -g`

`nswag --help`

`nswag openapi2csclient`

With params, no space between `/input:myinput`
```
nswag openapi2csclient \
/input:https://localhost:7067/swagger/v1/swagger.json \
/classname:MyClient \
/namespace:MyNamespace \
/output:MyClient.cs \
/JsonLibrary:SystemTextJson \
/GenerateClientInterfaces:true \
/UseBaseUrl:false
```

# NSwag CLI (with nswag.json)
`dotnet add package NSwag.AspNetCore`

`builder.Services.AddOpenApiDocument();`

Create an “nswag.json” file with
`nswag new`

make sure there is an `output` param specified other than `null`

also make sure that the dotnet sdk matches

`nswag run nswag.json`

this creates a cs client 

# Security

`dotnet user-jwts create`
