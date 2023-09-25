using MESBG.Firebase;
using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Api;
using Microsoft.Extensions.Hosting.Internal;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
// Inicializa FirebaseApp con las credenciales del archivo JSON
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase-credenciales.json"),
    ProjectId = "mesbg-gts"
});

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "firebase-credenciales.json");

// Obtener una instancia de FirebaseAuth
var auth = FirebaseAuth.DefaultInstance;

// Crea una nueva instancia de FirebaseManager con FirestoreDb y la ruta del archivo de credenciales
FirestoreDb db = FirestoreDb.Create(builder.Configuration["Firebase:ProjectId"]);
FirebaseManager firebase = new FirebaseManager(db, "firebase-credenciales.json");

// Agrega la instancia de FirebaseManager al servicio
builder.Services.AddSingleton(firebase);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();










