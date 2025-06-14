using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();
builder.Services.AddOpenTelemetry().UseAzureMonitor(options =>
{
    options.ConnectionString =
        "InstrumentationKey=9fe82e13-e1b0-489a-8c01-13977db9dc95;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=f7cf647e-de75-4589-a116-183bf472e1ea";
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseSentryTracing();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hit the /albums endpoint to retrieve a list of albums!");
});

app.MapGet("/albums", () =>
{
    return Album.GetAll();
})
.WithName("GetAlbums");

app.MapGet("/albums/{id:int}", async (int id) =>
{
    return Album.GetById(id);
})
.WithName("GetAlbum");

app.MapGet("/error", () =>
{
    try
    {
        throw null;
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        SentrySdk.CaptureException(e);
    }
    return "Something went wrong";
});

app.Run();

record Album(int Id, string Title, string Artist, double Price, string Image_url, string Description)
{
    private static List<Album> albums =
    [
        new Album(1, "You, Me and an App Id", "Daprize", 10.99, "https://aka.ms/albums-daprlogo", "A genre-blending journey through digital love and identity. This album explores the emotional intersections of technology and human connection, with synth-heavy beats and introspective lyrics that echo the complexities of modern relationships. Each track is a reflection on how our digital personas shape and sometimes distort our real-world emotions, making this a deeply resonant experience for the modern listener."),
        new Album(2, "Seven Revision Army", "The Blue-Green Stripes", 11.99, "https://aka.ms/albums-containerappslogo", "An explosive concept album inspired by the chaos of creation and iteration. With gritty guitar riffs and rebellious energy, it channels the spirit of resistance and reinvention\u2014perfect for coding sprints or late-night debugging sessions. Each track represents a version of the same idea, evolving through feedback, frustration, and breakthroughs."),
        new Album(3, "Scale It Up", "KEDA Club", 11.99, "https://aka.ms/albums-kedalogo", "A motivational anthem for startups and dreamers. This album fuses electronic pop with orchestral elements, capturing the highs and lows of building something from the ground up. From the first pitch to the final IPO, each track is a step toward something bigger, celebrating ambition, resilience, and the thrill of exponential growth."),
        new Album(4, "Lost in Translation", "MegaDNS", 12.99, "https://aka.ms/albums-envoylogo", "A moody, atmospheric collection of tracks that delve into miscommunication, cultural dissonance, and the beauty of the unspoken. With ambient textures and haunting vocals, it\u2019s a sonic exploration of what gets lost\u2014and found\u2014between the lines. Ideal for quiet nights and introspective moments, this album invites listeners to find meaning in ambiguity."),
        new Album(5, "Lock Down Your Love", "V is for VNET", 12.99, "https://aka.ms/albums-vnetlogo", "Set against the backdrop of a world in isolation, this album is a heartfelt ode to connection in uncertain times. Blending acoustic warmth with modern production, it\u2019s a reminder that love can thrive even behind closed doors. Each track captures a different facet of intimacy, from longing and distance to rediscovery and resilience."),
        new Album(6, "Sweet Container O' Mine", "Guns N Probeses", 14.99, "https://aka.ms/albums-containerappslogo", "A playful, DevOps-inspired twist on classic rock. This tongue-in-cheek tribute to containerization and cloud-native culture delivers punchy tracks with clever lyrics and nostalgic riffs. Ideal for developers with a sense of humor and a love for CI/CD, this album turns infrastructure into art.")
    ];
    
     public static List<Album> GetAll(){
        return albums; 
     }

     public static Album? GetById(int id)
     {
         return albums.FirstOrDefault(x => x.Id == id);
     }
}