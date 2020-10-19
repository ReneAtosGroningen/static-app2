//--------------------------------------------------
//JavaScript code voor de 'Galgje' static website
//Oktober 2020, Rene Hartsema
//--------------------------------------------------

// API url
gameUrl = "/api/playgame";


function SendCharacter() {
    var letterToSend = "a";
    var secretCode = "ksddfhkjdfh";
    $.ajax({
        method: "GET",
        url: gameUrl,
        data: { "letter": letterToSend,
                "code" : secretCode },
        contentType: "application/json",
        dataType: 'json'
    }).done(function (res) {
        ProcessApiResponse(res);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("API call failed: " + textStatus + ", " + errorThrown);
    }).always(function (res) {
        ProcessScore(res);
    });
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
    }).always(function (res) {
        ProcessScore(res);
    });
}

// Functie om de API-reply terug in het scherm te zetten
function ProcessApiResponse(apiResponse) {
    $("#code").text(apiResponse.code);
    $("#woord").text(MakeWider(apiResponse.woord));
    $("#gespeeld").text(MakeWider(apiResponse.gespeeldeLetters));
    $("#letter").val("");
    $("#score").text(apiResponse.score);
}

function ProcessScore(apiResponse) {
    if (apiResponse.woord.indexOf("_") == -1){
        // no minus-characters --> word found!
        $("#verzendKnop").hide();
        alert("Je hebt het woord geraden!");
    }
    if (apiResponse.score >=10) {
        // teveel pogingen fout!
        $("#verzendKnop").hide();
        alert("OEPS... Je hebt het woord niet op tijd geraden!");
    }
}


function MakeWider(inputString) {
    wideText="";
    if (inputString == "") {
        wideText="?";
    }
    
    for (var x = 0; x < inputString.length; x++) {
        var c = inputString.charAt(x);
        wideText += c + " ";
    }
    return wideText;
}
