let submitEndpoint = "home/submit";

function submitSong() {

    let body = {
        songUrl: document.getElementById('songInput').value
    };

    fetch(submitEndpoint, {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(document.getElementById('songInput').value)
    })
        .then(response => response.json())
        .then(data => updateText(data));
}

function updateText(text) {
    document.getElementById("SongName").innerHTML = text;
}