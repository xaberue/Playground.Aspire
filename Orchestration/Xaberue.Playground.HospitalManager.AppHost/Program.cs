var builder = DistributedApplication.CreateBuilder(args);

var noSqlTypeArgs = builder.Configuration["noSqlDb"];

var patientsApiKey = builder.Configuration["Auth:PatientsApiKey"];
var doctorsApiKey = builder.Configuration["Auth:DoctorsApiKey"];
var appointmentsApiKey = builder.Configuration["Auth:AppointmentsApiKey"];

var cache = builder.AddRedis("cache").WithRedisInsight();

var mssql = builder.AddSqlServer("hospital-manager-sql", port: 65379)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var rabbitmq = builder.AddRabbitMQ("RabbitMQ")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume(isReadOnly: false)
    .WithManagementPlugin(port: 15672);

var identityDb = mssql.AddDatabase("IdentityDb");
var patientsDb = mssql.AddDatabase("PatientsDb");
var doctorsDb = mssql.AddDatabase("DoctorsDb");

var patientsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Patients_WebAPI>("patients-webapi")
    .WithEnvironment("Auth__ApiKeyToken", patientsApiKey)
    .WithReference(patientsDb)
    .WaitFor(patientsDb);

var doctorsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Doctors_WebAPI>("doctors-webapi")
    .WithEnvironment("Auth__ApiKeyToken", doctorsApiKey)
    .WithReference(doctorsDb)
    .WaitFor(doctorsDb);

var appointmentsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Appointments_WebAPI>("appointments-webapi")
    .WithEnvironment("Auth__ApiKeyToken", appointmentsApiKey)
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.Xaberue_Playground_HospitalManager_WebUI_Server>("hospital-manager-webui")
    .WithReference(cache)
    .WithReference(rabbitmq)
    .WithReference(identityDb)
    .WaitFor(cache)
    .WaitFor(rabbitmq)
    .WaitFor(identityDb)
    .WithEnvironment("Auth__AppointmentsApiKey", appointmentsApiKey)
    .WithEnvironment("Auth__DoctorsApiKey", doctorsApiKey)
    .WithEnvironment("Auth__PatientsApiKey", patientsApiKey)
    .WithEnvironment("ConnectionStrings__PatientsApiUrl", patientsApi.GetEndpoint("https"))
    .WithEnvironment("ConnectionStrings__DoctorsApiUrl", doctorsApi.GetEndpoint("https"));

var appointmentsPanelServer = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_AppointmentsPanel_Server>("appointmentspanel-server")
    .WithEnvironment("Auth__AppointmentsApiKey", appointmentsApiKey)
    .WithEnvironment("ConnectionStrings__AppointmentsApiUrl", appointmentsApi.GetEndpoint("https"))
    .WithReference(rabbitmq)
    .WaitFor(rabbitmq)
    .WaitFor(appointmentsApi);

var appointmentsPanelClient = builder.AddJavaScriptApp("appointmentspanel-client", "../../Frontend/Xaberue.Playground.HospitalManager.AppointmentsPanel.Client", runScriptName: "start")
    .WithReference(appointmentsPanelServer)
    .WaitFor(appointmentsPanelServer)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

appointmentsPanelServer
    .WithEnvironment("AppointmentsPanelClientUrl", appointmentsPanelClient.GetEndpoint("http"));

if (noSqlTypeArgs == "MongoDb")
{
    var mongodb = builder.AddMongoDB("hospital-manager-mongodb", port: 54099)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume();

    var appointmentsDb = mongodb.AddDatabase("AppointmentsDb");

    appointmentsApi
        .WithReference(appointmentsDb)
        .WaitFor(appointmentsDb);
}
else if (noSqlTypeArgs == "RavenDB")
{
    var ravenServer = builder.AddRavenDB("hospital-manager-ravendb")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume();

    var appointmentsDb = ravenServer.AddDatabase("AppointmentsDb");

    appointmentsApi
        .WithReference(appointmentsDb)
        .WaitFor(appointmentsDb);
}

builder.Build().Run();