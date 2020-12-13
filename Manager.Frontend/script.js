function connect() {
    var port = document.getElementById('port').value
    csharp.connect(Number(port)).then(function(result) {
        if (!result) setContent('连接失败')
    })
}

function disconnect() {
    csharp.disconnect()
}

function setContent(content) {
    document.getElementById('content').innerText = content
}
