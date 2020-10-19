//--------------------------------------------------
//JavaScript code voor de 'Galgje' static website
//Oktober 2020, Rene Hartsema
//--------------------------------------------------

// API url
gameUrl = "/api/playgame";


function SendCharacter() {

    alert("Hi");
}

//Functie voor de knop "Nieuw Spel"
function StartNewGame() {  
    
    $.ajax({
        method: "GET",
        url: gameUrl,
        contentType: "application/json",
        dataType: 'json'
    }).done(function (res) {
        ProcessApiResponse(res);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("API call failed: " + textStatus + ", " + errorThrown);
    });

}

// Functie om de API-reply terug in het scherm te zetten
function ProcessApiResponse(response) {
    $("#code").text(response.code);
    $("#woord").text(MakeWider(response.woord));
    $("#letter").val("");
}

function MakeWider(inputString) {
    wideText="";
    for (var x = 0; x < inputString.length; x++)
    {
        var c = inputString.charAt(x);
        wideText = wideText + c + " ";
    }
    return wideText;
}
