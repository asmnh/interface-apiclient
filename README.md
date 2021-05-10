# Interface API client
.NET 5 for on-the-fly generation of HTTP API client from interface definition with metadata.

See sample projects for usage samples.

This is early alpha proof-of-concept attempt at making library that will ease transition from using standard dependency injection to extracting dependencies across network over HTTP.

## TODO

- Add HTTP error to exception conversion, following attributes.
- Harden and test what is already there.
- Proper error handling for unspecified situation (network errors, empty response etc).
- Allow named HTTP Client configuration.
- Test and handle empty responses.
- Handle non-JSON responses (tbd: how).
- Make NuGet package, build process and publish.
- Conditional response content to exception conversion (for services that always return HTTP 200).
- Better attributes validation.
- To be considered: server-side request handler generator (should use OpenAPI attributes for that probably).
- Profile performance of building requests, performance comparision against handbuilt requests.
