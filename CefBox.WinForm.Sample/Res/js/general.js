
var injectName = "app";
var jsCallback = "appCallback";
var frameId = getQueryString("id");
var moveTimer = 0;
var moving = false;

window[jsCallback] = function (name, usedLater, data) {
    window[name] && window[name](data);
    !usedLater && delete window[name];
}

function cefRequest(path, param, callback) {
    frameId = frameId || "mainFrame";
    var callbackName = new Date().getTime().toString();
    window[callbackName] = function (result) {
        console.log("callback begin");
        console.log(result);
        if (result.Code != 0) {
            alert("Msg:" + result.Message);
        }
        callback && callback(result);

        console.log("callback end");
    };

    window[injectName].fetch(path, JSON.stringify({ frameId, data: param, callback: callbackName }));
}

function getQueryString(name) {
    var reg = new RegExp('(^|&)' + name + '=([^&]*)(&|$)', 'i');
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return null;
}
var toolbar = document.getElementById("toolbar");
if (toolbar) {
    toolbar.addEventListener("dblclick", function (e) {
        cefRequest("frame/maximum");
        e.stopPropagation();
    }, false);

    toolbar.addEventListener("mousedown", function (e) {
        moveTimer = new Date().getTime();
    });

    toolbar.addEventListener("mouseout", function (e) {
        moveTimer = 0;
    }, false);

    toolbar.addEventListener("mouseup", function (e) {
        moveTimer = 0;
    }, false);

    toolbar.addEventListener("mousemove", function (e) {
        if (moveTimer == 0 || moving) {
            return;
        }

        var now = new Date().getTime();
        if (now - moveTimer > 50) {
            moving = true;
            cefRequest("frame/move", null, function (data) {
                moveTimer = 0;
                moving = false;
            });
        }
    }, false);
}

