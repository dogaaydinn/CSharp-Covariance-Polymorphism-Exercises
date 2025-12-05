# GraphQL Example

> GraphQL API with HotChocolate framework.

## Features
- **HotChocolate** - .NET GraphQL server
- **Type-safe queries** - Strongly typed schema
- **Introspection** - Auto-generated documentation

## Run
```bash
dotnet run
# Navigate to: http://localhost:5000/graphql
```

## Example Query
```graphql
query {
  books {
    id
    title
    author
  }
}
```

## Benefits
- Clients request only needed fields
- Single endpoint (vs REST's multiple endpoints)
- Strong typing
