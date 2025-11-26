# ReheeCmf.Utility Overview

## Tech Stack

- **Framework**: .NET Standard 2.1
- **Language**: C# 9.0
- **Testing**: xUnit with coverlet

## Namespaces

- `ReheeCmf` - Root namespace
- `ReheeCmf.Attributes` - Attribute types
- `ReheeCmf.Commons` - Common types, response models, delegates, and interfaces
- `ReheeCmf.DIContainers` - Dependency injection container types
- `ReheeCmf.Helpers` - Helper and extension methods
- `ReheeCmf.Entities` - Entity interfaces
- `ReheeCmf.Users` - User and tenant related types
- `ReheeCmf.Contexts` - Context and repository interfaces
- `ReheeCmf.Enums` - Enumeration types
- `ReheeCmf.Profiles` - Profile types
- `ReheeCmf.ProfileContainers` - Profile container types

## Types

## Attribute Types

### PooledAttribute

Location: `ReheeCmf.Attributes`

Attribute that can only be applied to classes. Used to mark classes for pooling or special handling.

**Usage:**
- `AttributeTargets.Class` - Can only be applied to classes
- `AllowMultiple = false` - Cannot be applied multiple times to the same class
- `Inherited = true` - Inherited by derived classes

**Usage Example:**
```csharp
[Pooled]
public class MyPooledClass
{
  // Class implementation
}
```

## Dependency Injection Types

### DIPool

Location: `ReheeCmf.DIContainers`

A static "Poor Man's DI" dependency injection container pool for managing ProfileContainer and Profile instances.

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `Initialize(params Action<Type>[] actions)` | `void` | Initializes the DI pool by scanning assemblies for ProfileContainer and Profile types. Can only be called once. Thread-safe. |
| `GetProfile(string key)` | `Profile?` | Retrieves a Profile by string key from all containers |
| `GetProfile(Enum key, string? keyOverride = null)` | `Profile?` | Retrieves a Profile by enum key from all containers |
| `GetProfile<T>()` | `T?` | Retrieves a Profile by generic type from all containers |
| `GetAllProfiles<T>()` | `IEnumerable<T>` | Returns all profiles of a specific type from all containers |
| `GetContainer<TContainer>()` | `TContainer?` | Retrieves a ProfileContainer by its type |
| `Reset()` | `void` | Resets the DIPool (primarily for testing purposes) |

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `IsInitialized` | `bool` | Gets whether the pool has been initialized |

**Initialization Logic:**
1. Scans all assemblies for types that inherit from `ProfileContainer` and are not abstract
2. Creates instances of discovered ProfileContainer types and caches them
3. Scans all assemblies for types that inherit from `Profile` and are not abstract
4. Executes optional actions on each discovered Profile type
5. Creates instances of Profile types and adds them to matching containers
6. Initialization only occurs once, subsequent calls return immediately

**Usage Example:**
```csharp
// Initialize the DI pool
DIPool.Initialize(
  profileType => Console.WriteLine($"Discovered: {profileType.Name}")
);

// Retrieve a profile by string key
var profile = DIPool.GetProfile("MyProfileKey");

// Retrieve a profile by enum key
var profile2 = DIPool.GetProfile(MyEnum.Value);

// Retrieve a strongly-typed profile
var myProfile = DIPool.GetProfile<MyProfile>();

// Get all profiles of a specific type
var allProfiles = DIPool.GetAllProfiles<MyProfile>();

// Get a container
var container = DIPool.GetContainer<MyProfileContainer>();
```

### ContentResponse (Abstract Base Class)

Location: `ReheeCmf.Commons`

Abstract base class for API response wrappers.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ObjContent` | `object?` | Abstract read-only property returning the content as object |
| `ContentType` | `Type` | Abstract read-only property returning the type of the content |
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
| `ContentType` | `Type` | Returns `typeof(T)` (override) |

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

### KeyValueItemDTO

Location: `ReheeCmf.Commons`

Simple key-value pair data transfer object.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `Key` | `string?` | The key of the item |
| `Value` | `string?` | The value of the item |

**Usage Example:**
```csharp
var item = new KeyValueItemDTO
{
  Key = "userId",
  Value = "12345"
};
```

### IWithKeyType

Location: `ReheeCmf.Commons`

Interface for types that have a key type property. Used to identify the type of key used by profiles and other keyed entities.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `Type` | Gets the type of the key used by this entity |

**Usage Example:**
```csharp
public class MyKeyedEntity : IWIthKeyType
{
  public Type KeyType => typeof(int);
}
```

### PoolInitialize

Location: `ReheeCmf.Commons`

Delegate for initializing profiles during DIPool initialization. Used to execute custom actions on each discovered Profile type.

**Signature:**
```csharp
public delegate void PoolInitialize(Type profileType);
```

**Parameters:**
- `profileType` - The type of the profile being initialized

**Usage Example:**
```csharp
PoolInitialize action = (profileType) => 
{
  Console.WriteLine($"Initializing profile: {profileType.Name}");
};

DIPool.Initialize(action);
```

## Entity Types

### IWithName

Location: `ReheeCmf.Entities`

Interface for entities with name and description properties.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string?` | Name of the entity |
| `Description` | `string?` | Description of the entity |

**Usage Example:**
```csharp
public class MyEntity : IWithName
{
  public string? Name { get; set; }
  public string? Description { get; set; }
}
```

### IId\<T\>

Location: `ReheeCmf.Entities`

Generic interface for entities with an identifier of type T.

**Type Parameter:**
- `T` - Type of the identifier, must implement `IEquatable<T>`

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `Id` | `T` | The entity identifier |

**Usage Example:**
```csharp
public class User : IId<Guid>
{
  public Guid Id { get; set; }
  public string Name { get; set; }
}
```

## User and Tenant Types

### TokenDTO

Location: `ReheeCmf.Users`

Data transfer object representing user authentication token information.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `TokenString` | `string?` | The authentication token string |
| `RefreshTokenString` | `string?` | The refresh token string |
| `ExpireUCTTime` | `DateTime?` | UTC expiration time |
| `UserId` | `string?` | User identifier |
| `UserName` | `string?` | User name |
| `UserEmail` | `string?` | User email address |
| `Avatar` | `string?` | User avatar URL or path |
| `Permissions` | `IEnumerable<string>?` | Collection of user permissions |
| `Roles` | `IEnumerable<string>?` | Collection of user roles |
| `IsSystemToken` | `bool?` | Indicates if this is a system token |
| `ExpireSecond` | `ulong` | Token expiration time in seconds |
| `TenantID` | `Guid?` | Associated tenant identifier |
| `Impersonate` | `bool?` | Indicates if this is an impersonation token (ignored when null in JSON) |
| `Claims` | `Dictionary<string, string>?` | Additional custom claims |

**Usage Example:**
```csharp
var token = new TokenDTO
{
  TokenString = "eyJhbGciOiJIUzI1NiIs...",
  UserId = "user123",
  UserName = "john.doe",
  UserEmail = "john@example.com",
  TenantID = Guid.NewGuid(),
  ExpireSecond = 3600,
  Roles = new[] { "Admin", "User" },
  Permissions = new[] { "read:users", "write:users" }
};
```

### Tenant

Location: `ReheeCmf.Users`

Represents a tenant in a multi-tenant application.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `TenantID` | `Guid?` | Unique tenant identifier |
| `TenantName` | `string?` | Name of the tenant |
| `TenantSubDomain` | `string?` | Tenant subdomain |
| `TenantUrl` | `string?` | Tenant URL |
| `TenantLevel` | `int?` | Tenant level or tier |
| `MainConnectionString` | `string?` | Primary database connection string |
| `ReadConnectionStrings` | `string[]?` | Read-only database connection strings |
| `IgnoreTenant` | `bool?` | Whether to ignore tenant filtering |

**Usage Example:**
```csharp
var tenant = new Tenant
{
  TenantID = Guid.NewGuid(),
  TenantName = "Acme Corporation",
  TenantUrl = "https://acme.example.com",
  TenantLevel = 1,
  MainConnectionString = "Server=localhost;Database=acme_db;",
  IgnoreTenant = false
};
```

## Context Interfaces

### ISaveChange

Location: `ReheeCmf.Contexts`

Interface for saving changes with optional user context.

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `SaveChanges(TokenDTO? user = null)` | `int` | Saves changes synchronously, returns number of affected entities |
| `SaveChangesAsync(TokenDTO? user = null, CancellationToken ct = default)` | `Task<int>` | Saves changes asynchronously, returns number of affected entities |

### IRepository

Location: `ReheeCmf.Contexts`

Base repository interface extending ISaveChange with CRUD operations.

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `Query<T>(bool asNoTracking)` | `IQueryable<T>` | Get queryable for entity type T |
| `GetByIdAsync<T>(object id, CancellationToken cancellationToken = default)` | `Task<T?>` | Get entity by ID asynchronously |
| `AddAsync<T>(T entity, CancellationToken cancellationToken = default)` | `Task` | Add entity asynchronously |
| `Delete<T>(T entity)` | `void` | Delete typed entity |
| `DeleteAsync<T>(T entity)` | `Task` | Delete typed entity asynchronously |
| `Delete(object entity)` | `void` | Delete untyped entity |
| `ExecuteTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)` | `Task` | Execute action within a transaction |

### IWithTenant

Location: `ReheeCmf.Contexts`

Interface for entities or contexts that have tenant association.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `TenantID` | `Guid?` | The associated tenant identifier |

### ICrossTenant

Location: `ReheeCmf.Contexts`

Interface for cross-tenant operations.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `CrossTenantID` | `Guid?` | Cross-tenant identifier (read-only) |
| `CrossTenant` | `Tenant?` | Cross-tenant object (read-only) |

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `SetCrossTenant(Tenant? tenant)` | `void` | Set the cross-tenant context |

### ITenantContext

Location: `ReheeCmf.Contexts`

Interface for managing tenant context, extends IWithTenant and ICrossTenant.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ThisTenant` | `Tenant?` | Current tenant (read-only) |
| `ReadOnly` | `bool?` | Indicates if context is read-only (read-only) |
| `IgnoreTenant` | `bool` | Whether tenant filtering is ignored (read-only) |

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `SetTenant(Tenant tenant)` | `void` | Set the current tenant |
| `SetReadOnly(bool readOnly)` | `void` | Set read-only mode |
| `SetIgnoreTenant(bool ignore)` | `void` | Set whether to ignore tenant filtering |
| `UseDefaultConnection()` | `void` | Switch to default database connection |
| `UseTenantConnection()` | `void` | Switch to tenant-specific database connection |

### ITokenDTOContext

Location: `ReheeCmf.Contexts`

Interface for managing user token context.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `User` | `TokenDTO?` | Current user token (read-only) |

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `SetUser(TokenDTO? user)` | `void` | Set the current user |

### IContext

Location: `ReheeCmf.Contexts`

Main context interface combining all context capabilities: save changes, repository operations, tenant management, and disposal.

Extends: `ISaveChange`, `IRepository`, `IWithTenant`, `ITenantContext`, `ICrossTenant`, `IDisposable`, `ITokenDTOContext`

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `ServiceProvider` | `IServiceProvider?` | Service provider for dependency injection |
| `Context` | `object?` | Underlying context object |

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `Query(Type type, bool noTracking, bool readCheck = false)` | `object?` | Get queryable for specified type |
| `QueryWithKey(Type type, Type keyType, bool noTracking, object key, bool readCheck = false)` | `object?` | Get queryable with key type specification |
| `Find(Type type, object key)` | `object?` | Find entity by type and key |
| `Add(Type type, object? value)` | `void` | Add entity of specified type |
| `Delete(Type type, object key)` | `void` | Delete entity by type and key |
| `TrackEntity(object entity, EnumEntityState enumEntityStatus = EnumEntityState.Modified)` | `void` | Track entity with specified state |
| `GetKeyValueItemDTO(Type type)` | `IEnumerable<KeyValueItemDTO>` | Get key-value items for specified type |

## Enumerations

### EnumEntityState

Location: `ReheeCmf.Enums`

Enumeration representing entity states for change tracking.

**Values:**
| Value | Description |
|-------|-------------|
| `NotSpecified` (0) | State not specified |
| `Added` (1) | Entity is newly added |
| `Modified` (2) | Entity has been modified |
| `Deleted` (3) | Entity is marked for deletion |
| `Unchanged` (4) | Entity is unchanged |

**Usage Example:**
```csharp
context.TrackEntity(myEntity, EnumEntityState.Modified);
```

## Profile Types

### Profile (Abstract Base Class)

Location: `ReheeCmf.Profiles`

Abstract base class for profile types. Implements `IWithName` and is typically used as a singleton pattern. Contains key management with support for both integer and string keys.

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string?` | Name of the profile (from IWithName) |
| `Description` | `string?` | Description of the profile (from IWithName) |
| `KeyType` | `Type` | Abstract property returning the type of the key |
| `KeyValue` | `int` | Abstract property for integer key value |
| `StringKeyValue` | `string?` | Abstract property for string key value |
| `StringKeyValueOverride` | `string?` | Optional override for string key value (settable) |
| `EffectiveKey` | `string?` | Returns StringKeyValue when KeyValue != 0, otherwise returns StringKeyValueOverride |

**Usage Example:**
```csharp
public class MyProfile : Profile
{
  public override Type KeyType => typeof(int);
  private int _keyValue;
  public override int KeyValue => _keyValue;
  public override string? StringKeyValue => _keyValue.ToString();
  
  public void SetKey(int value)
  {
    _keyValue = value;
  }
}

var profile = new MyProfile
{
  Name = "Configuration Profile",
  Description = "Main configuration",
  StringKeyValueOverride = "main-config"
};
profile.SetKey(0);

string key = profile.EffectiveKey; // Returns "main-config"
```

### Profile\<T\> (Abstract Generic Class)

Location: `ReheeCmf.Profiles`

Generic abstract profile class that inherits from `Profile`. The type parameter `T` must be an enum type. The `Key` property must be implemented by derived classes.

**Type Parameter:**
- `T` - Type of the key, must be an Enum

**Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `KeyType` | `Type` | Returns `typeof(T)` (override) |
| `Key` | `T` | Abstract property - strongly-typed enum key that must be implemented by derived class |
| `StringKeyValue` | `string?` | Returns the string representation of the Key (override, computed from Key) |
| `KeyValue` | `int` | Returns the integer value of the Key (override, computed from Key) |

**Usage Example:**
```csharp
public enum ProfileCategory
{
  System = 1,
  User = 2,
  Admin = 3
}

public class CategoryProfile : Profile<ProfileCategory>
{
  private readonly ProfileCategory _category;
  
  public CategoryProfile(ProfileCategory category)
  {
    _category = category;
  }
  
  public override ProfileCategory Key => _category;
}

var profile = new CategoryProfile(ProfileCategory.System)
{
  Name = "System Profile",
  Description = "System level configuration"
};

// Access strongly-typed key
ProfileCategory category = profile.Key; // Returns ProfileCategory.System
int keyValue = profile.KeyValue; // Returns 1
string? keyString = profile.StringKeyValue; // Returns "System"
```

### ProfileContainer (Abstract Base Class)

Location: `ReheeCmf.ProfileContainers`

Abstract base class for managing a collection of Profile instances. Provides dictionary-based storage and retrieval.

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `GetProfile(string key)` | `Profile?` | Retrieves a profile by string key, returns null if not found or key is null/empty |
| `GetProfile(Enum key, string? keyOverride = null)` | `Profile?` | Retrieves a profile by enum key. If enum value is 0, uses keyOverride; otherwise uses enum name. Returns null if not found |
| `AddProfile(Profile profile)` | `void` | Adds or updates a profile using its EffectiveKey property. Returns early if profile is null, key is empty, or Profiles dictionary is null |
| `RemoveProfile(string key, out Profile? value)` | `bool` | Removes a profile by key and outputs the removed profile. Returns true if successful, false if key is null/empty, Profiles dictionary is null, or key not found. Outputs null if removal failed |
| `GetAllProfiles()` | `IEnumerable<Profile>` | Returns all profiles in the container |

**Usage Example:**
```csharp
public class MyProfileContainer : ProfileContainer
{
}

var container = new MyProfileContainer();
var profile = new MyProfile();
profile.SetKey(1);
profile.Name = "Config1";

container.AddProfile(profile); // Uses profile.EffectiveKey as the key

// String-based retrieval
var retrieved = container.GetProfile("1");

// Remove with out parameter
bool removed = container.RemoveProfile("1", out var removedProfile);
if (removed)
{
  Console.WriteLine($"Removed: {removedProfile?.Name}");
}

var allProfiles = container.GetAllProfiles();
```

### ProfileContainer\<TKey, TProfile\> (Abstract Generic Class)

Location: `ReheeCmf.ProfileContainers`

Generic abstract profile container that inherits from `ProfileContainer`. Provides strongly-typed access to profiles.

**Type Parameters:**
- `TKey` - Type of the enum key, must be an Enum
- `TProfile` - Type of profiles in the container, must inherit from Profile<TKey>

**Methods:**
| Method | Return Type | Description |
|--------|-------------|-------------|
| `GetProfile(string key)` | `TProfile?` | Retrieves a strongly-typed profile by string key, returns null if not found |
| `GetProfile(TKey enumKey, string keyOverride)` | `TProfile?` | Retrieves a strongly-typed profile by enum key with optional override |
| `AddProfile(TProfile profile)` | `void` | Adds or updates a strongly-typed profile using its effective key |
| `GetAllProfiles()` | `IEnumerable<TProfile>` | Returns all profiles in the container as strongly-typed collection |

**Usage Example:**
```csharp
public enum ProfileCategory
{
  System = 1,
  User = 2
}

public class CategoryProfile : Profile<ProfileCategory>
{
  private readonly ProfileCategory _category;
  
  public CategoryProfile(ProfileCategory category)
  {
    _category = category;
  }
  
  public override ProfileCategory Key => _category;
}

public class CategoryProfileContainer : ProfileContainer<ProfileCategory, CategoryProfile>
{
}

var container = new CategoryProfileContainer();
var profile = new CategoryProfile(ProfileCategory.System)
{
  Name = "System Profile"
};

container.AddProfile(profile); // Uses profile.EffectiveKey as the key

// String-based retrieval
CategoryProfile? retrieved = container.GetProfile("System");

// Enum-based retrieval with optional override
CategoryProfile? retrieved2 = container.GetProfile(ProfileCategory.System, null);

IEnumerable<CategoryProfile> allProfiles = container.GetAllProfiles();
```

## Helper Methods

### TypeHelper

Location: `ReheeCmf.Helpers`

Static class providing extension methods for type inspection and reflection operations.

#### HasAttribute<TAttribute> (Generic)

Checks if a type is decorated with the specified attribute.

```csharp
public static bool HasAttribute<TAttribute>(this Type type) where TAttribute : Attribute
```

**Parameters:**
- `type`: The type to check
- `TAttribute`: The attribute type to check for (must be an Attribute)

**Returns:**
- `true` if the type has the attribute, `false` otherwise (or if type is null)

**Example:**
```csharp
var type = typeof(MyClass);
bool hasPooled = type.HasAttribute<PooledAttribute>();
```

#### HasAttribute (Type Parameter)

Checks if a type is decorated with the specified attribute using a type parameter.

```csharp
public static bool HasAttribute(this Type type, Type attributeType)
```

**Parameters:**
- `type`: The type to check
- `attributeType`: The attribute type to check for (must be an Attribute)

**Returns:**
- `true` if the type has the attribute, `false` otherwise (or if either parameter is null or attributeType is not an Attribute)

**Example:**
```csharp
var type = typeof(MyClass);
var attrType = typeof(PooledAttribute);
bool hasAttribute = type.HasAttribute(attrType);
```

#### ImplementsInterface

Checks if a type implements the specified interface. Checks base classes layer by layer until no base class is found.

```csharp
public static bool ImplementsInterface(this Type type, Type interfaceType)
```

**Parameters:**
- `type`: The type to check
- `interfaceType`: The interface type to check for (must be an interface)

**Returns:**
- `true` if the type implements the interface, `false` otherwise

**Behavior:**
- Checks the type and all base classes in the inheritance hierarchy
- Supports generic interface matching

**Example:**
```csharp
var type = typeof(MyClass);
bool implementsIDisposable = type.ImplementsInterface(typeof(IDisposable));
```

#### InheritsFrom

Checks if a type inherits from the specified class or implements the specified interface. Checks base classes layer by layer until no base class is found.

```csharp
public static bool InheritsFrom(this Type type, Type baseType)
```

**Parameters:**
- `type`: The type to check
- `baseType`: The base class or interface type to check for

**Returns:**
- `true` if the type inherits from the class or implements the interface, `false` otherwise

**Behavior:**
- If baseType is an interface, delegates to `ImplementsInterface`
- Checks the type and all base classes in the inheritance hierarchy
- Supports generic type matching

**Example:**
```csharp
var type = typeof(DerivedClass);
bool inherits = type.InheritsFrom(typeof(BaseClass));
```

#### ImplementsInterface<T> (Generic)

Checks if a generic type implements the specified interface.

```csharp
public static bool ImplementsInterface<T>(Type interfaceType)
```

**Type Parameter:**
- `T`: The type to check

**Parameters:**
- `interfaceType`: The interface type to check for

**Returns:**
- `true` if the type implements the interface, `false` otherwise

**Example:**
```csharp
bool implementsInterface = TypeHelper.ImplementsInterface<MyClass>(typeof(IDisposable));
```

#### InheritsFrom<T> (Generic)

Checks if a generic type inherits from the specified class or implements the specified interface.

```csharp
public static bool InheritsFrom<T>(Type baseType)
```

**Type Parameter:**
- `T`: The type to check

**Parameters:**
- `baseType`: The base class or interface type to check for

**Returns:**
- `true` if the type inherits from the class or implements the interface, `false` otherwise

**Example:**
```csharp
bool inherits = TypeHelper.InheritsFrom<DerivedClass>(typeof(BaseClass));
```

### ContentResponseHelper

Location: `ReheeCmf.Helpers`

Static class providing extension methods for both `ContentResponse<T>` and base `ContentResponse`.

#### SetContent (Generic)

Sets all properties on a `ContentResponse<T>` with object content that is converted to type T.

```csharp
public static void SetContent<T>(
    this ContentResponse<T> response,
    object? content,
    bool? success,
    HttpStatusCode? code,
    params Error[] errors)
```

**Parameters:**
- `response`: The response instance to modify
- `content`: Content value as object (uses pattern matching `content is T` for type checking, falls back to `Convert.ChangeType`)
- `success`: Whether the operation succeeded
- `code`: HTTP status code
- `errors`: Optional error collection

**Behavior:**
- Uses pattern matching to check if content is already type T
- Falls back to `Convert.ChangeType` for conversion
- Sets `Success` to `false` if conversion fails
- Supports nullable types via `Nullable.GetUnderlyingType`

**Example:**
```csharp
var response = new ContentResponse<int>();
response.SetContent(42, true, HttpStatusCode.OK);
```

#### SetContent (Non-Generic)

Sets all properties on a base `ContentResponse` using the `ContentType` property to invoke the generic method.

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
- `content`: Content value as object
- `success`: Whether the operation succeeded
- `code`: HTTP status code
- `errors`: Optional error collection

**Behavior:**
- Uses `ContentType` property to determine the generic type T
- Caches the reflection method lookup for performance using `ConcurrentDictionary`
- Invokes the generic `SetContent<T>` method with the content
- If invocation fails, sets `Success` to `false`

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

### DictionaryHelper

Location: `ReheeCmf.Helpers`

Static class providing extension methods for `IDictionary<TKey, TValue>` that add safe dictionary operations with null checks.

#### TryAddOrUpdate

Adds a new key-value pair or updates an existing one.

```csharp
public static bool TryAddOrUpdate<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary,
    TKey key,
    TValue value)
```

**Parameters:**
- `dictionary`: The dictionary to modify
- `key`: The key to add or update
- `value`: The value to set

**Returns:**
- `true` if the operation succeeded
- `false` if dictionary or key is null

**Behavior:**
- Returns false if dictionary is null
- Returns false if key is null
- If key exists, updates the value
- If key doesn't exist, adds the key-value pair
- Returns true on successful add or update

**Example:**
```csharp
var dict = new Dictionary<string, int>();
bool result = dict.TryAddOrUpdate("key1", 100); // Adds, returns true
result = dict.TryAddOrUpdate("key1", 200);      // Updates, returns true
// dict["key1"] == 200
```

#### TryAdd

Attempts to add a key-value pair only if the key doesn't already exist.

```csharp
public static bool TryAdd<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary,
    TKey key,
    TValue value)
```

**Parameters:**
- `dictionary`: The dictionary to modify
- `key`: The key to add
- `value`: The value to add

**Returns:**
- `true` if the key-value pair was added
- `false` if dictionary is null, key is null, or key already exists

**Behavior:**
- Returns false if dictionary is null
- Returns false if key is null
- Returns false if key already exists (does not modify existing value)
- Adds the key-value pair if key doesn't exist and returns true

**Example:**
```csharp
var dict = new Dictionary<string, int>();
bool result = dict.TryAdd("key1", 100); // Adds, returns true
result = dict.TryAdd("key1", 200);      // Does not add, returns false
// dict["key1"] == 100
```

#### TryRemove

Attempts to remove a key-value pair and returns the removed value.

```csharp
public static bool TryRemove<TKey, TValue>(
    this IDictionary<TKey, TValue> dictionary,
    TKey key,
    out TValue? value)
```

**Parameters:**
- `dictionary`: The dictionary to modify
- `key`: The key to remove
- `value`: Output parameter receiving the removed value (or default if removal failed)

**Returns:**
- `true` if the key was found and removed
- `false` if dictionary is null, key is null, or key doesn't exist

**Behavior:**
- Returns false and sets value to default if dictionary is null
- Returns false and sets value to default if key is null
- Returns false and sets value to default if key doesn't exist
- Removes the key-value pair if key exists, sets value to the removed value, and returns true

**Example:**
```csharp
var dict = new Dictionary<string, int> { { "key1", 100 } };
bool result = dict.TryRemove("key1", out int value); // Removes, returns true
// result == true, value == 100, dict is empty
result = dict.TryRemove("key1", out value);          // Does not remove, returns false
// result == false, value == 0
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
│       ├── Attributes/
│       │   └── PooledAttribute.cs       # Pooled attribute for classes
│       ├── Commons/
│       │   ├── ContentResponse.cs       # ContentResponse and ContentResponse<T>
│       │   ├── Error.cs                 # Error placeholder class
│       │   └── KeyValueItemDTO.cs       # Key-value pair DTO
│       ├── Contexts/
│       │   ├── IContext.cs              # Main context interface
│       │   ├── ICrossTenant.cs          # Cross-tenant interface
│       │   ├── IRepository.cs           # Repository interface
│       │   ├── ISaveChange.cs           # Save changes interface
│       │   ├── ITenantContext.cs        # Tenant context interface
│       │   ├── ITokenDTOContext.cs      # Token DTO context interface
│       │   └── IWithTenant.cs           # With tenant interface
│       ├── DIContainers/
│       │   └── DIPool.cs                # Poor Man's DI container pool
│       ├── Entities/
│       │   ├── IId.cs                   # Generic ID interface
│       │   └── IWithName.cs             # Name and description interface
│       ├── Enums/
│       │   └── EnumEntityState.cs       # Entity state enumeration
│       ├── Helpers/
│       │   ├── ContentResponseHelper.cs # Extension methods for ContentResponse
│       │   ├── DictionaryHelper.cs      # Extension methods for IDictionary
│       │   └── TypeHelper.cs            # Type inspection and reflection helpers
│       ├── Profiles/
│       │   ├── Profile.cs               # Abstract base profile class
│       │   └── ProfileGeneric.cs        # Generic profile<T> class
│       ├── ProfileContainers/
│       │   ├── ProfileContainer.cs      # Abstract profile container
│       │   └── ProfileContainerGeneric.cs # Generic profile container<T>
│       ├── Users/
│       │   ├── Tenant.cs                # Tenant class
│       │   └── TokenDTO.cs              # Token DTO class
│       ├── Overview.md                  # This documentation file
│       └── ReheeCmf.Utility.csproj
└── tests/
    └── ReheeCmf.Utility.Tests/
        ├── ContentResponseHelperTests.cs  # ContentResponse helper tests
        ├── DIPoolTests.cs                 # DIPool tests
        ├── DictionaryHelperTests.cs       # Dictionary helper tests
        ├── EntityTypesTests.cs            # Entity types tests
        ├── PooledAttributeTests.cs        # PooledAttribute tests
        ├── ProfileTests.cs                # Profile and ProfileContainer tests
        ├── TypeHelperTests.cs             # TypeHelper tests
        ├── UnitTest1.cs                   # Sample test
        └── ReheeCmf.Utility.Tests.csproj
```
