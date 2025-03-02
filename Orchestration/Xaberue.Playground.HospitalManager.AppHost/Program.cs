var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var mssql = builder.AddSqlServer("hospital-manager-sql", port: 65379).WithLifetime(ContainerLifetime.Persistent);

var patientsDb = mssql.AddDatabase("PatientsDb");
var doctorsDb = mssql.AddDatabase("DoctorsDb");

var patientsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Patients_WebAPI>("patients-webapi")
    .WithReference(patientsDb)
    .WaitFor(patientsDb);

var doctorsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Doctors_WebAPI>("doctors-webapi")
    .WithReference(doctorsDb)
    .WaitFor(doctorsDb);

builder.AddProject<Projects.Xaberue_Playground_HospitalManager_WebUI_Server>("webui")
    .WithReference(cache)
    .WaitFor(cache)
    .WithEnvironment("ConnectionStrings__PatientsApiUrl", patientsApi.GetEndpoint("https"))
    .WithEnvironment("ConnectionStrings__DoctorsApiUrl", doctorsApi.GetEndpoint("https"));

builder.Build().Run();