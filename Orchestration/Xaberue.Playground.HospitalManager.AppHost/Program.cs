var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache").WithRedisInsight();

var mssql = builder.AddSqlServer("hospital-manager-sql", port: 65379)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var mongodb = builder.AddMongoDB("hospital-manager-mongodb", port: 52099)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var rabbitmq = builder.AddRabbitMQ("HospitalManagerServiceBroker")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume(isReadOnly: false)
    .WithManagementPlugin(port: 15672);

var identityDb = mssql.AddDatabase("IdentityDb");
var patientsDb = mssql.AddDatabase("PatientsDb");
var doctorsDb = mssql.AddDatabase("DoctorsDb");
var appointmentsDb = mongodb.AddDatabase("AppointmentsDb");

var patientsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Patients_WebAPI>("patients-webapi")
    .WithReference(patientsDb)
    .WaitFor(patientsDb);

var doctorsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Doctors_WebAPI>("doctors-webapi")
    .WithReference(doctorsDb)
    .WaitFor(doctorsDb);

var appointmentsApi = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_Appointments_WebAPI>("appointments-webapi")
    .WithReference(appointmentsDb)
    .WithReference(rabbitmq)
    .WaitFor(appointmentsDb)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.Xaberue_Playground_HospitalManager_WebUI_Server>("hospital-manager-webui")
    .WithReference(identityDb)
    .WithReference(cache)
    .WithReference(rabbitmq)
    .WaitFor(cache)
    .WaitFor(identityDb)
    .WithEnvironment("ConnectionStrings__PatientsApiUrl", patientsApi.GetEndpoint("https"))
    .WithEnvironment("ConnectionStrings__DoctorsApiUrl", doctorsApi.GetEndpoint("https"));

var appointmentsPanelServer = builder.AddProject<Projects.Xaberue_Playground_HospitalManager_AppointmentsPanel_Server>("appointmentspanel-server")
    .WithEnvironment("ConnectionStrings__AppointmentsApiUrl", appointmentsApi.GetEndpoint("https"))
    .WithReference(rabbitmq);

var appointmentsPanelClient = builder.AddNpmApp("appointmentspanel-client", "../../Frontend/Xaberue.Playground.HospitalManager.AppointmentsPanel.Client")
    .WithReference(appointmentsPanelServer)
    .WaitFor(appointmentsPanelServer)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

appointmentsPanelServer
    .WithEnvironment("AppointmentsPanelClientUrl", appointmentsPanelClient.GetEndpoint("http"));

builder.Build().Run();