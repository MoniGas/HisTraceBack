var nDIV;
var docEle = function () {
    return document.getElementById(arguments[0]) || false;
}

function openNewDiv(_id) {
    nDIV = _id;
    var m = "mask";
    if (docEle(_id)) document.body.removeChild(docEle(_id));
    if (docEle(m)) document.body.removeChild(docEle(m));

    //mask遮罩层  

    var newMask = document.createElement("div");
    newMask.id = m;
    newMask.style.position = "absolute";
    newMask.style.zIndex = "1";
    _scrollWidth = Math.max(document.body.scrollWidth, document.documentElement.scrollWidth);
    _scrollHeight = Math.max(document.body.scrollHeight, document.documentElement.scrollHeight);
    newMask.style.width = _scrollWidth + "px";
    newMask.style.height = _scrollHeight + "px";
    newMask.style.top = "0px";
    newMask.style.left = "0px";
    newMask.style.background = "#33393C";
    newMask.style.filter = "alpha(opacity=40)";
    newMask.style.opacity = "0.40";
    document.body.appendChild(newMask);

    //新弹出层  

    var newDiv = document.createElement("div");
    newDiv.id = _id;
    newDiv.style.position = "absolute";
    newDiv.style.zIndex = "9999";
    newDivWidth = 800;
    newDivHeight = 100;
    newDiv.style.width = newDivWidth + "px";
    newDiv.style.height = newDivHeight + "px";
    newDiv.style.top = (document.body.scrollTop + document.body.clientHeight / 2 - newDivHeight / 2) + "px";
    newDiv.style.left = (document.body.scrollLeft + document.body.clientWidth / 2 - newDivWidth / 2) + "px";
    //newDiv.style.background = "#EFEFEF";
    newDiv.style.background = "#33393C";
    newDiv.style.border = "0px solid #860001";
    newDiv.style.padding = "0px";
    newDiv.innerHTML = '<img src="/images/admin/loading.gif" alt="" />';
    //newDiv.innerHTML = '<font style=" font-size:24px; font-weight:bold;">正在加载中。。。</font>';
    document.body.appendChild(newDiv);

    //弹出层滚动居中  

    function newDivCenter() {
        newDiv.style.top = (document.body.scrollTop + document.body.clientHeight / 2 - newDivHeight / 2) + "px";
        newDiv.style.left = (document.body.scrollLeft + document.body.clientWidth / 2 - newDivWidth / 2) + "px";
    }
    if (document.all) {
        window.attachEvent("onscroll", newDivCenter);
    }
    else {
        window.addEventListener('scroll', newDivCenter, false);
    }

    //关闭新图层和mask遮罩层  

//    var newA = document.createElement("a");
//    newA.href = "#";
//    newA.innerHTML = "关闭";
//    newA.onclick = function () {
//        if (document.all) {
//            window.detachEvent("onscroll", newDivCenter);
//        }
//        else {
//            window.removeEventListener('scroll', newDivCenter, false);
//        }
//        document.body.removeChild(docEle(_id));
//        document.body.removeChild(docEle(m));
//        return false;
//    }
//    newDiv.appendChild(newA);
}

function cl() {
    document.body.removeChild(docEle(nDIV));
    document.body.removeChild(docEle('mask'));
} 
