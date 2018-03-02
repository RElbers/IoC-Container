# IoC-Container

Inversion of Control container for C#. Internally uses (cached) factory methods generated at runtime using compiled expressions.

## Example
```csharp
var container = new CompiledContainer();

// Register interface
container.RegisterType<IService, ServiceImpl>();
var service1 = container.Resolve<IService>();

// Register singleton object
Container.RegisterInstance(new ServiceImpl());
var service2 = container.Resolve<Service>();

// Register factory method
 Container.RegisterFunc(() =>   {
        Console.WriteLine("Creating new service . . .");
        return new ServiceImpl();
    });
var service3 = container.Resolve<Service>();
> Creating new service . . .
```
