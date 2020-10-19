//--------------------------------------------------
//JavaScript code voor de 'Galgje' static website
//Oktober 2020, Rene Hartsema
//--------------------------------------------------

// API url
gameUrl = "/api/playgame";
gameUrlComplete = "https://red-field-03d09a603.azurestaticapps.net/api/playgame";

function SendCharacter() {

    alert("Hi");
}

function StartNewGame() {  
    
    $.ajax({
        method: "GET",
        url: "https://red-field-03d09a603.azurestaticapps.net/api/playgame",
        contentType: "application/json",
        dataType: 'json'
    }).done(function (res) {
        alert(res);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        alert("AJAX call failed: " + textStatus + ", " + errorThrown);
    });


}

