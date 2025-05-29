# Xaberue.Playground.HospitalManager

Sample project simulating a real hospital management system using mainly C# and .NET, but other technologies as well orchestrated with Aspire.

Aspire is the orchestration project, which contains the main entry point for the application. It is responsible for initializing the application and starting the necessary services.


# Project Structure
_TBD_
![Alt text here](.resources/hospital_manager_diagram.png)

# Samples included
- Aspire orchestration
  - .NET services
  - Angular app
  - rabbitMQ
  - SQL Server
  - Redis
  - MongoDb
- Blazor
  - SSR
  - Interactive WebAssembly
  - Dealing with both server and client side
  - How to deal with SignalR connections in Blazor SSR 
- SignalR
- Identity
- Minimal APIs
  - How to work with RouteGroupBuilder
- gRPC
- EntityFrameworkCore
- Central package management
- SLNX


# Known Issues

- Identity is not used at the moment, roles are not considered within the manager UI. (only login and register) _(TBI)_
- Working with multiple instances, check logs, check servers. _(TBI)_
- DistributedCache / HybridCache is not properly adding traces to OTEL collector. _(TBI)_ _(TBF)_
- Change how queues and exchanges are created, should be in an different manner since some consumers crashes when starts for first time. _(TBF)_

_(TBF)_: To Be Fixed
_(TBI)_: To Be Implemented 