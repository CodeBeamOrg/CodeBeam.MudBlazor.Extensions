
class MudScrollManagerExtended {

    scrollToMiddle(parentId, childId) {

        let parent = document.getElementById(parentId);
        let child = document.getElementById(childId);

        parent.scrollTop = (child.offsetTop - parent.offsetHeight) + (parent.offsetHeight / 2) + (child.offsetHeight / 2);
    }
};
window.mudScrollManagerExtended = new MudScrollManagerExtended();

function auto_size(element) {
    element.style.height = "5px";
    element.style.height = (element.scrollHeight + 4) + "px";
}

function getcss(classe, cssprop) {
    const element = document.querySelector(classe);
    if (cssprop == "backgroundcolor") {
        return element.style.backgroundColor;
    }
    else if (cssprop == "transition") {
        return element.style.transition;
    }
    else if (cssprop == "width") {
        return element.style.width;
    }
    
}

function setcss(classe, csstype, value) {
    const elements = document.querySelectorAll(classe);
    if (csstype == "backgroundcolor") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.backgroundColor = value;
        }
    }
    else if (csstype == "borderradius") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.borderRadius = value;
        }
    }
    else if (csstype == "color") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.color = value;
        }
    }
    else if (csstype == "height") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.height = value;
        }
    }
    else if (csstype == "transition") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.transition = value;
        }
    }
    else if (csstype == "width") {
        for (let i = 0; i < elements.length; i++) {
            elements[i].style.width = value;
        }
    }
}