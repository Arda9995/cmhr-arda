var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://cms.cumhuriyet.com.tr:7283/signalRHub",
        {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        })
    .build();

connection.start();

connection.on("getPromptResult", function (response) {
    var promptResult = document.getElementById('promptResult');
    promptResult.innerHTML += response;
})

connection.on('GetTitleGenerationResult', function (response) {
    var titleInput = document.getElementById('Title');
    titleInput.value += response;
});

connection.on('GetSubtitleGenerationResult', function (response) {
    var subtitleInput = document.getElementById('Subtitle');
    subtitleInput.value += response;
});

function sendContentToAi(content) {
    connection.invoke('sendContentToAi', content)
}

function sendTitleGenerationRequestToAi(promptId) {
    var titleInput = document.getElementById('Title');
    titleInput.value = '';
    var subtitle = document.getElementById('Subtitle').value;
    var content = document.getElementById('Content').value;
    var tags = document.getElementById('Tags').value;
    var titleCheck = document.getElementById('titleRequestType').value;
    if (titleCheck === "Create") {
        connection.invoke('SendTitleGenerationRequestToAi', promptId, subtitle, content, tags, null);
    } else {
        var title = document.getElementById('Title').value;
        connection.invoke('SendTitleGenerationRequestToAi', promptId, subtitle, content, tags, title);
    }
}

function sendSubtitleGenerationRequestToAi(promptId) {
    var subtitleInput = document.getElementById('Subtitle');
    subtitleInput.value = '';
    var title = document.getElementById('Title').value;
    var content = document.getElementById('Content').value;
    var tags = document.getElementById('Tags').value;
    var subtitleCheck = document.getElementById('subtitleRequestType').value;
    if (subtitleCheck === "Create") {
        connection.invoke('SendSubtitleGenerationRequestToAi', promptId, title, content, tags, null);
    } else {
        var subtitle = document.getElementById('Subtitle').value;
        connection.invoke('SendSubtitleGenerationRequestToAi', promptId, title, content, tags, subtitle);
    }
}

function applyGeneratedContent(){
    var generatedContent = document.getElementById('generatedContent').innerHTML;
    
    var contentInput = document.getElementById('Content');

    $('.summernote').summernote('code', generatedContent);
    
}