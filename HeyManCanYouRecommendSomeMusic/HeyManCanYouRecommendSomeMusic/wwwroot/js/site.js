let submitEndpoint = "home/submit";
let submitNewEndpoint = "home/submit-new";
let populateEndpoint = "home/populate";

document.getElementById('populate').onclick = populateDB;

function populateDB() {
	document.getElementById('info').innerHTML = "Please wait...";
	fetch(populateEndpoint, {
		method: 'POST',
		headers: {
			'Accept': 'application/json',
			'Content-Type': 'application/json'
		},
		body: {}
	})
		.then(res => res.json())
		.then(data => document.getElementById('info').innerHTML = "Database populated!")
}

function submitSong() {

	document.getElementById('info').innerHTML = "Please wait...";

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
		.then(data => updateSongLists(data));
	
}

function submitNewSong() {
	var songNameInput = document.getElementById("SongNameInput");
	var artistInput = document.getElementById("ArtistInput");
	var genreInput = document.getElementById("GenreInput");
	var durationInput = document.getElementById("DurationInput");
	var bpmInput = document.getElementById("BpmInput");

	var song = {
		name: songNameInput.value,
		band: artistInput.value,
		genre: genreInput.value,
		duration: durationInput.value,
		bpm: bpmInput.value
	}
	console.log(JSON.stringify(song));
	fetch(submitNewEndpoint,
		{
			method: 'POST',
			headers: {
				'Accept': 'application/json',
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(song)
		}).then(response => response.json()).then(data => {
			if (data.succ == true) {
				window.alert("Song succesfully added to the database!");
				refreshNewSongForm();
			} else {
				window.alert("Song was not added to the database. Try again with different parameters.");
			}
	})
}

function refreshNewSongForm() {
	var songNameInput = document.getElementById("SongNameInput");
	var artistInput = document.getElementById("ArtistInput");
	var genreInput = document.getElementById("GenreInput");
	var durationInput = document.getElementById("DurationInput");
	var bpmInput = document.getElementById("BpmInput");

	songNameInput.value = "";
	artistInput.value = "";
	genreInput.value = "";
	durationInput.value = "";
	bpmInput.value = "";

	document.getElementById("NewSongForm").style.display = "none";
}

function updateText(text) {
    document.getElementById("SongName").innerHTML = text;
}

function updateSongLists(data) {
/*console.log(data);*/
	document.getElementById('info').innerHTML = "";
	if (data.succ == false) {
		document.getElementById("NewSongForm").style.display = "block";
		document.getElementById("SongLists").style.display = "none";
		return;
	}
	document.getElementById("SongLists").style.display = "block";
	refreshSongLists();

	var tableArtist = document.getElementById("SongListsArtist");
	var tableGenre = document.getElementById("SongListsGenre");
	var tableDuration = document.getElementById("SongListsDuration");
	var tableBpm = document.getElementById("SongListsBpm");
	var tableRelationship = document.getElementById("SongListsRelationship");

	tableArtist.innerHTML += "<tbody>";
	tableGenre.innerHTML += "<tbody>";
	tableDuration.innerHTML += "<tbody>";
	tableBpm.innerHTML += "<tbody>";
	tableRelationship.innerHTML += "<tbody>";
	data.artist.forEach(song => {
		tableArtist.innerHTML += "<tr><td>" +
			song.band +
			"</td><td>" +
			song.name +
			"</td><td></tr>";
	});
	data.genre.forEach(song => {
		tableGenre.innerHTML += "<tr><td>" +
			song.band +
			"</td><td>" +
			song.name +
			"</td><td>" +
			song.genre +
			"</td></tr>";
	});
	data.duration.forEach(song => {
		tableDuration.innerHTML += "<tr><td>" +
			song.band +
			"</td><td>" +
			song.name +
			"</td><td>" +
			song.duration +
			"</td></tr>";
	});
	data.bpm.forEach(song => {
		tableBpm.innerHTML += "<tr><td>" +
			song.band +
			"</td><td>" +
			song.name +
			"</td><td>" +
			song.bpm +
			"</td></tr>";
	});
	data.relationship.forEach(song => {
		tableRelationship.innerHTML += "<tr><td>" +
			song.band +
			"</td><td>" +
			song.name +
			"</td><td></tr>";
	});
	tableArtist.innerHTML += "</tbody>";
	tableGenre.innerHTML += "</tbody>";
	tableDuration.innerHTML += "</tbody>";
	tableBpm.innerHTML += "</tbody>";
	tableRelationship.innerHTML += "</tbody>";
}

function refreshSongLists() {
	document.getElementById("NewSongForm").style.display = "none"; //hide the new song form

	var tableArtist = document.getElementById("SongListsArtist");
	var tableGenre = document.getElementById("SongListsGenre");
	var tableDuration = document.getElementById("SongListsDuration");
	var tableBpm = document.getElementById("SongListsBpm");
	var tableRelationship = document.getElementById("SongListsRelationship");

	tableArtist.innerHTML =
		"<thead><tr><th scope=\"col\">Artist</th><th scope=\"col\">Song</th></tr></thead>";
	tableGenre.innerHTML = "<thead><tr><th scope=\"col\">Artist</th><th scope=\"col\">Song</th><th scope=\"col\">Genre</th></tr></thead>";
	tableDuration.innerHTML = "<thead><tr><th scope=\"col\">Artist</th><th scope=\"col\">Song</th><th scope=\"col\">Duration</th></tr></thead>";
	tableBpm.innerHTML = "<thead><tr><th scope=\"col\">Artist</th><th scope=\"col\">Song</th><th scope=\"col\">Bpm</th></tr></thead>";
	tableRelationship.innerHTML =
		"<thead><tr><th scope=\"col\">Artist</th><th scope=\"col\">Song</th></tr></thead>";
}