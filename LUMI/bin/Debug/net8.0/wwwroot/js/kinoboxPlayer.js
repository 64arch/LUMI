function initializeKinoboxPlayer(tmdbId) {
    console.log("Init Kinobox with ID:", tmdbId);

    const parsedTmdbId = parseInt(tmdbId, 10);
    if (!isNaN(parsedTmdbId)) {
        kbox('.kinobox_player', {
            search: {
                tmdb: parsedTmdbId
            },
            menu: {
                enable: false,
                open: false
            }
        });
    } else {
        console.error("Incorrect TmdbID:", tmdbId);
    }
}

function scrollToId(id) {
    const element = document.getElementById(id);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
    }
}
