var msg = document.getElementById("msg");

document.getElementById("close").addEventListener("click", function (e) {
    var hideToTray = this.attributes["hide2Tray"];
    if (hideToTray) {
        hideToTray = hideToTray.value || false;
    }
    cefRequest('frame/close', { hideToTray });
}, false);

function mathRequest() {
    var resultLabel = document.getElementById("result");

    msg.textContent = "";
    resultLabel.textContent = "";

    var first = document.getElementById("first").value;
    var second = document.getElementById("second").value;
    cefRequest("math/" + document.getElementById("op").value, { first, second }, function (result) {
        if (result.Code != 0) {
            msg.textContent = "Error: " + result.Message;
        }
        else {
            resultLabel.textContent = result.Data;
        }
    });
}

function openNew(isModal) {
    isModal = isModal || false;
    var id = new Date().getTime();
    cefRequest("frame/open", { id, isModal, width: 600, height: 400, contentPath: "sub.html?id=" + id });
}

function original(isConfirm) {
    if (isConfirm) {
        msg.textContent = "";
        var result = confirm("Continue?") ? "yes" : "no";
        msg.textContent = "You choose:" + result;
    }
    else {
        alert("A alert from js.");
    }
}

function resetFrame() {
    cefRequest("frame/reset", { width: 700, height: 500, position: 2 });
}

function changeMain() {

    cefRequest("frame/open", { width: this.innerWidth + 100, height: this.innerHeight + 70, contentPath: "index.html", isMain: true });
}

function nothing() {
    cefRequest("nothing/test");
}

