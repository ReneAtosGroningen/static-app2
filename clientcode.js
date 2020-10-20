//--------------------------------------------------
//JavaScript code voor de 'Galgje' static website
//Oktober 2020, Rene Hartsema
//--------------------------------------------------

// API url
gameUrl =  "/api/playgame";

// some defaults
$(document).ready(function(){
    $("#verzendKnop").hide();
    $("#letterOption").hide();
  });

function SendCharacter() {
    var letterToSend = $("#letter").val();
    var secretCode = $("#code").text();
    if (IsValidUserInput(letterToSend)) {
        $.ajax({
            method: "POST",
            url: gameUrl,
            data: { "letter": letterToSend.toLowerCase(),
                    "code" : secretCode }
            ,contentType: "application/json"
            ,dataType: "json"
        }).done(function (res) {
            ProcessApiResponse(res);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            alert("API call failed: " + textStatus + ", " + errorThrown);
        }).always(function (res) {
            ProcessScore(res);
        });
    }
}

function IsValidUserInput(letterToSend) {
    var IsInputValid = true;
    if (letterToSend == "" || letterToSend == " ") {
        alert("Geef een letter om te spelen.")
        IsInputValid = false;
        $("#letter").val("").focus();
    }
    else if (!isNaN(letterToSend)) {
        alert("Geef een letter om te spelen, geen getal.")
        IsInputValid = false;
        $("#letter").val("").focus();
    }
    else if ("qwertyuiopasdfghjklzxcvbnm".indexOf(letterToSend.toLowerCase()) == -1) {
        alert("Geef een letter om te spelen, geen leestekens.")
        IsInputValid = false;
        $("#letter").val("").focus();
    }
    return IsInputValid;
}

//Functie voor de knop "Nieuw Spel"
function StartNewGame() {  
    $("#verzendKnop").show();
    $("#letterOption").show();

    $.ajax({
        method: "GET",
        url: gameUrl,
        contentType: "application/json",
        dataType: "json"
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
    $("#score").text(10 - apiResponse.score);
    $("#hangManImage").attr("src", `img/hm${apiResponse.score}.png` );

    $("#letter").val("").focus();
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
