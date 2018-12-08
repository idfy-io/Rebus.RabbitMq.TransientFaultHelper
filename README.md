# Rebus.RabbitMq.TransientFaultHelper
A transient fault helper for Rebus using RabbitMQ. Will retry sending messages to RabbitMQ. Use default policy or provide your own.



[![Build Status](https://travis-ci.org/idfy-io/Rebus.RabbitMq.TransientFaultHelper.svg?branch=master)](https://travis-ci.org/idfy-io/Rebus.RabbitMq.TransientFaultHelper) [![NuGet](https://img.shields.io/nuget/v/Rebus.RabbitMq.TransientFaultHelper.svg)](https://www.nuget.org/packages/Rebus.RabbitMq.TransientFaultHelper)



Supports .NET Standard 2.0+, .NET Core 2.0+ and .NET Framework 4.6.1+.

## Installation
Using NuGet is the easiest way to install the Rebus extension.

Package Manager:

	PM > Install-Package Rebus.RabbitMq.TransientFaultHelper

Command line:  

	nuget install Rebus.RabbitMq.TransientFaultHelper

.NET Core CLI:  

	dotnet add package Rebus.RabbitMq.TransientFaultHelper

## Example

  ```csharp
Configure.With(someContainerAdapter)
        .Logging(l => l.Serilog())
        .Transport(t => t.UseMsmq("myInputQueue"))
      	.Routing(r => r.TypeBased().MapAssemblyOf<SomeMessageType>("anotherInputQueue"))
        .Options(o => o.AddTransientFaultBus())
        .Start();
```
