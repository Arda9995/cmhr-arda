$(document).ready(function () {
    var versionVideoPlayerID = $('#versionVideoPlayerID').val();
    var versionVideoCode = $('#versionVideoCode').val();

    LoadVideoPlayer(versionVideoPlayerID, versionVideoCode);
});

$('#kt_modal_1').on('shown.bs.modal', function () {
    // Fetch the hidden input values only after the modal is shown
    var versionVideoPlayerID = $('#versionVideoPlayerID').val();
    var versionVideoCode = $('#versionVideoCode').val();

    var newsVideoPlayerID = $('#newsVideoPlayerID').val();
    var newsVideoCode = $('#newsVideoCode').val();

    // Call the video player loader function
    LoadVideoPlayerModal(versionVideoPlayerID, versionVideoCode, newsVideoPlayerID, newsVideoCode);
});

try {
    const publishButton = document.getElementById('changeLive');
    publishButton.addEventListener('click', handleChangeLiveButtonClick);
} catch (error) {

}

function HandleRedirect(){
    var newsId = document.getElementById('newsId').value;
    window.location.href = `/News/Details/${newsId}`;
}

function ReloadFunc(){
    location.reload();
}
function handleChangeLiveButtonClick(e) {
    e.preventDefault();

    var url = window.location.pathname;
    var id = url.substring(url.lastIndexOf('/') + 1);

    Confirm(`${localizer.changeLiveMsg}`, '/News/PublishNow', id, HandleRedirect, ReloadFunc);

}

function LoadVideoPlayerModal(versionVideoPlayerID, versionVideoCode, newsVideoPlayerID, newsVideoCode) {
    if(versionVideoPlayerID == 1){

    }
    else if(versionVideoPlayerID == 2){
        dailymotion
            .createPlayer("my-dailymotion-player-version", {
                video: versionVideoCode,
                referrerPolicy: 'no-referrer-when-downgrade',
                params: {
                    mute: true,
                    customConfig: {
                        // keyvalues: 'video_position=inpage&page_category=' + slugKategoriArray.toString(),
                        plcmt: 2
                    }
                },
            })
            .then((player) => { console.log(player); player.mute(); })
            .catch((e) => console.error(e));
    }

    if(newsVideoPlayerID == 1){

    }
    else if(newsVideoPlayerID == 2){
        dailymotion
            .createPlayer("my-dailymotion-player-news", {
                video: newsVideoCode,
                referrerPolicy: 'no-referrer-when-downgrade',
                params: {
                    mute: true,
                    customConfig: {
                        // keyvalues: 'video_position=inpage&page_category=' + slugKategoriArray.toString(),
                        plcmt: 2
                    }
                },
            })
            .then((player) => { console.log(player); player.mute(); })
            .catch((e) => console.error(e));
    }
}

function LoadVideoPlayer(versionVideoPlayerID, versionVideoCode) {
    if(versionVideoPlayerID == 1){

    }
    else if(versionVideoPlayerID == 2){
        dailymotion
            .createPlayer("my-dailymotion-player", {
                video: versionVideoCode,
                referrerPolicy: 'no-referrer-when-downgrade',
                params: {
                    mute: true,
                    customConfig: {
                        // keyvalues: 'video_position=inpage&page_category=' + slugKategoriArray.toString(),
                        plcmt: 2
                    }
                },
            })
            .then((player) => { console.log(player); player.mute(); })
            .catch((e) => console.error(e));
    }
}