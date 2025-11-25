# ReheeCmf.Utility Overview

## Tech Stack

- **Framework**: .NET Standard 2.1
- **Language**: C# 9.0
- **Testing**: xUnit with coverlet

## Namespaces

- `ReheeCmf` - Root namespace
- `ReheeCmf.Commons` - Common types and response models
- `ReheeCmf.Helpers` - Helper and extension methods

## Types

### ContentResponse (Abstract Base Class)

Location: `ReheeCmf.Commons`

Abstract base class for API response wrappers.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ObjContent` | `object?` | Abstract read-only property returning the content as object |
| `Success` | `bool?` | Indicates if the request was successful |
| `Status` | `HttpStatusCode` | HTTP status code representing the response status |
| `Errors` | `IEnumerable<Error>?` | Collection of errors if the request failed |

### ContentResponse\<T\>

Location: `ReheeCmf.Commons`

Generic implementation of `ContentResponse` for strongly-typed responses.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `Content` | `T?` | Strongly-typed content of the response |
| `ObjContent` | `object?` | Returns `Content` as object (override) |

**Usage Example:**
```csharp
var response = new ContentResponse<string>();
response.Content = "Hello World";
response.Success = true;
response.Status = HttpStatusCode.OK;
```

### Error

Location: `ReheeCmf.Commons`

Placeholder class for error information.

## Helper Methods

### ContentResponseHelper

Location: `ReheeCmf.Helpers`

Static class providing extension methods for both `ContentResponse<T>` and base `ContentResponse`.

#### SetContent (Generic)

Sets all properties on a `ContentResponse<T>` with typed content.

```csharp
public static void SetContent<T>(
    this ContentResponse<T> response,
    T? content,
    bool? success,
    HttpStatusCode? code,
    params Error[] errors)
```

**Parameters:**
- `response`: The response instance to modify
- `content`: Typed content value
- `success`: Whether the operation succeeded
- `code`: HTTP status code
- `errors`: Optional error collection

**Example:**
```csharp
var response = new ContentResponse<int>();
response.SetContent(42, true, HttpStatusCode.OK);
```

#### SetContent (Non-Generic)

Sets all properties on a base `ContentResponse` using reflection to determine the generic type and convert content.

```csharp
public static void SetContent(
    this ContentResponse response,
    object? content,
    bool? success,
    HttpStatusCode? code,
    params Error[] errors)
```

**Parameters:**
- `response`: The response instance to modify (must be a `ContentResponse<T>` instance)
- `content`: Content value (will be converted to type T via reflection)
- `success`: Whether the operation succeeded
- `code`: HTTP status code
- `errors`: Optional error collection

**Behavior:**
- Automatically detects the generic type T of the response
- Attempts to convert the content to type T
- If conversion fails, sets `Success` to `false` and content to default
- Supports nullable types via `Nullable.GetUnderlyingType`

**Example:**
```csharp
ContentResponse response = new ContentResponse<int>();
response.SetContent(42, true, HttpStatusCode.OK);
// Content is automatically converted to int
```

#### SetSuccess (Generic)

Convenience method for successful responses with typed content.

```csharp
public static void SetSuccess<T>(
    this ContentResponse<T> response,
    T? content)
```

Sets:
- `Success` to `true`
- `Status` to `HttpStatusCode.OK`
- `Content` to the provided value

**Example:**
```csharp
var response = new ContentResponse<string>();
response.SetSuccess("Operation completed");
```

#### SetSuccess (Non-Generic)

Convenience method for successful responses using base ContentResponse.

```csharp
public static void SetSuccess(
    this ContentResponse response,
    object? content)
```

Sets:
- `Success` to `true`
- `Status` to `HttpStatusCode.OK`
- `Content` to the provided value (converted via reflection)

**Example:**
```csharp
ContentResponse response = new ContentResponse<string>();
response.SetSuccess("Operation completed");
```

#### SetErrors (Generic)

Convenience method for error responses with typed content.

```csharp
public static void SetErrors<T>(
    this ContentResponse<T> response,
    HttpStatusCode code,
    params Error[] errors)
```

Sets:
- `Success` to `false`
- `Status` to the provided code
- `Errors` to the provided error collection
- `Content` to default

**Example:**
```csharp
var response = new ContentResponse<string>();
response.SetErrors(HttpStatusCode.BadRequest, new Error(), new Error());
```

#### SetErrors (Non-Generic)

Convenience method for error responses using base ContentResponse.

```csharp
public static void SetErrors(
    this ContentResponse response,
    HttpStatusCode code,
    params Error[] errors)
```

Sets:
- `Success` to `false`
- `Status` to the provided code
- `Errors` to the provided error collection
- `Content` to null

**Example:**
```csharp
ContentResponse response = new ContentResponse<string>();
response.SetErrors(HttpStatusCode.NotFound, new Error());
```

## Usage Patterns

### Basic Success Response (Generic)
```csharp
using ReheeCmf.Commons;
using ReheeCmf.Helpers;
using System.Net;

var response = new ContentResponse<UserDto>();
response.SetSuccess(new UserDto { Id = 1, Name = "John" });
// response.Success == true
// response.Status == HttpStatusCode.OK
// response.Content == UserDto instance
```

### Using Base ContentResponse (Non-Generic)
```csharp
ContentResponse response = new ContentResponse<UserDto>();
response.SetSuccess(new UserDto { Id = 1, Name = "John" });
// Automatically converts content via reflection
// response.Success == true
// response.Status == HttpStatusCode.OK
```

### Error Response
```csharp
var response = new ContentResponse<UserDto>();
response.SetErrors(HttpStatusCode.NotFound, new Error());
// response.Success == false
// response.Status == HttpStatusCode.NotFound
// response.Errors contains the error
```

### Full Control
```csharp
var response = new ContentResponse<int>();
response.SetContent(
    content: 100,
    success: true,
    code: HttpStatusCode.Created,
    errors: Array.Empty<Error>()
);
```

## Building and Testing

```bash
# Build
dotnet build

# Run tests
dotnet test

# Run tests with verbosity
dotnet test --verbosity normal
```

## Project Structure

```
ReheeCmf.Utility/
├── src/
│   └── ReheeCmf.Utility/
│       ├── Commons/
│       │   ├── ContentResponse.cs    # ContentResponse and ContentResponse<T>
│       │   └── Error.cs              # Error placeholder class
│       ├── Helpers/
│       │   └── ContentResponseHelper.cs  # Extension methods
│       ├── Overview.md               # This documentation file
│       └── ReheeCmf.Utility.csproj
└── tests/
    └── ReheeCmf.Utility.Tests/
        ├── ContentResponseHelperTests.cs  # Unit tests
        └── ReheeCmf.Utility.Tests.csproj
```
