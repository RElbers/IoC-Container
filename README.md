# IoC-Container

Inversion of Control container for C#. Internally uses (cached) factory methods generated at runtime using compiled expressions.

## Example
```csharp
var container = new CompiledContainer();

// Register interface
container.RegisterType<IService, Service>();
var service1 = container.Resolve<Service>();

// Register singleton object
container.RegisterInstance(new Service());
var service2 = container.Resolve<Service>();

// Register factory method
container.RegisterFunc(() => {
Console.WriteLine("Creating new service . . .");
return new Service();
});
var service3 = container.Resolve<Service>();
```

### Output

```
 Creating new service . . .
```
