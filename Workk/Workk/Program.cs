//Web için buldir
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Html'e ulaþmak için
app.UseStaticFiles();

//Anasayfa için Get endpointi olduðunu tanýmý
app.MapGet("/", async context =>
{   //yanýtýn html tipinde
    context.Response.ContentType = "text/html";
    //Html dosyasýný kullanýcýya gönderiyoruz
    await context.Response.SendFileAsync("wwwroot/index.html");
});

//Metin analizi için post endpoint'i tanýmlýyoruz
app.MapPost("/analyze", async (HttpContext context) =>
{
    //Form verisini oku
    var form = await context.Request.ReadFormAsync();
    //Kullanýcýnýn gönderdigi metin 
    var text = form["inputText"];

    // Kelime analiz fonksiyonu buraya gelecek
    var result = AnalyzeText(text);
    //Json formatý
    return Results.Json(result);
});

app.Run();
//Metin analýz fonk
Dictionary<string, int> AnalyzeText(string text)
{
    //Metni parçala
    var words = text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    var frequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

    foreach (var word in words)
    {
        //Kelimeden sadece harfleri alýyoruz(Temizleniyor)
        var cleanedWord = new string(word.Where(char.IsLetter).ToArray()).ToLower();
        if (!string.IsNullOrEmpty(cleanedWord))
        {
            //sayacý artýr sozlukteyse yoksa ekle
            if (frequency.ContainsKey(cleanedWord))
                frequency[cleanedWord]++;
            else
                frequency[cleanedWord] = 1;
        }
    }

    return frequency.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
}